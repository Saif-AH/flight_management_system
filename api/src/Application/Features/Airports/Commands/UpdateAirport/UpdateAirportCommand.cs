using MediatR;

namespace FlightManagementSystem.Application.Features.Airports.Commands.UpdateAirport;

public sealed record UpdateAirportCommand(
    Guid Id,
    string Name,
    string Code,
    string City,
    string Country) : IRequest;
