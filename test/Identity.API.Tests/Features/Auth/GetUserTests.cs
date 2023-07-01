using Common;
using Identity.API.Entities;
using Identity.API.Features;
using Identity.API.Helpers;
using Identity.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Tests.Features.Auth;

public class GetUserTests : IDisposable
{
    private readonly GetUser.Handler _handler;
    private readonly UserDbContext _userDbContext;

    public GetUserTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
        _userDbContext = new UserDbContext(options);
        _handler = new GetUser.Handler(_userDbContext);
    }

    public void Dispose()
    {
        _userDbContext.Database.EnsureDeleted();
        _userDbContext.Dispose();
    }

    [Fact]
    public async Task GetUser_ExistingUserId_ReturnsSuccessResultWithUserInfo()
    {
        // Arrange
        const string email = "test@example.com";
        const string userName = "test_user";
        const string password = "Password1@";
        const Role role = Role.Freelancer;

        var (passwordHash, passwordSalt) = PasswordManager.CreatePasswordHash(password);
        var user = new User(userName, email, passwordHash, passwordSalt, new List<UserClaim>
        {
            new(ClaimTypes.Role, role)
        });

        _userDbContext.Users.Add(user);
        await _userDbContext.SaveChangesAsync();

        var query = new GetUser.Query(user.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.UserId.Should().Be(user.Id);
        result.Value.Email.Should().Be(email);
        result.Value.UserName.Should().Be(userName);
        result.Value.Role.Should().Be(role);
    }

    [Fact]
    public async Task GetUser_InvalidId_ReturnsFailureResult()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        var query = new GetUser.Query(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Errors.Should().NotBeNull();
        result.Errors.Should().Contain(DomainErrors.GetUser.UserDoesNotExist);
    }
}