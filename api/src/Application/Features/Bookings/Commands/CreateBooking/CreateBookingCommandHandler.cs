using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Application.Features.Bookings.Commands.CreateBooking;

public sealed class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateBookingCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var flight = await _context.Flights
                         .FirstOrDefaultAsync(x => x.Id == request.FlightId, cancellationToken)
                     ?? throw new KeyNotFoundException("Flight not found.");

        if (flight.Status != Domain.Enums.FlightStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled flights can be booked.");

        if (request.SeatsBooked <= 0)
            throw new InvalidOperationException("Seats booked must be greater than 0.");

        if (flight.AvailableSeats < request.SeatsBooked)
            throw new InvalidOperationException("Not enough available seats.");

        flight.AvailableSeats -= request.SeatsBooked;
        flight.UpdatedAtUtc = DateTime.UtcNow;

        var booking = new Booking()
        {
            FlightId = flight.Id,
            UserId = _currentUser.UserId!,
            PassengerName = request.PassengerName,
            SeatsBooked = request.SeatsBooked
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync(cancellationToken);

        return booking.Id;
    }
}