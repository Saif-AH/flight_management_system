using MediatR;

namespace FlightManagementSystem.Application.Features.Airports.Commands.CreateAirport;

public sealed record CreateAirportCommand(
    string Name,
    string Code,
    string City,
    string Country) : IRequest<Guid>;