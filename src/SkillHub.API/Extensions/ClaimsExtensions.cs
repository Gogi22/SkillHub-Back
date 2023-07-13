namespace SkillHub.API.Extensions;

public static class ClaimsExtensions
{
    public static User GetUser(this ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                     ?? throw new ArgumentNullException(ClaimTypes.NameIdentifier);
        var roleString = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
                         ?? throw new ArgumentNullException(ClaimTypes.Role);
        var role = (Role)Enum.Parse(typeof(Role), roleString);
        return new User(userId, role);
    }

    public static FullUserInfo GetUserInfo(this IEnumerable<Claim> claims)
    {
        var claimsList = claims as Claim[] ?? claims.ToArray();
        var userId = claimsList.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                     ?? throw new ArgumentNullException(ClaimTypes.NameIdentifier);
        var userName = claimsList.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                       ?? throw new ArgumentNullException(ClaimTypes.Name);
        var email = claimsList.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                    ?? throw new ArgumentNullException(ClaimTypes.Email);

        return new FullUserInfo(userId, email, userName);
    }
}