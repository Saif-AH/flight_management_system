using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace FlightManagementSystem.Infrastructure.Auth;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(string UserId, string UserName, string Email, IList<string> Roles)> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials");

        var valid = await _userManager.CheckPasswordAsync(user, password);

        if (!valid)
            throw new UnauthorizedAccessException("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);

        return (user.Id, user.UserName ?? user.Email!, user.Email!, roles);
    }

    public async Task<(string UserId, string UserName, string Email, IList<string> Roles)> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            throw new UnauthorizedAccessException("User not found.");

        var roles = await _userManager.GetRolesAsync(user);

        return (user.Id, user.UserName ?? user.Email!, user.Email!, roles);

    }
}
