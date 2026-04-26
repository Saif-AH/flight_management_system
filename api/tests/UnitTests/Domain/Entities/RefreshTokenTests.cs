using FlightManagementSystem.Domain.Entities;
using FluentAssertions;

namespace FlightManagementSystem.UnitTests.Domain.Entities;

public sealed class RefreshTokenTests
{
    [Fact]
    public void IsActive_ShouldReturnTrue_WhenNotRevokedAndNotExpired()
    {
        var token = new RefreshToken
        {
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(5),
            IsRevoked = false
        };

        token.IsActive.Should().BeTrue();
    }

    [Fact]
    public void IsActive_ShouldReturnFalse_WhenRevoked()
    {
        var token = new RefreshToken
        {
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(5),
            IsRevoked = true
        };

        token.IsActive.Should().BeFalse();
    }

    [Fact]
    public void IsActive_ShouldReturnFalse_WhenExpired()
    {
        var token = new RefreshToken
        {
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(-1),
            IsRevoked = false
        };

        token.IsActive.Should().BeFalse();
    }
}
