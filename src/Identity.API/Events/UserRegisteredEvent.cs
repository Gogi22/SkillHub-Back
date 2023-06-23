using System.Security.Claims;
using Common;
using IdentityServer.Entities;

namespace IdentityServer.Events;

public class UserRegisteredEvent : IIntegrationEvent
{
    public UserRegisteredEvent(User user)
    {
        EventId = Guid.NewGuid();
        UserId = user.Id;
        Email = user.Email;
        UserName = user.UserName;
        Role = user.Claims.FirstOrDefault(x => x.ClaimType == ClaimTypes.Role)?.ClaimValue ??
               throw new ArgumentNullException(nameof(ClaimTypes.Role));
    }

    public Guid EventId { get; }
    public string UserId { get; }
    public string Email { get; }
    public string UserName { get; }
    public string Role { get; }
}