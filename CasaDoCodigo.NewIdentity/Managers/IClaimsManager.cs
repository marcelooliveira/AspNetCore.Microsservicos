using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.API.Managers
{
    public interface IClaimsManager
    {
        Task AddUpdateClaim(string userId, IDictionary<string, string> claims);
    }
}