using System;
using Sinx.AspNetCore.Hosting.Abstractions;
using Sinx.AspNetCore.Http.Features;

namespace Sinx.AspNetCore.Hosting.Internal
{
    public class WebHost : IWebHost
    {
	    public IFeatureCollection ServiceFeatures { get; }
	    public IServiceProvider Services { get; }
	    public void Start()
	    {
			// 创建应用程序
		    throw new NotImplementedException();
	    }
		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
