using System.Security.Claims;
using IdentityServer.Entities;

namespace IdentityServer.Extensions;

public static class UserClaimExtensions
{
    public static string GetRole(this IEnumerable<UserClaim> claims)
    {
        return claims.FirstOrDefault(x => x.ClaimType == ClaimTypes.Role)?.ClaimValue ??
               throw new ArgumentNullException(nameof(ClaimTypes.Role));
    }
    
    public static string GetRole(this IEnumerable<Claim> claims)
    {
        return claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ??
               throw new ArgumentNullException(nameof(ClaimTypes.Role));
    }
}