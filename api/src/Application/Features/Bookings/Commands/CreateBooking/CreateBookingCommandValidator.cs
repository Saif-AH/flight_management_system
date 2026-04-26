using FluentValidation;

namespace FlightManagementSystem.Application.Features.Bookings.Commands.CreateBooking;

public sealed class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.FlightId).NotEmpty();
        RuleFor(x => x.PassengerName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.SeatsBooked).GreaterThan(0);
    }
}
