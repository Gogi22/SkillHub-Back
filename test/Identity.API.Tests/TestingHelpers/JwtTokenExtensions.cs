using System.IdentityModel.Tokens.Jwt;

namespace Identity.API.Tests.TestingHelpers;

public static class JwtTokenExtensions
{
    public static List<Claim> ToClaims(this string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var decodedToken = tokenHandler.ReadJwtToken(token);

        return decodedToken.Claims.ToList();
    }
}