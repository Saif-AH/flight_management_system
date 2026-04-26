using FlightManagementSystem.Application.Features.Bookings.Commands.CancelBooking;
using FluentAssertions;

namespace FlightManagementSystem.UnitTests.Features.Bookings.CancelBooking;

public sealed class CancelBookingCommandValidatorTests
{
    private readonly CancelBookingCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenBookingIdIsEmpty()
    {
        var command = new CancelBookingCommand(Guid.Empty);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "BookingId");
    }

    [Fact]
    public void Validate_ShouldPass_WhenBookingIdIsProvided()
    {
        var command = new CancelBookingCommand(Guid.NewGuid());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
