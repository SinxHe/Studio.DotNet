﻿using System;
using System.IO;
using System.Threading.Tasks;
using Sinx.AspNetCore.Http;
using Sinx.AspNetCore.Http.Features;

namespace Studio.DotNet.Server
{
	internal class SinxHttpResponse : IHttpResponseFeature
    {
	    public int StatusCode { get; set; }
	    public string ReasonPhrase { get; set; }
	    public IHeaderDictionary Headers { get; set; }
	    public Stream Body { get; set; }
	    public bool HasStarted { get; }
	    public void OnStarting(Func<object, Task> callback, object state)
	    {
		    throw new NotImplementedException();
	    }
	    public void OnCompleted(Func<object, Task> callback, object state)
	    {
		    throw new NotImplementedException();
	    }
		public SinxHttpResponse()
		{
			StatusCode = 200;
			ReasonPhrase = "Ok";
			Headers = new HeaderDictionary();
			Body = Stream.Null;
			HasStarted = false;
		}
    }
}
