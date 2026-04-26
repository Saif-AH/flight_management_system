using FlightManagementSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Application.Features.Flights.Commands.UpdateFlight;

public sealed class UpdateFlightCommandHandler : IRequestHandler<UpdateFlightCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateFlightCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateFlightCommand request, CancellationToken cancellationToken)
    {
        var flight = await _context.Flights
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Flight not found.");

        await ValidateAirportsAsync(request.DepartureAirportId, request.ArrivalAirportId, cancellationToken);

        var bookedSeats = flight.TotalSeats - flight.AvailableSeats;
        if (request.TotalSeats < bookedSeats)
            throw new InvalidOperationException("Total seats cannot be less than seats already booked.");

        flight.FlightNumber = request.FlightNumber.Trim().ToUpperInvariant();
        flight.DepartureAirportId = request.DepartureAirportId;
        flight.ArrivalAirportId = request.ArrivalAirportId;
        flight.DepartureTimeUtc = request.DepartureTimeUtc;
        flight.ArrivalTimeUtc = request.ArrivalTimeUtc;
        flight.TotalSeats = request.TotalSeats;
        flight.AvailableSeats = request.TotalSeats - bookedSeats;
        flight.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);
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
