using IdentityServer.Features.Auth;

namespace Identity.API.Tests.Features.Auth;

public class RegisterValidatorTests
{
    private readonly Register.Validator _validator;

    public RegisterValidatorTests()
    {
        _validator = new Register.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_ReturnsTrue()
    {
        // Arrange
        var command = new Register.Command
        {
            Email = "test@example.com",
            Password = "Password1@",
            UserName = "testUser",
            Role = "Freelancer"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Password1@", "test_user", "Freelancer")]
    [InlineData("test@example.com", "", "test_user", "Freelancer")]
    [InlineData("test@example.com", "Password1@", "", "Freelancer")]
    [InlineData("test@example.com", "Password1@", "test_user", "")]
    [InlineData("test@example.com", "password1@", "test_user", "Freelancer")] // Password does not meet requirements
    [InlineData("test@example.com", "Password1", "test_user", "Freelancer")] // Password does not meet requirements
    [InlineData("test@example.com", "Password1@", "test",
        "Freelancer")] // Username does not meet minimum length requirement
    [InlineData("test@example.com", "Password1@", "test_user", "invalid_role")] // Invalid role
    public void Validate_InvalidCommand_ReturnsFalse(string email, string password, string userName, string role)
    {
        // Arrange
        var command = new Register.Command
        {
            Email = email,
            Password = password,
            UserName = userName,
            Role = role
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}