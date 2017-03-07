using Sinx.AspNetCore.Http.Features;
using System;

namespace Sinx.AspNetCore.Hosting.Abstractions
{
	/// <summary>
	/// Represents a configured web host.
	/// </summary>
	/// <remarks>
	/// 
	/// </remarks>
	public interface IWebHost : IDisposable
	{
		/// <summary>
		/// The <see cref="IFeatureCollection"/> exposed by the configured server.
		/// </summary>
		IFeatureCollection ServiceFeatures { get; }
		/// <summary>
		/// The <see cref="IServiceProvider"/> for the host.
		/// </summary>
		IServiceProvider Services { get; }
		/// <summary>
		/// Starts listening on the configured addresses.
		/// </summary>
		void Start();
	}
}
