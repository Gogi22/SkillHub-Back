using Common.Options;
using Identity.API.Tests.TestingHelpers;
using IdentityServer.Helpers;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Identity.API.Tests.Helpers;

public class PasswordManagerTests
{
    private readonly JwtSettings _jwtSettings;

    public PasswordManagerTests(ITestOutputHelper testOutputHelper)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath("/Users/mamutgog/RiderProjects/SkillHub-Back/src/Identity.API")
            .AddJsonFile("appsettings.json")
            .Build();
        _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
    }

    [Fact]
    public void CreatePasswordHash_ValidPassword_ReturnsNonEmptyHashAndSalt()
    {
        // Arrange
        const string password = "P@ssw0rd";

        // Act
        var (passwordHash, passwordSalt) = PasswordManager.CreatePasswordHash(password);
        passwordHash.Should().NotBeNull();
        passwordHash.Should().NotBeEmpty();
        passwordSalt.Should().NotBeNull();
        passwordSalt.Should().NotBeEmpty();
    }

    [Fact]
    public void IsValidPassword_ValidPassword_ReturnsTrue()
    {
        // Arrange
        const string password = "P@ssw0rd";
        var (passwordHash, passwordSalt) = PasswordManager.CreatePasswordHash(password);

        // Act
        var isValid = PasswordManager.IsValidPassword(password, passwordHash, passwordSalt);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsValidPassword_InvalidPassword_ReturnsFalse()
    {
        // Arrange
        const string password = "P@ssw0rd";
        var (passwordHash, passwordSalt) = PasswordManager.CreatePasswordHash(password);
        const string invalidPassword = "InvalidPassword";

        // Act
        var isValid = PasswordManager.IsValidPassword(invalidPassword, passwordHash, passwordSalt);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void GenerateToken_ReturnsNonEmptyToken()
    {
        // Arrange
        const string userName = "test_user";
        var claims = new List<Claim> { new(ClaimTypes.Role, "Freelancer") };

        // Act
        var token = PasswordManager.GenerateToken(userName, claims, _jwtSettings);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        // Verify claims
        var tokenClaims = token.ToClaims();
        tokenClaims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Freelancer");
    }
}