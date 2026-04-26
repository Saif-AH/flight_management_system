using FlightManagementSystem.Domain.Enums;
using MediatR;

namespace FlightManagementSystem.Application.Features.Flights.Commands.UpdateFlight;

public sealed record UpdateFlightCommand(
    Guid Id,
    string FlightNumber,
    Guid DepartureAirportId,
    Guid ArrivalAirportId,
    DateTime DepartureTimeUtc,
    DateTime ArrivalTimeUtc,
    int TotalSeats,
    FlightStatus Status) : IRequest;
