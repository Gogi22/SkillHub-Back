namespace SkillHub.API.Entities;

public abstract class BaseUser : BaseEntity<string>
{
    protected BaseUser()
    {
    }

    protected BaseUser(string id, string userName, string email)
    {
        Id = id;
        UserName = userName;
        Email = email;
    }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
}