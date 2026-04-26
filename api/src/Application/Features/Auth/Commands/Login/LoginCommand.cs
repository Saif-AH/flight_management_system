using FlightManagementSystem.Application.Features.Auth.Common;
using MediatR;

namespace FlightManagementSystem.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password) :  IRequest<AuthResponse>;