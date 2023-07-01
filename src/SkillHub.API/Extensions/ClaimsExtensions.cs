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
}