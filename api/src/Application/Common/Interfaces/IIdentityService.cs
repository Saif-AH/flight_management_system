namespace FlightManagementSystem.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(string UserId, string UserName, string Email, IList<string> Roles)> LoginAsync(string email, string password);
    Task<(string UserId, string UserName, string Email, IList<string> Roles)> GetUserByIdAsync(string userId);
}
