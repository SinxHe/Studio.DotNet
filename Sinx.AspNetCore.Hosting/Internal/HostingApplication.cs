// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Sinx.AspNetCore.Hosting.Server;
using Sinx.AspNetCore.Http;
using Sinx.AspNetCore.Http.Features;

namespace Sinx.AspNetCore.Hosting.Internal
{
	public class HostingApplication : IHttpApplication<HostingApplication.Context>
	{
		private readonly RequestDelegate _application;
		private readonly IHttpContextFactory _httpContextFactory;

		public HostingApplication(
			RequestDelegate application,
			IHttpContextFactory httpContextFactory)
		{
			_application = application;
			_httpContextFactory = httpContextFactory;
		}

		// Set up the request
		public Context CreateContext(IFeatureCollection contextFeatures)
		{
			var httpContext = _httpContextFactory.Create(contextFeatures);

			// Create and return the request Context
			return new Context
			{
				HttpContext = httpContext,
				//Scope = scope,
				//StartTimestamp = startTimestamp,
			};
		}

		// Execute the request
		public Task ProcessRequestAsync(Context context)
		{
			return _application(context.HttpContext);
		}

		public struct Context
		{
			public HttpContext HttpContext { get; set; }
			public IDisposable Scope { get; set; }
			public long StartTimestamp { get; set; }
		}
	}
}
