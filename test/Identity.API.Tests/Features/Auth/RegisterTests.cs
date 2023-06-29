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

public class RegisterTests : IDisposable
{
    private readonly Register.Handler _handler;
    private readonly UserDbContext _userDbContext;

    public RegisterTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath("/Users/mamutgog/RiderProjects/SkillHub-Back/src/Identity.API")
            .AddJsonFile("appsettings.json")
            .Build();
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

        _userDbContext = new UserDbContext(options);
        _handler = new Register.Handler(_userDbContext, jwtSettings, Mock.Of<IEventProducer>(), configuration);
    }

    public void Dispose()
    {
        _userDbContext.Database.EnsureDeleted();
        _userDbContext.Dispose();
    }

    [Fact]
    public async Task Register_ValidCredentials_ReturnsSuccessResultWithUserInfo()
    {
        // Arrange
        const string email = "test@example.com";
        const string password = "Password1@";
        const string userName = "testUser";
        const Role role = Role.Freelancer;

        var command = new Register.Command
        {
            Email = email,
            Password = password,
            UserName = userName,
            Role = role
        };

        // Act
        var result = await _handler.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.UserName.Should().Be(userName);
        result.Value.Role.Should().Be(role);
        result.Value.Token.Should().NotBeNullOrWhiteSpace();

        var claims = result.Value.Token.ToClaims();
        claims.Should().NotBeNull();
        claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == role.ToString());
        claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier);
    }

    [Fact]
    public async Task Register_DuplicateUser_ReturnsFailureResult()
    {
        // Arrange
        const string email = "test@example.com";
        const string password = "Password1@";
        const string userName = "testuser";
        const Role role = Role.Freelancer;

        var command = new Register.Command
        {
            Email = email,
            Password = password,
            UserName = userName,
            Role = role
        };


        var (passwordHash, passwordSalt) = PasswordManager.CreatePasswordHash(password);
        var existingUser = new User(userName, email, passwordHash, passwordSalt, new List<UserClaim>());
        await _userDbContext.Users.AddAsync(existingUser);
        await _userDbContext.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeNullOrEmpty();
        result.Errors.Should().Contain(DomainErrors.Register.UserAlreadyExists);
        result.Value.Should().BeNull();
    }
}