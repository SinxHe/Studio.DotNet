using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sinx.Net.Http.Headers;
using Sinx.AspNetCore.WebUtilities;
// ReSharper disable MemberCanBePrivate.Global
namespace Sinx.AspNetCore.Http.Features
{
	public class FormFeature : IFormFeature
	{
		private IFormCollection _form;

		public FormFeature(IFormCollection form)
		{
			_form = form;
		}

		public FormFeature(HttpRequest request/*Form Options options  TODO*/)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request));
			}
			_request = request;
		}

		private readonly HttpRequest _request;
		private MediaTypeHeaderValue ContentType
		{
			get
			{
				MediaTypeHeaderValue.TryParse(_request.ContentType, out var mt);
				return mt;
			}
		}

		public bool HasFormContentType
		{
			get
			{
				if (Form != null) { return true; }
				var contentType = ContentType;
				return HasApplicationFormContentType(contentType) ||
					HasMultipartFormContentType(contentType);
			}
		}

		private static bool HasApplicationFormContentType(MediaTypeHeaderValue contentType)
		{
			// Content-Type: application/x-www-form-urlencoded; charset=utf-8
			return contentType != null && contentType.MediaType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase);
		}

		private static bool HasMultipartFormContentType(MediaTypeHeaderValue contentType)
		{
			// Content-Type: multipart/form-data; boundary=----WebKitFormBoundarymx2fSWqWSd0OxQqq
			return contentType != null && contentType.MediaType.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase);
		}
		// TODO HasFormDataContentDisposition() HasFileContentDisposition()
		public IFormCollection Form
		{
			get { return _form; }
			set
			{
				// TODo
				_parsedFormTask = null;
				_form = value;
			}
		}

		public IFormCollection ReadForm()
		{
			if (Form != null)
			{
				return Form;
			}
			if (!HasFormContentType)
			{
				throw new InvalidOperationException("Incorrect Content-Type: " + _request.ContentType);
			}
			// TODO: Issue #456 Avoid Sync-over-Async http://blogs.msdn.com/b/pfxteam/archive/2012/04/13/10293638.aspx
			// TODO: How do we prevent thread exhaustion?
			return ReadFormAsync().GetAwaiter().GetResult();
		}

		public Task<IFormCollection> ReadFormAsync() => ReadFormAsync(CancellationToken.None);
		private Task<IFormCollection> _parsedFormTask;
		public Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken)
		{
			// Avoid state machine and task allocation for repeated reads
			return _parsedFormTask ??
			       (_parsedFormTask = Form != null ? Task.FromResult(Form) : InnerReadFormAsync(cancellationToken));
		}

		private async Task<IFormCollection> InnerReadFormAsync(CancellationToken cancellationToken)
		{
			if (!HasFormContentType)
			{
				throw new InvalidOperationException("Incorrect Content-Type: " + _request.ContentType);
			}
			cancellationToken.ThrowIfCancellationRequested();
			//if (_options.BufferBody)
			//{
			//	_request.EnableRewind(_options.MemoryBufferThreshold, _options.BufferBodyLengthLimit);
			//} // TODO
			FormCollection formFields = null;
			// FormFileCollection files = null;
			using (cancellationToken.Register(state => ((HttpContext)state).Abort(), _request.HttpContext))
			{
				var contentType = ContentType;
				// Check the content-type
				if (HasApplicationFormContentType(contentType))
				{
					var encoding = FilterEncoding(contentType.Encoding);
					using (var formReader = new FormReader(_request.Body, encoding))
					{
						formFields = new FormCollection(await formReader.ReadFormAsync(cancellationToken));
					}
				}
				else if (HasMultipartFormContentType(contentType))
				{
					throw new NotImplementedException();
				}

				Form = formFields ?? FormCollection.Empty;

				// Rewind so later readers don't have to.
				if (_request.Body.CanSeek)
				{
					_request.Body.Seek(0, System.IO.SeekOrigin.Begin);
				}

				return Form;
			}
		}
		private Encoding FilterEncoding(Encoding encoding)
		{
			// UTF-7 is insecure and should not be honored. UTF-8 will succeed for most cases.
			if (encoding == null || Encoding.UTF7.Equals(encoding))
			{
				return Encoding.UTF8;
			}
			return encoding;
		}
	}
}
