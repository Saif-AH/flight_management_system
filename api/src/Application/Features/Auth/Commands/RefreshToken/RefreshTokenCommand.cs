using FlightManagementSystem.Application.Features.Auth.Common;
using MediatR;

namespace FlightManagementSystem.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(
    string RefreshToken) : IRequest<AuthResponse>;