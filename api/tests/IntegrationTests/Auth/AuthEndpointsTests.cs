using FlightManagementSystem.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace FlightManagementSystem.IntegrationTests.Auth;

[Collection(IntegrationTestCollection.Name)]
public sealed class AuthEndpointsTests
{
    private readonly ApiWebApplicationFactory _factory;

    public AuthEndpointsTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_ShouldReturnTokensAndUserMetadata()
    {
        if (!_factory.RequireDocker())
            return;

        await _factory.ResetDatabaseAsync();
        using var client = _factory.CreateClient();

        var auth = await client.LoginAsync("user@flight.com", "User123!");

        auth.AccessToken.Should().NotBeNullOrWhiteSpace();
        auth.RefreshToken.Should().NotBeNullOrWhiteSpace();
        auth.UserId.Should().NotBeNullOrWhiteSpace();
        auth.UserName.Should().Be("user@flight.com");
        auth.Roles.Should().Contain("User");
    }
}
