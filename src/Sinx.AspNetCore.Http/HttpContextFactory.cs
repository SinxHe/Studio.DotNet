using System;
using System.Collections.Generic;
using System.Text;
using Sinx.AspNetCore.Http.Features;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace Sinx.AspNetCore.Http
{
    public class HttpContextFactory : IHttpContextFactory
    {
		public HttpContextFactory(/* TODO ObjectPoolProvider poolProvider*//*, IOptions<FormOptions>*/)
		{

		}

	    public HttpContext Create(IFeatureCollection featureCollection)
	    {
			if (featureCollection == null)
			{
				throw new ArgumentNullException(nameof(featureCollection));
			}
		    var httpContext = new DefaultHttpContext(featureCollection);
		    var formFeature = new FormFeature(httpContext.Request);
		    featureCollection.Set<IFormFeature>(formFeature);
			return httpContext;
	    }

	    public void Dispose(HttpContext httpContext)
	    {
		    throw new NotImplementedException();
	    }
    }
}
