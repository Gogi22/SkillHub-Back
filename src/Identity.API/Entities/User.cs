namespace Identity.API.Entities;

public class User : BaseAggregateRoot<string>
{
    public User(string userName, string email, byte[] passwordHash, byte[] passwordSalt, List<UserClaim> claims)
    {
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        PasswordSalt = passwordSalt ?? throw new ArgumentNullException(nameof(passwordSalt));
        Claims = claims ?? throw new ArgumentNullException(nameof(claims));
    }

    private User()
    {
    }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public List<UserClaim> Claims { get; } = null!;
}