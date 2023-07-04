namespace SkillHub.API.Entities;

public abstract class BaseUser : BaseEntity<string>
{
    protected BaseUser(string userId, string userName, string email)
    {
        UserId = userId;
        UserName = userName;
        Email = email;
    }

    public string UserId { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
}