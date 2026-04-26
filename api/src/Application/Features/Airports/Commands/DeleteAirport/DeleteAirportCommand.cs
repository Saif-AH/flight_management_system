using MediatR;

namespace FlightManagementSystem.Application.Features.Airports.Commands.DeleteAirport;

public sealed record DeleteAirportCommand(Guid Id) : IRequest;
