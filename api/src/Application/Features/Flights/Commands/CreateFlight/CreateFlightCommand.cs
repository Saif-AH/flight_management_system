using FlightManagementSystem.Domain.Enums;
using MediatR;

namespace FlightManagementSystem.Application.Features.Flights.Commands.CreateFlight;

public sealed record CreateFlightCommand(
    string FlightNumber,
    Guid DepartureAirportId,
    Guid ArrivalAirportId,
    DateTime DepartureTimeUtc,
    DateTime ArrivalTimeUtc,
    int TotalSeats,
    FlightStatus Status = FlightStatus.Scheduled) : IRequest<Guid>;
