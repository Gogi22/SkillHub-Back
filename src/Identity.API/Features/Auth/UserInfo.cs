using Common;

namespace IdentityServer.Features.Auth;

public class UserInfo
{
    public UserInfo(string userName, Role role, string token)
    {
        UserName = userName;
        Role = role;
        Token = token;
    }

    public string UserName { get; set; }
    public Role Role { get; set; }
    public string Token { get; set; }
}