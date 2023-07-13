namespace SkillHub.API.Common;

public class User
{
    public User(string id, Role role)
    {
        Id = id;
        Role = role;
    }

    public string Id { get; }
    public Role Role { get; }
}