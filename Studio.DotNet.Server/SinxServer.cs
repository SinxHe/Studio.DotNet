using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Sinx.AspNetCore.Hosting;
using Sinx.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Sinx.AspNetCore.Hosting.Server.Abstractions;
using Sinx.AspNetCore.Hosting.Server;

#pragma warning disable 1570

namespace Studio.DotNet.Server
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// In: IHttpApplication<TContext>, [对Server的Addresses等配置]
	/// 这里没有把HttpWebListener暴露出去, 因为传入IHttpApplication后所有的逻辑都内部进行处理了
	/// </remarks>
	public class SinxServer : IServer
    {
		#region Private Fields
	    private readonly IServerAddressesFeature _serverAddressesFeature = new ServerAddressesFeature();
	    private readonly ILogger _logger;
	    //private Stack<IDisposable> _disposables;	// TODO not imp
		#endregion

		#region Ctor
		public SinxServer(IEnumerable<string> addresses, ILoggerFactory loggerFactory)
		{
			if (loggerFactory == null)
			{
				throw new ArgumentNullException(nameof(loggerFactory));
			}
			addresses?.ToList().ForEach(a => _serverAddressesFeature.Addresses.Add(a));
			_logger = loggerFactory.CreateLogger(typeof(SinxServer).GetTypeInfo().Namespace);	// TODO: 这里使用Namespace是什么鬼?
		} 
		#endregion

		#region IServer

		public IFeatureCollection Features { get; } = new FeatureCollection();
		public void Start<TContext>(IHttpApplication<TContext> application)
		{
			if (!HttpListener.IsSupported)
			{
				_logger.LogError("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
				return;
			}
			if (!_serverAddressesFeature.Addresses.Any())
			{
				_serverAddressesFeature.Addresses.Add("http://localhost:50000/");
			}
			var listener = new HttpListener();
			foreach (string s in _serverAddressesFeature.Addresses)
			{
				// 1. 必须以 "/" 结尾
				// 2. https://+:8080 监听所有host的指定端口
				listener.Prefixes.Add(s);
			}
			listener.Start();
			listener.Prefixes.ToList().ForEach(prefiex => _logger.LogInformation($"Start Listene At {prefiex} ..."));
			while (true)
			{
				var listenerContext = listener.GetContext();
				var sinxHttpContext = new SinxHttpContext(listenerContext);
				Features.Set(sinxHttpContext.HttpRequest);	// 替换赋值
				Features.Set(sinxHttpContext.HttpResponse);
				var httpContext = application.CreateContext(Features);
				var task = application.ProcessRequestAsync(httpContext);
				task.Wait();
				listenerContext = sinxHttpContext.ToHttpListenerContext();
				listenerContext.Response.Close();
			}
		}
		public void Dispose()
		{
			throw new NotImplementedException();
		} 
		#endregion
	}
}
