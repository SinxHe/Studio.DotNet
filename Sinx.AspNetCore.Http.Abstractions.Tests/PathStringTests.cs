using System;
using Xunit;

namespace Sinx.AspNetCore.Http.Abstractions.Tests
{
    public class PathStringTests
    {
		[Theory]
		[InlineData("/123?hello&world", "/123%3Fhello&world")]
		public void Ctor(string from, string excepted)
	    {
			var path = new PathString(from);
			Assert.Equal(path.ToString(), excepted);
		}

	    [Fact]
		[InlineData("/123?hello", "/123")]
        public void Converte_FromUrlComponet(string url, string excepted)
        {
	        var path = new PathString(url);
	        Assert.Equal(url, excepted);
        }
    }
}
