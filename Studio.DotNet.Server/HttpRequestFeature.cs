// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNetCore.TestHost.RequestFeature
// Assembly: Microsoft.AspNetCore.TestHost, Version=1.2.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
// MVID: 0F09A107-967B-4EAF-A0E5-58D6C1976B3B
// Assembly location: C:\Users\HeabKing\.nuget\packages\Microsoft.AspNetCore.TestHost\1.2.0-preview1-23271\lib\net451\Microsoft.AspNetCore.TestHost.dll

using System.IO;
using Sinx.AspNetCore.Http;
using Sinx.AspNetCore.Http.Features;

namespace Studio.DotNet.Server
{
	internal class RequestFeature : IHttpRequestFeature
	{
		public Stream Body { get; set; }

		public IHeaderDictionary Headers { get; set; }

		public string Method { get; set; }

		public string Path { get; set; }

		public string PathBase { get; set; }

		public string Protocol { get; set; }

		public string QueryString { get; set; }

		public string Scheme { get; set; }

		public string RawTarget { get; set; }

		public RequestFeature()
		{
			this.Body = Stream.Null;
			this.Headers = (IHeaderDictionary)new HeaderDictionary();
			this.Method = "GET";
			this.Path = "";
			this.PathBase = "";
			this.Protocol = "HTTP/1.1";
			this.QueryString = "";
			this.Scheme = "http";
		}
	}
}
