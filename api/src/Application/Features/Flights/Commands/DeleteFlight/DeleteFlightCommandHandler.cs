using FlightManagementSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Application.Features.Flights.Commands.DeleteFlight;

public sealed class DeleteFlightCommandHandler : IRequestHandler<DeleteFlightCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteFlightCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteFlightCommand request, CancellationToken cancellationToken)
    {
        var flight = await _context.Flights
            .Include(x => x.Bookings)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Flight not found.");

        if (flight.Bookings.Any(x => x.Status == Domain.Enums.BookingStatus.Active))
            throw new InvalidOperationException("Flight cannot be deleted while it has active bookings.");

        _context.Flights.Remove(flight);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
