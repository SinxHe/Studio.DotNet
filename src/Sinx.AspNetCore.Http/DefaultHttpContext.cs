using System;
using System.Collections.Generic;
using System.Text;
using Sinx.AspNetCore.Http.Features;
using Sinx.AspNetCore.Http.Internal;

namespace Sinx.AspNetCore.Http
{
    public class DefaultHttpContext : HttpContext
    {
	    private FeatureReferences<FeatureInterfaces> _features;
	    private HttpRequest _request;
	    private HttpResponse _response;
		public DefaultHttpContext(IFeatureCollection features)
		{
			// ReSharper disable once VirtualMemberCallInConstructor
			Initialize(features);
		}

	    public virtual void Initialize(IFeatureCollection features)
	    {
		    _features = new FeatureReferences<FeatureInterfaces>(features);
		    _request = InitializeHttpRequest();
		    _response = InitializeHttpResponse();
	    }
		protected virtual HttpRequest InitializeHttpRequest() => new DefaultHttpRequest(this);
	    protected virtual HttpResponse InitializeHttpResponse() => new DefaultHttpResponse(this);
		public override IFeatureCollection Features => _features.Collection;
	    public override HttpRequest Request => _request;
		public override HttpResponse Response => _response;
	    public override void Abort()
	    {
		    throw new NotImplementedException();
	    }

	    private struct FeatureInterfaces
	    {
			//public IItems MyProperty { get; set; }
			public IServiceProvidersFeature ServiceProviders { get; set; }
		}
    }
}
