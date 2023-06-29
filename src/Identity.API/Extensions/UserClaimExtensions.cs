using System.Security.Claims;
using Common;
using IdentityServer.Entities;

namespace IdentityServer.Extensions;

public static class UserClaimExtensions
{
    public static Role GetRole(this IEnumerable<UserClaim> claims)
    {
         return claims.FirstOrDefault(x => x.ClaimType == ClaimTypes.Role)?.ClaimValue ??
               throw new ArgumentNullException(nameof(ClaimTypes.Role));
    }
    
    public static Role GetRole(this IEnumerable<Claim> claims)
    {
         var roleString = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ??
               throw new ArgumentNullException(nameof(ClaimTypes.Role));

         return roleString.ToRole();
    }

    public static Role ToRole(this string roleString)
    {
        return (Role)Enum.Parse(typeof(Role), roleString);
    }
}