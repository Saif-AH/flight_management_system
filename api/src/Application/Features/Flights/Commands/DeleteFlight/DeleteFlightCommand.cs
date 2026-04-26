using MediatR;

namespace FlightManagementSystem.Application.Features.Flights.Commands.DeleteFlight;

public sealed record DeleteFlightCommand(Guid Id) : IRequest;
