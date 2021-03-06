// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Sinx.AspNetCore.Http;
using Sinx.AspNetCore.Http.Features;
using Sinx.AspNetCore.Http.Internal;
using Sinx.Extensions.Internal;

// ReSharper disable once CheckNamespace
namespace Sinx.AspNetCore.Builder.Internal
{
	/// <summary>
	/// In: IServiceProvider, [object server]
	/// </summary>
	public class ApplicationBuilder : IApplicationBuilder
	{
		private readonly IList<Func<RequestDelegate, RequestDelegate>> _components = new List<Func<RequestDelegate, RequestDelegate>>();

		#region Ctor
		public ApplicationBuilder(IServiceProvider serviceProvider)
		{
			Properties = new Dictionary<string, object>();
			ApplicationServices = serviceProvider;
		}
		public ApplicationBuilder(IServiceProvider serviceProvider, object server)
			: this(serviceProvider)	// 实测: 这里会先执行 this(serviceProvider)
		{
			SetProperty(Constants.BuilderProperties.ServerFeatures, server);
		}
		/// <summary>
		/// 为内部方法 New() 提供的构造器
		/// </summary>
		/// <param name="builder"></param>
		private ApplicationBuilder(IApplicationBuilder builder)
		{
			Properties = builder.Properties;
		}
		#endregion
		#region IApplictionBuilder
		/// <summary>
		/// 中间件们用来共享数据的键值对集合
		/// </summary>
		/// <remarks>
		/// 值包括:
		///		1. IServiceProvider
		///		2. [object server]
		/// </remarks>
		public IDictionary<string, object> Properties { get; }
		public IServiceProvider ApplicationServices
		{
			get { return GetProperty<IServiceProvider>(Constants.BuilderProperties.ApplicationServices); }
			set { SetProperty(Constants.BuilderProperties.ApplicationServices, value); }
		}
		public IFeatureCollection ServerFeatures => GetProperty<IFeatureCollection>(Constants.BuilderProperties.ServerFeatures);

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
			RequestDelegate app = context =>
			{
				context.Response.StatusCode = 404;
				return TaskCache.CompletedTask;
			};

			return _components.Reverse().Aggregate(app, (current, component) => component(current));
		}
		private T GetProperty<T>(string key)
		{
			return Properties.TryGetValue(key, out object value) ? (T)value : default(T);
		}
		private void SetProperty<T>(string key, T value)
		{
			Properties[key] = value;
		}
		#endregion
	}
}
