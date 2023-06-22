using Microsoft.AspNetCore.Identity;

namespace Application.Entities;

public class User : IdentityUser<string>
{
    public string PasswordSalt { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public Role Role { get; init; }
}