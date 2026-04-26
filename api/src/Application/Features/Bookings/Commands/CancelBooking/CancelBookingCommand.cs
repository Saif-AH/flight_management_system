using MediatR;

namespace FlightManagementSystem.Application.Features.Bookings.Commands.CancelBooking;

public sealed record CancelBookingCommand(Guid BookingId) : IRequest;