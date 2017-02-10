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

			//while (true)
			//{
			//	var context = listener.GetContext();
			//	var pipeline = new Pipeline<SinxHttpContext>();
			//	RouteMiddleware.Add(pipeline); 
			//	var sinxHttpContext = new SinxHttpContext(context);
			//	pipeline.ProcessAsync(sinxHttpContext);
			//	(sinxHttpContext.Response)?.ToHttpListenerResponse(context.Response);
			//	context.Response.Close();
			//}

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

	public class SinxHttpContext : HttpContext
	{
		public SinxHttpContext(HttpListenerContext lc)
		{
			Features = new FeatureCollection();
			Request = new SinxHttpRequest(lc.Request);
		}

		public override IFeatureCollection Features { get; }
		public override HttpRequest Request { get; }
		public override HttpResponse Response { get; }
	}

	public class SinxHttpRequest : HttpRequest
	{
		public SinxHttpRequest(HttpListenerRequest lr)
		{
			this.Method = lr.HttpMethod;
			this.Path = new PathString(lr.Url.AbsolutePath);
		}
		public override HttpContext HttpContext { get; }
		public override string Method { get; set; }
		public override PathString Path { get; set; }
		public override string Scheme { get; set; }
		public override bool IsHttps { get; set; }
		public override HostString Host { get; set; }
		public override PathString PathBase { get; set; }
		public override QueryString QueryString { get; set; }
		public override IQueryCollection Query { get; set; }
		public override string Protocol { get; set; }
		public override IHeaderDictionary Headers { get; }
		public override long? ContentLength { get; set; }
		public override string ContentType { get; set; }
		public override Stream Body { get; set; }
		public override bool HasFormContentType { get; }
		public override IFormCollection Form { get; set; }
		public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = new CancellationToken())
		{
			throw new NotImplementedException();
		}
	}
}
