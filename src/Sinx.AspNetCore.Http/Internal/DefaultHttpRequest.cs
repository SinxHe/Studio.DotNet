using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sinx.AspNetCore.Http.Features;
using Sinx.Net.Http.Headers;

// ReSharper disable MemberCanBeProtected.Global

namespace Sinx.AspNetCore.Http.Internal
{
	/// <summary>
	/// AspNetCore 默认使用的请求类
	/// </summary>
	public class DefaultHttpRequest : HttpRequest
	{
		public override HttpContext HttpContext => _context;

		private IHttpRequestFeature HttpRequestFeature
			=> _features.Fetch(ref _features.Cache.Request, null);  // TODO study Fetch logic

		public override string Method
		{
			get { return HttpRequestFeature.Method; }
			set { HttpRequestFeature.Method = value; }
		}
		public override string Scheme
		{
			get { return HttpRequestFeature.Scheme; }
			set { HttpRequestFeature.Scheme = value; }
		}
		public override bool IsHttps
		{
			get { return string.Equals(Constants.Https, Scheme, StringComparison.OrdinalIgnoreCase); }
			set { Scheme = value ? Constants.Https : Constants.Http; }
		}

		public override IHeaderDictionary Headers => HttpRequestFeature.Headers;

		public override HostString Host
		{
			get { return HostString.FromUriComponent(Headers[HeaderNames.Host]); }
			set { Headers[HeaderNames.Host] = value.ToUriComponent(); }
		}
		public override PathString PathBase
		{
			get { return new PathString(HttpRequestFeature.PathBase); }
			set { HttpRequestFeature.PathBase = value.Value; }
		}
		public override PathString Path
		{
			get { return new PathString(HttpRequestFeature.Path); }
			set { HttpRequestFeature.Path = value.Value; }
		}

		public override QueryString QueryString
		{
			get { return new QueryString(HttpRequestFeature.QueryString); }
			set { HttpRequestFeature.QueryString = value.Value; }
		}

		private IQueryFeature QueryFeature => _features.Fetch(ref _features.Cache.Query, f => new QueryFeature(f));
		public override IQueryCollection Query
		{
			get { return QueryFeature.Query; }
			set { QueryFeature.Query = value; }
		}
		public override string Protocol
		{
			get { return HttpRequestFeature.Protocol; }
			set { HttpRequestFeature.Protocol = value; }
		}

		public override long? ContentLength
		{
			get { return Headers.ContentLength; }
			set { Headers.ContentLength = value; }
		}
		public override string ContentType
		{
			get { return Headers[HeaderNames.ContentType]; }
			set { Headers[HeaderNames.ContentType] = value; }
		}

		public override Stream Body
		{
			get { return HttpRequestFeature.Body; }
			set { HttpRequestFeature.Body = value; }
		}

		private IFormFeature FormFeature => _features.Fetch(ref _features.Cache.Form, null);
		public override bool HasFormContentType => FormFeature.HasFormContentType;
		public override IFormCollection Form
		{
			get { return FormFeature.ReadForm(); }
			set { FormFeature.Form = value; }
		}
		public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = new CancellationToken())
		{
			return FormFeature.ReadFormAsync(cancellationToken);
		}
		public DefaultHttpRequest(HttpContext context)
		{
			// ReSharper disable once VirtualMemberCallInConstructor
			Initialize(context);
		}

		private HttpContext _context;
		private FeatureReferences<FeatureInterfaces> _features;
		public virtual void Initialize(HttpContext context)
		{
			_context = context;
			_features = new FeatureReferences<FeatureInterfaces>(context.Features);
		}

		public virtual void Uninitialize()
		{
			_context = null;
			_features = default(FeatureReferences<FeatureInterfaces>);
		}

		private struct FeatureInterfaces
		{
			public IHttpRequestFeature Request;
			public IQueryFeature Query; // IHttpRequestFeature 中只有 QueryString 没有 QueryCollection, 但是 HttpRequest 中有啊
			public IFormFeature Form;
			//public IRequestCookiesFeature Cookies;	TODO cookie to imp
		}
	}
}
