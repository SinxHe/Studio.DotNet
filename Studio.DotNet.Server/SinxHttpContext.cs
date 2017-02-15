using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Primitives;
using Sinx.AspNetCore.Http;
using Sinx.AspNetCore.Http.Features;

namespace Studio.DotNet.Server
{
	public class SinxHttpContext
	{
		public IHttpRequestFeature HttpRequest { get; set; }
		public IHttpResponseFeature HttpResponse { get; set; }
		private HttpListenerContext _httpListenerContext;
		public SinxHttpContext(HttpListenerContext context)
		{
			_httpListenerContext = context;
			HttpRequest = new SinxHttpRequest
			{
				Body = context.Request.InputStream,
				Headers = new HeaderDictionary(context.Request.Headers.AllKeys
					.Select(k => new KeyValuePair<string, StringValues>(k, new StringValues(context.Request.Headers.GetValues(k))))
					.ToDictionary(kv => kv.Key, kv => kv.Value)),
				Method = context.Request.HttpMethod,
				Path = context.Request.Url.AbsolutePath,
				PathBase = string.Empty,
				Protocol = context.Request.ProtocolVersion.ToString(2),
				QueryString = context.Request.QueryString.ToString(),
				RawTarget = string.Empty,	// TODO 
				Scheme = "HTTP"	// TODO
			};
			HttpResponse = new SinxHttpResponse();
		}

		public HttpListenerContext ToHttpListenerContext()
		{
			foreach (var item in HttpResponse.Headers)
			{
				_httpListenerContext.Response.Headers.Add(item.Key, string.Join(";", item.Value));
			}
			var buffer = new byte[HttpResponse.Body.Length];// TODO make sure long -> int
			HttpResponse.Body.Read(buffer, 0, buffer.Length);	
			_httpListenerContext.Response.OutputStream.Write(buffer, 0, buffer.Length);
			_httpListenerContext.Response.StatusCode = HttpResponse.StatusCode;
			return _httpListenerContext;
		}
	}
}
