using System.Collections.Generic;
using Sinx.AspNetCore.Http.Features;

namespace Sinx.AspNetCore.Hosting
{
    public class ServerAddressesFeature : IServerAddressesFeature
    {
	    public ICollection<string> Addresses { get; } = new List<string>();
    }
}
