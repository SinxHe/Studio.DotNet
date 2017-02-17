using System.IO;
using Sinx.AspNetCore.Http.Features;
using Sinx.Net.Http.Headers;

// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
namespace Sinx.AspNetCore.Http.Internal
{
	public class DefaultHttpResponse : HttpResponse
	{
		private HttpContext _context;
		private FeatureReferences<FeatureInterfaces> _features;
		public DefaultHttpResponse(HttpContext context)
		{
			Initialize(context);
		}

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
		private IHttpResponseFeature HttpResponseFeature => _features.Fetch(ref _features.Cache.Response, f => null);
		public override HttpContext HttpContext => _context;
		public override int StatusCode
		{
			get { return HttpResponseFeature.StatusCode; }
			set { HttpResponseFeature.StatusCode = value; }
		}
		public override IHeaderDictionary Headers => HttpResponseFeature.Headers;
		public override Stream Body
		{
			get { return HttpResponseFeature.Body; }
			set { HttpResponseFeature.Body = value; }
		}
		public override long? ContentLength
		{
			get { return Headers.ContentLength; }
			set { Headers.ContentLength = value; }
		}
		public override string ContentType
		{
			get { return Headers[HeaderNames.ContentType]; }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					Headers.Remove(HeaderNames.ContentType);
				}
				else
				{
					Headers[HeaderNames.ContentType] = value;
				}
			}
		}
		public override bool HasStarted => HttpResponseFeature.HasStarted;
		public override void Redirect(string location, bool permanent)
		{
			HttpResponseFeature.StatusCode = permanent ? 301 : 302;
			Headers[HeaderNames.Location] = location;
		}

		private struct FeatureInterfaces
		{
			public IHttpResponseFeature Response;
			//public IResponseCookieFeature Cookies;
		}
	}
}
