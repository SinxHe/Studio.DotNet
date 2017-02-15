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
				});
				var sinxHttpContext = new SinxHttpContext(context);
				var httpContext = new HttpContextFactory().Create();
				pipeline.ProcessAsync();
				//(sinxHttpContext.Response)?.ToHttpListenerResponse(context.Response);
				context.Response.Close();
			}

			//var request = context.Request;
			// Obtain a response object.
			//var response = context.Response;
			// Construct a response.
			//var responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
			//var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
			// Get a response stream and write the response to it.
			//response.ContentLength64 = buffer.Length;
			//var output = response.OutputStream;
			//output.Write(buffer, 0, buffer.Length);
			// You must close the output stream.
			//output.Close();
			listener.Stop();
		}
	}
}
