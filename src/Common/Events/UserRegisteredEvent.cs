using System.Security.Claims;

namespace Common.Events;

public class UserRegisteredEvent : IIntegrationEvent
{
    public UserRegisteredEvent(string userId, string email, string userName, Role role)
    {
        EventId = Guid.NewGuid();
        UserId = userId;
        Email = email;
        UserName = userName;
        Role = role;
    }

    public Guid EventId { get; }
    public string UserId { get; }
    public string Email { get; }
    public string UserName { get; }
    public Role Role { get; }
}

// TODO fix wait for rabbit mq when starting SkillHub.API