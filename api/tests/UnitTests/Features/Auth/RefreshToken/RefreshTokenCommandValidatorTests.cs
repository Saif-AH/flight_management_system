using FlightManagementSystem.Application.Features.Auth.Commands.RefreshToken;
using FluentAssertions;

namespace FlightManagementSystem.UnitTests.Features.Auth.RefreshToken;

public sealed class RefreshTokenCommandValidatorTests
{
    private readonly RefreshTokenCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenRefreshTokenIsEmpty()
    {
        var command = new RefreshTokenCommand(string.Empty);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "RefreshToken");
    }

    [Fact]
    public void Validate_ShouldPass_ForValidCommand()
    {
        var command = new RefreshTokenCommand("refresh-token");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
