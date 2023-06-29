using Common;

namespace IdentityServer.Entities;

public class UserClaim : BaseEntity<int>
{
    private UserClaim()
    {
    }

    public UserClaim(string claimType, Role claimValue)
    {
        ClaimType = claimType ?? throw new ArgumentNullException(nameof(claimType));
        ClaimValue = claimValue;
    }

    public string UserId { get; set; } = null!;
    public string ClaimType { get; set; } = null!;
    public Role ClaimValue { get; set; }
}