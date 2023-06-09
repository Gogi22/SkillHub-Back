using Identity.API.Features.Auth;

namespace Identity.API.Tests.Features.Auth;

public class LoginValidatorTests
{
    private readonly Login.Validator _validator;

    public LoginValidatorTests()
    {
        _validator = new Login.Validator();
    }

    [Fact]
    public void Validator_UserNameNotEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new Login.Command
        {
            UserName = "", // Empty value
            Password = "password123"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "UserName");
    }

    [Fact]
    public void Validator_PasswordNotEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new Login.Command
        {
            UserName = "username",
            Password = "" // Empty value
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Password");
    }

    [Fact]
    public void Validator_ValidCommand_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new Login.Command
        {
            UserName = "username",
            Password = "password123"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}