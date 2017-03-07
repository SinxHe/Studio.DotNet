using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sinx.AspNetCore.Builder;
using Sinx.AspNetCore.Http.Features;
using Sinx.AspNetCore.Http.Internal;

namespace Sinx.AspNetCore.Http
{
	/// <summary>
	/// 定义一个类以提供对应用程序管道进行配置的机制
	/// </summary>
	public class ApplicationBuilder : IApplicationBuilder
	{
		private readonly IList<Func<RequestDelegate, RequestDelegate>> _components =
			new List<Func<RequestDelegate, RequestDelegate>>();

		/// <summary>
		/// 应用程序管道构造器
		/// </summary>
		/// <param name="sp"></param>
		/// <remarks>
		/// 对应没有 FeatureCollection 传入的情况的ApplicationBuilder构造
		/// </remarks>
		public ApplicationBuilder(IServiceProvider sp)
		{
			ApplicationServices = sp;
			Properties = new Dictionary<string, object>();
		}

		/// <summary>
		/// 应用程序管道构造器
		/// </summary>
		/// <param name="sp"></param>
		/// <param name="server">带有 FeatureCollection</param>
		public ApplicationBuilder(IServiceProvider sp, object server)
			: this(sp)
		{
			SetProperty(Constants.BuilderProperties.ServerFeatures, server);
		}

		private ApplicationBuilder(IApplicationBuilder ab)
		{
			Properties = ab.Properties;
		}

		public IServiceProvider ApplicationServices { get; set; }
		public IFeatureCollection ServerFeatures => GetProperty<IFeatureCollection>(Constants.BuilderProperties.ServerFeatures);

		/// <summary>
		/// Gets a key/value collection that can be used to share data between middleware.
		/// </summary>
		/// <remarks>
		/// 在中间件中共享的键值对, 当然是在火车头上初始化
		/// </remarks>
		public IDictionary<string, object> Properties { get; }

		public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
		{
			_components.Add(middleware);
			return this;
		}

		public IApplicationBuilder New()
		{
			return new ApplicationBuilder(this);
		}

		public RequestDelegate Build()
		{
			return _components.Reverse().Aggregate((RequestDelegate)(ctx => Task.CompletedTask), (c, i) => i(c));
		}

		private TValue GetProperty<TValue>(string key)
		{
			return Properties.TryGetValue(key, out var value) ? (TValue)value : default(TValue);
		}

		private void SetProperty<TValue>(string key, TValue value)
		{
			Properties[key] = value;
		}
	}
}
