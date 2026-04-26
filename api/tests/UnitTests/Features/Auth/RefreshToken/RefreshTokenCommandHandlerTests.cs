using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Application.Common.Settings;
using FlightManagementSystem.Application.Features.Auth.Commands.RefreshToken;
using FlightManagementSystem.UnitTests.Common;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace FlightManagementSystem.UnitTests.Features.Auth.RefreshToken;

public sealed class RefreshTokenCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRotateRefreshTokenAndReturnUpdatedUserMetadata()
    {
        await using var context = TestApplicationDbContextFactory.Create();

        context.RefreshTokens.Add(new FlightManagementSystem.Domain.Entities.RefreshToken
        {
            Token = "old-refresh",
            UserId = "user-1",
            ExpiresAtUtc = DateTime.UtcNow.AddDays(2)
        });
        await context.SaveChangesAsync(CancellationToken.None);

        var identityService = new Mock<IIdentityService>();
        identityService
            .Setup(x => x.GetUserByIdAsync("user-1"))
            .ReturnsAsync(("user-1", "user@flight.com", "user@flight.com", (IList<string>)["User"]));

        var jwtTokenService = new Mock<IJwtTokenService>();
        jwtTokenService
            .Setup(x => x.GenerateAccessTokenAsync("user-1", "user@flight.com", It.IsAny<IList<string>>()))
            .ReturnsAsync(("new-access", DateTime.UtcNow.AddMinutes(30)));
        jwtTokenService
            .Setup(x => x.GenerateRefreshToken())
            .Returns("new-refresh");

        var options = Options.Create(new JwtSettings { RefreshTokenDays = 7 });
        var handler = new RefreshTokenCommandHandler(identityService.Object, context, jwtTokenService.Object, options);

        var result = await handler.Handle(new RefreshTokenCommand("old-refresh"), CancellationToken.None);

        result.UserId.Should().Be("user-1");
        result.UserName.Should().Be("user@flight.com");
        result.Roles.Should().BeEquivalentTo(["User"]);
        result.RefreshToken.Should().Be("new-refresh");
        context.RefreshTokens.Should().Contain(x => x.Token == "new-refresh" && x.UserId == "user-1");
        context.RefreshTokens.Single(x => x.Token == "old-refresh").IsRevoked.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenRefreshTokenDoesNotExist()
    {
        await using var context = TestApplicationDbContextFactory.Create();
        var handler = new RefreshTokenCommandHandler(
            Mock.Of<IIdentityService>(),
            context,
            Mock.Of<IJwtTokenService>(),
            Options.Create(new JwtSettings { RefreshTokenDays = 7 }));

        var act = () => handler.Handle(new RefreshTokenCommand("missing"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid refresh token.");
    }
}
