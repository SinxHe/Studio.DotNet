﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading.Tasks;

namespace Sinx.AspNetCore.Http.Features
{
	/// <summary>
	/// Represents the fields and state of an HTTP response.
	/// </summary>
	/// <remarks>
	/// 1. 由 Server 的 HttpResponse 实现;
	/// 2. DefaultHttpResponse 使用的如:StatusCode字段其实跟IHttpResponseFeature指向的同一个托管堆地址, 所以改变DeafultHttpResponse的StatusCode, IHttpResponseFeature也会改变
	///			1. 基于以上原因, 也能发现, 比如: QueryString, 返回的是一个新的 QueryString, 所以里面没有Add方法, 因为在因为新的QeuryString赋值影响不了原来的
	/// </remarks>
	public interface IHttpResponseFeature
	{
		/// <summary>
		/// The status-code as defined in RFC 7230. The default value is 200.
		/// </summary>
		int StatusCode { get; set; }

		/// <summary>
		/// The reason-phrase as defined in RFC 7230. Note this field is no longer supported by HTTP/2.
		/// </summary>
		string ReasonPhrase { get; set; }

		/// <summary>
		/// The response headers to send. Headers with multiple values will be emitted as multiple headers.
		/// </summary>
		IHeaderDictionary Headers { get; set; }

		/// <summary>
		/// The <see cref="Stream"/> for writing the response body.
		/// </summary>
		Stream Body { get; set; }

		/// <summary>
		/// Indicates if the response has started. If true, the <see cref="StatusCode"/>,
		/// <see cref="ReasonPhrase"/>, and <see cref="Headers"/> are now immutable, and
		/// OnStarting should no longer be called.
		/// </summary>
		bool HasStarted { get; }

		/// <summary>
		/// Registers a callback to be invoked just before the response starts. This is the
		/// last chance to modify the <see cref="Headers"/>, <see cref="StatusCode"/>, or
		/// <see cref="ReasonPhrase"/>.
		/// </summary>
		/// <param name="callback">The callback to invoke when starting the response.</param>
		/// <param name="state">The state to pass into the callback.</param>
		void OnStarting(Func<object, Task> callback, object state);

		/// <summary>
		/// Registers a callback to be invoked after a response has fully completed. This is
		/// intended for resource cleanup.
		/// </summary>
		/// <param name="callback">The callback to invoke after the response has completed.</param>
		/// <param name="state">The state to pass into the callback.</param>
		void OnCompleted(Func<object, Task> callback, object state);
	}
}
