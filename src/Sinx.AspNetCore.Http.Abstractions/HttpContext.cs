// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNetCore.Http.HttpContext
// Assembly: Microsoft.AspNetCore.Http.Abstractions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
// MVID: D6E0EB3B-0C91-4B13-877C-35DC6F5A4C6D
// Assembly location: C:\Users\HeabKing\.nuget\packages\Microsoft.AspNetCore.Http.Abstractions\1.0.0\lib\netstandard1.3\Microsoft.AspNetCore.Http.Abstractions.dll

//using Microsoft.AspNetCore.Http.Authentication;
//using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Net;
//using System.Security.Claims;
using System.Threading;
using Sinx.AspNetCore.Http.Features;

// ReSharper disable once CheckNamespace
namespace Sinx.AspNetCore.Http
{
	/// <summary>
	/// Encapsulates all HTTP-specific information about an individual HTTP request.
	/// </summary>
	public abstract class HttpContext
	{
		/// <summary>
		/// Gets the collection of HTTP features provided by the server and middleware available on this request.
		/// </summary>
		public abstract IFeatureCollection Features { get; }

		/// <summary>
		/// Gets the <see cref="T:Microsoft.AspNetCore.Http.HttpRequest" /> object for this request.
		/// </summary>
		public abstract HttpRequest Request { get; }

		/// <summary>
		/// Gets the <see cref="T:Microsoft.AspNetCore.Http.HttpResponse" /> object for this request.
		/// </summary>
		public abstract HttpResponse Response { get; }

		/// <summary>
		/// Gets information about the underlying connection for this request.
		/// </summary>
		//public abstract ConnectionInfo Connection { get; }

		/// <summary>
		/// Gets an object that manages the establishment of WebSocket connections for this request.
		/// </summary>
		//public abstract WebSocketManager WebSockets { get; }

		/// <summary>
		/// Gets an object that facilitates authentication for this request.
		/// </summary>
		//public abstract AuthenticationManager Authentication { get; }

		/// <summary>Gets or sets the the user for this request.</summary>
		//public abstract ClaimsPrincipal User { get; set; }

		/// <summary>
		/// Gets or sets a key/value collection that can be used to share data within the scope of this request.
		/// </summary>
		public abstract IDictionary<object, object> Items { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="T:System.IServiceProvider" /> that provides access to the request's service container.
		/// </summary>
		public abstract IServiceProvider RequestServices { get; set; }

		/// <summary>
		/// Notifies when the connection underlying this request is aborted and thus request operations should be
		/// cancelled.
		/// </summary>
		//public abstract CancellationToken RequestAborted { get; set; }

		/// <summary>
		/// Gets or sets a unique identifier to represent this request in trace logs.
		/// </summary>
		//public abstract string TraceIdentifier { get; set; }

		/// <summary>
		/// Gets or sets the object used to manage user session data for this request.
		/// </summary>
		//public abstract ISession Session { get; set; }

		/// <summary>Aborts the connection underlying this request.</summary>
		public abstract void Abort();
	}
}
