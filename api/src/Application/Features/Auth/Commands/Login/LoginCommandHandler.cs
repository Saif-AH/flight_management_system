using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Application.Common.Settings;
using FlightManagementSystem.Application.Features.Auth.Common;
using MediatR;
using Microsoft.Extensions.Options;

namespace FlightManagementSystem.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public LoginCommandHandler(
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

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.LoginAsync(
            request.Email,
            request.Password);

        var accessTokenResult = await _jwtTokenService.GenerateAccessTokenAsync(
            user.UserId,
            user.Email,
            user.Roles);

        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();
        
        var refreshToken = new Domain.Entities.RefreshToken
        {
            Token = refreshTokenValue,
            UserId = user.UserId,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays)
        };

        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Roles = user.Roles.ToArray(),
            AccessToken = accessTokenResult.AccessToken,
            AccessTokenExpiresAtUtc = accessTokenResult.ExpiresAtUtc,
            RefreshToken = refreshTokenValue
        };
    }
}
