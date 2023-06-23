namespace IdentityServer.Features.Auth;

public class UserInfo
{
    public UserInfo(string userName, string role, string token)
    {
        UserName = userName;
        Role = role;
        Token = token;
    }

    public string UserName { get; set; }
    public string Role { get; set; }
    public string Token { get; set; }
}