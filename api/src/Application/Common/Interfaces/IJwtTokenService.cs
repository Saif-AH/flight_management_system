namespace FlightManagementSystem.Application.Common.Interfaces;

public interface IJwtTokenService
{
    Task<(string AccessToken, DateTime ExpiresAtUtc)> GenerateAccessTokenAsync(string userId, string email, IList<string> roles);
    string GenerateRefreshToken();
}