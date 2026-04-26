using FlightManagementSystem.Application.Features.Bookings.Commands.CreateBooking;
using FluentAssertions;

namespace FlightManagementSystem.UnitTests.Features.Bookings.CreateBooking;

public sealed class CreateBookingCommandValidatorTests
{
    private readonly CreateBookingCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenPassengerNameIsEmpty()
    {
        var command = new CreateBookingCommand(Guid.NewGuid(), string.Empty, 1);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "PassengerName");
    }

    [Fact]
    public void Validate_ShouldFail_WhenSeatsBookedIsNotPositive()
    {
        var command = new CreateBookingCommand(Guid.NewGuid(), "Jane Doe", 0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "SeatsBooked");
    }

    [Fact]
    public void Validate_ShouldPass_ForValidCommand()
    {
        var command = new CreateBookingCommand(Guid.NewGuid(), "Jane Doe", 2);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
