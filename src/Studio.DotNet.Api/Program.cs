using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
			if (!HttpListener.IsSupported)
			{
				Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
				return;
			}
			// URI prefixes are required,
			// for example "http://contoso.com:8080/index/".
			var prefixes = new[] { "http://localhost:55555/" };
			if (prefixes == null || prefixes.Length == 0)
				throw new ArgumentException("prefixes");
			// Create a listener.
			var listener = new HttpListener();
			// Add the prefixes.
			foreach (var s in prefixes)
			{
				listener.Prefixes.Add(s);
			}
			listener.Start();
			listener.Prefixes.ToList().ForEach(prefiex => Console.WriteLine($"Start Listene At {prefiex} ..."));

			while (true)
			{
				var context = listener.GetContext();
				var pipeline = new Pipeline<HttpContext>();
				pipeline.Add(next => ctx =>
				{
					ctx.Response.StatusCode = 201;
					return next(ctx);
				}).Add(next => ctx =>
				{
					var strings = "<body><h1>HelloWorld</h1></body>".ToCharArray();
					var buffer = Encoding.UTF8.GetBytes(strings, 0, strings.Length);
					ctx.Response.Body = new MemoryStream(buffer);
					return next(ctx);
				});
				var sinxHttpContext = new SinxHttpContext(context);
				var featureCollection = new FeatureCollection();
				featureCollection.Set(sinxHttpContext.HttpRequest);
				featureCollection.Set(sinxHttpContext.HttpResponse);
				var httpContext = new HttpContextFactory().Create(featureCollection);
				pipeline.ProcessAsync(httpContext);
				context = sinxHttpContext.ToHttpListenerContext();
				context.Response.Close();
			}
			listener.Stop();
		}
	}
}
