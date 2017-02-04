using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sinx.AspNetCore.Http;
using Studio.DotNet.Api.Controllers;

namespace Studio.DotNet.Api
{
	/// <summary>
	/// 路由分发
	/// </summary>
    public static class RouteMiddleware
    {
	    public static IPipeline<THttpContext> Add<THttpContext>(IPipeline<THttpContext> pipeline)
			where THttpContext : HttpContext
	    {
		    return pipeline.Add(nextMid => ctx =>
		    {
			    var match = Regex.Match(ctx.Request.Path.Value, @"/(?<ctl>\w+?)/(?<act>.+?)$", RegexOptions.IgnoreCase);
				if (match.Success)
				{
					var routeData = new RouteData
					{
						Controller = match.Result("${ctl}"),
						Action = match.Result("${act}")
					};
					ctx.Features[typeof(RouteData)] = routeData;
					var asses = typeof(HomeController).Assembly;
					var controller =
						Activator.CreateInstance(asses.ExportedTypes.FirstOrDefault(type => type.Name.ToLower() == "homecontroller"));
					var action =
						controller?.GetType()
							.GetTypeInfo()
							.DeclaredMethods.FirstOrDefault(m => string.Equals(m.Name, ctx.Features.Get<RouteData>().Action, StringComparison.CurrentCultureIgnoreCase));
					var httpContextType = controller?.GetType().GetProperty("HttpContext");
					httpContextType?.SetValue(controller,ctx);
					action?.Invoke(controller,null);
					return nextMid(ctx);
				}
				else
				{
					ctx.Response.StatusCode = 404;
					//ctx.Response.OutputStream.Close();
					//ctx.Response.Close();
					return Task.FromResult(ctx);
				}
		    });
	    }
    }

	public class RouteData
	{
		public string Controller { get; set; }
		public string Action { get; set; }
	}
}
