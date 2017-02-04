using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sinx.AspNetCore.Http;

namespace Studio.DotNet.Api.Controllers
{
    public class HomeController : Controller
    {
	    public void Index()
	    {
		    HttpContext.Response.StatusCode = 201;
	    }
    }

	public abstract class Controller
	{
		public HttpContext HttpContext { get; set; }
	}
}
