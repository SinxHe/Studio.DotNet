﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Sinx.AspNetCore.Http.Features
{
    public interface IHttpRequestLifetimeFeature
	{
		/// <summary>
		/// A <see cref="CancellationToken"/> that fires if the request is aborted and
		/// the application should cease processing. The token will not fire if the request
		/// completes successfully.
		/// </summary>
		CancellationToken RequestAborted { get; set; }

		/// <summary>
		/// Forcefully aborts the request if it has not already completed. This will result in
		/// RequestAborted being triggered.
		/// </summary>
		void Abort();
	}
}
