namespace SkillHub.API.Common;

public class FullUserInfo
{
    public FullUserInfo(string id, string email, string userName)
    {
        Id = id;
        Email = email;
        UserName = userName;
    }

    public string Id { get; }
    public string Email { get; }
    public string UserName { get; }
}