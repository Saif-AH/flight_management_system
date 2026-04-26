using FluentValidation;

namespace FlightManagementSystem.Application.Features.Flights.Commands.CreateFlight;

public sealed class CreateFlightCommandValidator : AbstractValidator<CreateFlightCommand>
{
    public CreateFlightCommandValidator()
    {
        RuleFor(x => x.FlightNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.DepartureAirportId).NotEmpty();
        RuleFor(x => x.ArrivalAirportId).NotEmpty();
        RuleFor(x => x.TotalSeats).GreaterThan(0);
        RuleFor(x => x.ArrivalTimeUtc)
            .GreaterThan(x => x.DepartureTimeUtc)
            .WithMessage("Arrival time must be after departure time.");
    }
}
