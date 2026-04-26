using MediatR;

namespace FlightManagementSystem.Application.Features.Bookings.Commands.CreateBooking;

public sealed record CreateBookingCommand(
    Guid FlightId,
    string PassengerName,
    int SeatsBooked) : IRequest<Guid>;