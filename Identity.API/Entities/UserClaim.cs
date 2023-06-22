namespace IdentityServer.Entities;

public class UserClaim : BaseEntity<int>
{
    private UserClaim()
    {
    }

    public UserClaim(string claimType, string claimValue)
    {
        ClaimType = claimType ?? throw new ArgumentNullException(nameof(claimType));
        ClaimValue = claimValue ?? throw new ArgumentNullException(nameof(claimValue));
    }

    public string UserId { get; set; } = null!;
    public string ClaimType { get; set; } = null!;
    public string ClaimValue { get; set; } = null!;
}