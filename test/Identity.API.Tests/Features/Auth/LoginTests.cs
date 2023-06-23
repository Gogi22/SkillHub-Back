using System.Security.Claims;
using Common;
using Common.Options;
using FluentAssertions;
using Identity.API.Tests.TestingHelpers;
using IdentityServer.Entities;
using IdentityServer.Features.Auth;
using IdentityServer.Helpers;
using IdentityServer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Identity.API.Tests.Features.Auth;

public class LoginTests
{
    private readonly JwtSettings _jwtSettings;

    public LoginTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath("/Users/mamutgog/RiderProjects/SkillHub-Back/src/Identity.API")
            .AddJsonFile("appsettings.json")
            .Build();

        _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsSuccessResultWithUserInfo()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        const string userName = "gogi1@";
        const string email = "gogi@gmail.com";
        const string password = "password1@";
        const string role = "Freelancer";
        string userId;
        var (passwordHash, passwordSalt) = PasswordManager.CreatePasswordHash(password);
        await using (var context = new UserDbContext(options))
        {
            var user1 = new User(userName, email, passwordHash, passwordSalt, new List<UserClaim>
            {
                new(ClaimTypes.Role, role)
            });

            context.Users.Add(user1);
            await context.SaveChangesAsync();
            userId = user1.Id;
        }

        await using (var context = new UserDbContext(options))
        {
            var command = new Login.Command
            {
                UserName = userName,
                Password = password
            };

            var handler = new Login.Handler(context, _jwtSettings);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value?.UserName.Should().Be(userName);
            result.Value?.Role.Should().Be(role);
            result.Value?.Token.Should().NotBeNull();

            var claims = result.Value?.Token.GetClaims();
            claims.Should().NotBeNull();

            claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Freelancer");
            claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId);

        }
    }

    [Fact]
    public async Task Handle_InvalidCredentials_ReturnsFailureResultWithErrorMessage()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        const string userName = "gogi1@";
        const string email = "gogi@gmail.com";
        const string password = "password1@";
        const string role = "Freelancer";
        var (passwordHash, passwordSalt) = PasswordManager.CreatePasswordHash(password);

        await using (var context = new UserDbContext(options))
        {
            var user = new User(userName, email, passwordHash, passwordSalt,
                new List<UserClaim> { new(ClaimTypes.Role, role) });
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        await using (var context = new UserDbContext(options))
        {
            var command = new Login.Command
            {
                UserName = userName,
                Password = "invalidPassword"
            };

            var handler = new Login.Handler(context, _jwtSettings);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeNull();
            result.Errors.Should().NotBeEmpty();
            result.Errors.First().Should().Be(DomainErrors.Login.UserDoesNotExist);
            result.Value.Should().BeNull();
        }
    }
}