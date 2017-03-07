using System;
using Sinx.AspNetCore.Http.Features;

namespace Sinx.AspNetCore.Hosting.Server.Abstractions
{
	/// <summary>
	/// Represents a server.
	/// </summary>
    public interface IServer : IDisposable
    {
		/// <summary>
		/// A collection of HTTP features of server.
		/// </summary>
	    IFeatureCollection Features { get; }

	    void Start<TContext>(IHttpApplication<TContext> application);
    }
}
