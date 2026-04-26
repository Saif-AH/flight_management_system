using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Application.Common.Settings;
using FlightManagementSystem.Application.Features.Auth.Commands.Login;
using FlightManagementSystem.UnitTests.Common;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace FlightManagementSystem.UnitTests.Features.Auth.Login;

public sealed class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnTokensAndUserMetadata_AndPersistRefreshToken()
    {
        await using var context = TestApplicationDbContextFactory.Create();

        var identityService = new Mock<IIdentityService>();
        identityService
            .Setup(x => x.LoginAsync("user@flight.com", "Password123!"))
            .ReturnsAsync(("user-1", "user@flight.com", "user@flight.com", (IList<string>)["User"]));

        var jwtTokenService = new Mock<IJwtTokenService>();
        jwtTokenService
            .Setup(x => x.GenerateAccessTokenAsync("user-1", "user@flight.com", It.IsAny<IList<string>>()))
            .ReturnsAsync(("access-token", DateTime.UtcNow.AddMinutes(30)));
        jwtTokenService
            .Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        var options = Options.Create(new JwtSettings { RefreshTokenDays = 7 });
        var handler = new LoginCommandHandler(identityService.Object, context, jwtTokenService.Object, options);

        var result = await handler.Handle(new LoginCommand("user@flight.com", "Password123!"), CancellationToken.None);

        result.UserId.Should().Be("user-1");
        result.UserName.Should().Be("user@flight.com");
        result.Roles.Should().BeEquivalentTo(["User"]);
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        context.RefreshTokens.Should().ContainSingle(x => x.UserId == "user-1" && x.Token == "refresh-token");
    }
}
