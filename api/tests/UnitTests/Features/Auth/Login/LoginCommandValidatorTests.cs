using FlightManagementSystem.Application.Features.Auth.Commands.Login;
using FluentAssertions;

namespace FlightManagementSystem.UnitTests.Features.Auth.Login;

public sealed class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsInvalid()
    {
        var command = new LoginCommand("not-an-email", "Password123!");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Email");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordIsEmpty()
    {
        var command = new LoginCommand("user@flight.com", string.Empty);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Password");
    }

    [Fact]
    public void Validate_ShouldPass_ForValidCommand()
    {
        var command = new LoginCommand("user@flight.com", "Password123!");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
