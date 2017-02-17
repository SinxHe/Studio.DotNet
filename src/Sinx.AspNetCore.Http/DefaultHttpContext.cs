using System;
using System.Collections.Generic;
using Sinx.AspNetCore.Http.Features;
using Sinx.AspNetCore.Http.Internal;

namespace Sinx.AspNetCore.Http
{
	public class DefaultHttpContext : HttpContext
	{
		private FeatureReferences<FeatureInterfaces> _features;
		private HttpRequest _request;
		private HttpResponse _response;
		public DefaultHttpContext(IFeatureCollection features)
		{
			// ReSharper disable once VirtualMemberCallInConstructor
			Initialize(features);
		}

		public virtual void Initialize(IFeatureCollection features)
		{
			_features = new FeatureReferences<FeatureInterfaces>(features);
			_request = InitializeHttpRequest();
			_response = InitializeHttpResponse();
		}
		protected virtual HttpRequest InitializeHttpRequest() => new DefaultHttpRequest(this);
		protected virtual HttpResponse InitializeHttpResponse() => new DefaultHttpResponse(this);
		public override IFeatureCollection Features => _features.Collection;
		public override HttpRequest Request => _request;
		public override HttpResponse Response => _response;
		public override void Abort()
		{
			throw new NotImplementedException();
		}

		private IItemsFeature ItemsFeature => _features.Fetch(ref _features.Cache.Items, f => new ItemsFeature());  // TODO _features.Fetch usage

		/// <summary>
		/// 所有中间件共享的 键值对 集合, 包括 IFeatureCollections, IServiceProvider
		/// </summary>
		public override IDictionary<object, object> Items
		{
			get { return ItemsFeature.Items; }
			set { ItemsFeature.Items = value; }
		}

		private IServiceProvidersFeature ServiceProvidersFeature => _features.Fetch(ref _features.Cache.ServiceProviders, f => new ServiceProvidersFeature());
		public override IServiceProvider RequestServices
		{
			get { return ServiceProvidersFeature.RequestServices; }
			set { ServiceProvidersFeature.RequestServices = value; }
		}

		private struct FeatureInterfaces
		{
			public IItemsFeature Items;
			public IServiceProvidersFeature ServiceProviders;
		}
	}
}
