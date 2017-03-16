using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sinx.AspNetCore.Builder;
using Sinx.AspNetCore.Hosting.Internal;
using Sinx.AspNetCore.Hosting.Server;
using Sinx.AspNetCore.Hosting.Server.Abstractions;
using Sinx.AspNetCore.Http;
using Sinx.AspNetCore.Http.Features;
using Studio.DotNet.Server;

namespace Studio.DotNet.Api
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// 管道处理
	///		. 静态资源处理 (MVC)
	///		. 路由分发
	/// </remarks>
	public static class Program
	{
		public static void Main(string[] args)
		{
			ILoggerFactory loggerFactory = new LoggerFactory().AddConsole();
			IHttpContextFactory ctxFactory = new HttpContextFactory();
			IServer server = new SinxServer(null, loggerFactory);
			IServiceProvider sp = new SinxServiceProvider();
			IApplicationBuilder appBuilder = new ApplicationBuilder(sp, server);
			appBuilder.Use(next => ctx =>
			{
				ctx.Response.StatusCode = 201;
				return next(ctx);
			});
			RequestDelegate app = appBuilder.Build();
			server.Start(new HostingApplication(app,ctxFactory));
		}
	}
}
