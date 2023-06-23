using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Identity.API.Tests.TestingHelpers;

public static class JwtTokenExtensions
{
    public static List<Claim> GetClaims(this string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var decodedToken = tokenHandler.ReadJwtToken(token);

        return decodedToken.Claims.ToList();
    }
}