using FlightManagementSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Application.Features.Bookings.Commands.CancelBooking;

public sealed class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CancelBookingCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
                          .Include(x => x.Flight)
                          .FirstOrDefaultAsync(x => x.Id == request.BookingId, cancellationToken)
                      ?? throw new KeyNotFoundException("Booking not found.");

        var isAdmin = _currentUser.IsInRole("Admin");
        var isOwner = booking.UserId == _currentUser.UserId;

        if (!isAdmin && !isOwner)
            throw new UnauthorizedAccessException("You cannot cancel this booking.");

        if (!isAdmin && !booking.CanBeCancelledByUser(DateTime.UtcNow))
            throw new InvalidOperationException("Bookings can only be cancelled at least 7 days before departure.");

        booking.Cancel();
        booking.Flight.AvailableSeats += booking.SeatsBooked;
        booking.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}