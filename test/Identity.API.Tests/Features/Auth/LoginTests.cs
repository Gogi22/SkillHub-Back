using Common;
using Common.Options;
using Identity.API.Tests.TestingHelpers;
using IdentityServer.Entities;
using IdentityServer.Features.Auth;
using IdentityServer.Helpers;
using IdentityServer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Identity.API.Tests.Features.Auth;

public class LoginTests : IDisposable
{
    private readonly Login.Handler _handler;
    private readonly UserDbContext _userDbContext;

    public LoginTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath("/Users/mamutgog/RiderProjects/SkillHub-Back/src/Identity.API")
            .AddJsonFile("appsettings.json")
            .Build();
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _userDbContext = new UserDbContext(options);
        _handler = new Login.Handler(_userDbContext, jwtSettings);
    }

    public void Dispose()
    {
        _userDbContext.Database.EnsureDeleted();
        _userDbContext.Dispose();
    }

    [Fact]
    public async Task Login_ValidCredentialsWithUsername_ReturnsSuccessResultWithUserInfo()
    {
        // Arrange
        const string userName = "gogi1@";
        const string email = "gogi@gmail.com";
        const string password = "password1@";
        const string role = "Freelancer";

        var (passwordHash, passwordSalt) = PasswordManager.CreatePasswordHash(password);
        var user1 = new User(userName, email, passwordHash, passwordSalt, new List<UserClaim>
        {
            new(ClaimTypes.Role, role)
        });

        _userDbContext.Users.Add(user1);
        await _userDbContext.SaveChangesAsync();
        var userId = user1.Id;

        var command = new Login.Command
        {
            UserName = userName,
            Password = password
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value?.UserName.Should().Be(userName);
        result.Value?.Role.Should().Be(role);
        result.Value?.Token.Should().NotBeNull();

        var claims = result.Value?.Token.ToClaims();
        claims.Should().NotBeNull();

        claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Freelancer");
        claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId);
    }

    [Fact]
    public async Task Login_ValidCredentialsWithEmail_ReturnsSuccessResultWithUserInfo()
    {
        // Arrange
        const string userName = "gogi1@";
        const string email = "gogi@gmail.com";
        const string password = "password1@";
        const string role = "Freelancer";

        var (passwordHash, passwordSalt) = PasswordManager.CreatePasswordHash(password);
        var user1 = new User(userName, email, passwordHash, passwordSalt, new List<UserClaim>
        {
            new(ClaimTypes.Role, role)
        });

        _userDbContext.Users.Add(user1);
        await _userDbContext.SaveChangesAsync();
        var userId = user1.Id;

        var command = new Login.Command
        {
            UserName = email,
            Password = password
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value?.UserName.Should().Be(userName);
        result.Value?.Role.Should().Be(role);
        result.Value?.Token.Should().NotBeNull();

        var claims = result.Value?.Token.ToClaims();
        claims.Should().NotBeNull();

        claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Freelancer");
        claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsFailureResultWithErrorMessage()
    {
        // Arrange
        const string userName = "gogi1@";
        const string email = "gogi@gmail.com";
        const string password = "password1@";
        const string role = "Freelancer";

        var (passwordHash, passwordSalt) = PasswordManager.CreatePasswordHash(password);

        var user = new User(userName, email, passwordHash, passwordSalt,
            new List<UserClaim> { new(ClaimTypes.Role, role) });
        _userDbContext.Users.Add(user);
        await _userDbContext.SaveChangesAsync();

        var command = new Login.Command
        {
            UserName = userName,
            Password = "invalidPassword"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeNull();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Should().Be(DomainErrors.Login.UserDoesNotExist);
        result.Value.Should().BeNull();
    }
}