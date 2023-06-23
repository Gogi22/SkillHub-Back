using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Common.Options;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Helpers;

public static class PasswordManager
{
    internal static (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHash(string password)
    {
        using var hmac = new HMACSHA256();
        var passwordSalt = hmac.Key;
        var passwordHash = hmac.ComputeHash(Encoding.Unicode.GetBytes(password));
        return (passwordHash, passwordSalt);
    }

    internal static bool IsValidPassword(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA256(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.Unicode.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }

    internal static string GenerateToken(string userName, List<Claim> claims, JwtSettings jwtSettings)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(jwtSettings.Issuer,
            jwtSettings.Audience,
            claims,
            expires: DateTime.Now.AddHours(10),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}