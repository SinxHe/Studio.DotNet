using System.Collections.Generic;

namespace Sinx.AspNetCore.Http.Features
{
    public interface IServerAddressesFeature
    {
	    ICollection<string> Addresses { get; }
    }
}
