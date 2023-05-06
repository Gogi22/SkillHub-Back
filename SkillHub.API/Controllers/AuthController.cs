using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace SkillHub.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private readonly UserRepository _userRepository;
    private readonly string _token;
    
    public AuthController(IConfiguration configuration, UserRepository userRepository)
    {
        _userRepository = userRepository;
        _token = configuration["JwtSettings:Key"];
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserDto model)
    {
        CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User
        {
            Username = model.Email,
            Email = model.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };
            
        await _userRepository.CreateUser(user);
        
        return Ok();
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserDto model)
    {
        var user = await _userRepository.GetByEmail(model.Email);
        if (user is null || user.Email != model.Email || !VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt))
        {
            return BadRequest("Email or Password is incorrect");
            
        }
        var token = GenerateToken(user);
        return Ok(token);
    }

    private string GenerateToken([FromBody] User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_token));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA256();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }

    private static bool VerifyPasswordHash(string password, IEnumerable<byte> passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA256(passwordSalt);
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }
}