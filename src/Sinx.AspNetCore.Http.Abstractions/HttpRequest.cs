// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNetCore.Http.HttpRequest
// Assembly: Microsoft.AspNetCore.Http.Abstractions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
// MVID: D6E0EB3B-0C91-4B13-877C-35DC6F5A4C6D
// Assembly location: C:\Users\HeabKing\.nuget\packages\Microsoft.AspNetCore.Http.Abstractions\1.0.0\lib\netstandard1.3\Microsoft.AspNetCore.Http.Abstractions.dll

using System.IO;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Sinx.AspNetCore.Http
{
	/// <summary>
	/// Represents the incoming side of an individual HTTP request.
	/// </summary>
	/// <remarks>
	/// eg: http://localhost:2912/home/index?hello=1&world=1
	///		1. PathBase: {HasValue:false, Value:""}
	///		2. Path: {HasValue:true, Value:"/home/index"}
	///		3. Query: [{"Hello", 1},{"World", 1}]
	///		4. QueryString: "?hello=1&world=1"
	/// </remarks>
	public abstract class HttpRequest
	{
		/// <summary>
		/// Gets the <see cref="P:Microsoft.AspNetCore.Http.HttpRequest.HttpContext" /> this request;
		/// </summary>
		public abstract HttpContext HttpContext { get; }

		/// <summary>Gets or set the HTTP method.</summary>
		/// <returns>The HTTP method.</returns>
		public abstract string Method { get; set; }

		/// <summary>Gets or set the HTTP request scheme.</summary>
		/// <returns>The HTTP request scheme.</returns>
		//public abstract string Scheme { get; set; }

		/// <summary>Returns true if the RequestScheme is https.</summary>
		/// <returns>true if this request is using https; otherwise, false.</returns>
		//public abstract bool IsHttps { get; set; }

		/// <summary>Gets or set the Host header. May include the port.</summary>
		/// <return>The Host header.</return>
		//public abstract HostString Host { get; set; }

		/// <summary>Gets or set the RequestPathBase.</summary>
		/// <returns>The RequestPathBase.</returns>
		//public abstract PathString PathBase { get; set; }

		/// <summary>Gets or set the request path from RequestPath.</summary>
		/// <returns>The request path from RequestPath.</returns>
		public abstract PathString Path { get; set; }

		/// <summary>
		/// Gets or set the raw query string used to create the query collection in Request.Query.
		/// </summary>
		/// <returns>The raw query string.</returns>
		//public abstract QueryString QueryString { get; set; }

		/// <summary>
		/// Gets the query value collection parsed from Request.QueryString.
		/// </summary>
		/// <returns>The query value collection parsed from Request.QueryString.</returns>
		//public abstract IQueryCollection Query { get; set; }

		/// <summary>Gets or set the RequestProtocol.</summary>
		/// <returns>The RequestProtocol.</returns>
		//public abstract string Protocol { get; set; }

		/// <summary>Gets the request headers.</summary>
		/// <returns>The request headers.</returns>
		//public abstract IHeaderDictionary Headers { get; }

		/// <summary>Gets the collection of Cookies for this request.</summary>
		/// <returns>The collection of Cookies for this request.</returns>
		//public abstract IRequestCookieCollection Cookies { get; set; }

		/// <summary>Gets or sets the Content-Length header</summary>
		//public abstract long? ContentLength { get; set; }

		/// <summary>Gets or sets the Content-Type header.</summary>
		/// <returns>The Content-Type header.</returns>
		//public abstract string ContentType { get; set; }

		/// <summary>Gets or set the RequestBody Stream.</summary>
		/// <returns>The RequestBody Stream.</returns>
		//public abstract Stream Body { get; set; }

		/// <summary>Checks the content-type header for form types.</summary>
		//public abstract bool HasFormContentType { get; }

		/// <summary>Gets or sets the request body as a form.</summary>
		//public abstract IFormCollection Form { get; set; }

		/// <summary>Reads the request body if it is a form.</summary>
		/// <returns></returns>
		//public abstract Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = null);
	}
}
