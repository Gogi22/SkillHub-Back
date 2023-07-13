namespace SkillHub.API.Models;

public class GetUserResponse
{
    public GetUserResponse(string userId, string email, string userName, Role role)
    {
        UserId = userId;
        Email = email;
        UserName = userName;
        Role = role;
    }

    public string UserId { get; }
    public string Email { get; }
    public string UserName { get; }
    public Role Role { get; }
}