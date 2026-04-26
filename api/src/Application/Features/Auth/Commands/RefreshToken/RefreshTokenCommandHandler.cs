using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Application.Common.Settings;
using FlightManagementSystem.Application.Features.Auth.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FlightManagementSystem.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public RefreshTokenCommandHandler(
        IIdentityService identityService,
        IApplicationDbContext context,
        IJwtTokenService jwtTokenService,
        IOptions<JwtSettings> jwtOptions)
    {
        _identityService = identityService;
        _context = context;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existingRefreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);

        if (existingRefreshToken is null)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        if (!existingRefreshToken.IsActive)
            throw new UnauthorizedAccessException("Refresh token is expired or revoked.");

        var user = await _identityService.GetUserByIdAsync(existingRefreshToken.UserId);

        var accessTokenResult = await _jwtTokenService.GenerateAccessTokenAsync(
            user.UserId,
            user.Email,
            user.Roles);

        var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        existingRefreshToken.IsRevoked = true;
        existingRefreshToken.RevokedAtUtc = DateTime.UtcNow;
        existingRefreshToken.ReplacedByToken = newRefreshTokenValue;

        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            Token = newRefreshTokenValue,
            UserId = user.UserId,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays)
        };

        _context.RefreshTokens.Add(newRefreshToken);

        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Roles = user.Roles.ToArray(),
            AccessToken = accessTokenResult.AccessToken,
            AccessTokenExpiresAtUtc = accessTokenResult.ExpiresAtUtc,
            RefreshToken = newRefreshTokenValue
        };
    }
}
