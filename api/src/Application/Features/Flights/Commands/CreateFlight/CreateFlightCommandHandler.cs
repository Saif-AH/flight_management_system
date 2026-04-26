using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Application.Features.Flights.Commands.CreateFlight;

public sealed class CreateFlightCommandHandler : IRequestHandler<CreateFlightCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateFlightCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateFlightCommand request, CancellationToken cancellationToken)
    {
        await ValidateAirportsAsync(request.DepartureAirportId, request.ArrivalAirportId, cancellationToken);

        var flight = new Flight
        {
            FlightNumber = request.FlightNumber.Trim().ToUpperInvariant(),
            DepartureAirportId = request.DepartureAirportId,
            ArrivalAirportId = request.ArrivalAirportId,
            DepartureTimeUtc = request.DepartureTimeUtc,
            ArrivalTimeUtc = request.ArrivalTimeUtc,
            TotalSeats = request.TotalSeats,
            AvailableSeats = request.TotalSeats,
            Status = request.Status
        };

        _context.Flights.Add(flight);
        await _context.SaveChangesAsync(cancellationToken);

        return flight.Id;
    }

    private async Task ValidateAirportsAsync(Guid departureAirportId, Guid arrivalAirportId, CancellationToken cancellationToken)
    {
        if (departureAirportId == arrivalAirportId)
            throw new InvalidOperationException("Departure and arrival airports must be different.");

        var airportIds = await _context.Airports
            .Where(x => x.Id == departureAirportId || x.Id == arrivalAirportId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (!airportIds.Contains(departureAirportId))
            throw new KeyNotFoundException("Departure airport not found.");

        if (!airportIds.Contains(arrivalAirportId))
            throw new KeyNotFoundException("Arrival airport not found.");
    }
}
