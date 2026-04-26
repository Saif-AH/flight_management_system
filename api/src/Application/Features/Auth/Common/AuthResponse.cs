namespace FlightManagementSystem.Application.Features.Auth.Common;

public sealed record LoginRequest(string Email, string Password);

public sealed class AuthResponse
{
    public string UserId { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public IReadOnlyList<string> Roles { get; set; } = [];
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime AccessTokenExpiresAtUtc { get; set; }
}
