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
	/// <summary>
	/// 自定义服务器的Http请求特性
	/// </summary>
	/// <remarks>
	/// 服务器的请求只有转换成Asp.Net Core能够识别的IHttpResponseFeature, Asp.Net Core才能将请求转换成自己内部的HTTP请求
	/// </remarks>
	internal class SinxHttpRequest : IHttpRequestFeature
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

		public SinxHttpRequest()
		{
			this.Body = Stream.Null;
			this.Headers = new HeaderDictionary();
			this.Method = "GET";
			this.Path = "";
			this.PathBase = "";
			this.Protocol = "HTTP/1.1";
			this.QueryString = "";
			this.Scheme = "http";
		}
	}
}
