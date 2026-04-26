using FlightManagementSystem.Domain.Entities;
using FlightManagementSystem.Domain.Enums;
using FluentAssertions;

namespace FlightManagementSystem.UnitTests.Domain.Entities;

public sealed class BookingTests
{
    [Fact]
    public void CanBeCancelledByUser_ShouldReturnTrue_WhenActiveAndAtLeastSevenDaysBeforeDeparture()
    {
        var booking = new Booking
        {
            Status = BookingStatus.Active,
            Flight = new Flight
            {
                DepartureTimeUtc = DateTime.UtcNow.AddDays(8)
            }
        };

        var canCancel = booking.CanBeCancelledByUser(DateTime.UtcNow);

        canCancel.Should().BeTrue();
    }

    [Fact]
    public void CanBeCancelledByUser_ShouldReturnFalse_WhenWithinSevenDaysOfDeparture()
    {
        var booking = new Booking
        {
            Status = BookingStatus.Active,
            Flight = new Flight
            {
                DepartureTimeUtc = DateTime.UtcNow.AddDays(3)
            }
        };

        var canCancel = booking.CanBeCancelledByUser(DateTime.UtcNow);

        canCancel.Should().BeFalse();
    }

    [Fact]
    public void Cancel_ShouldSetStatusToCancelled()
    {
        var booking = new Booking { Status = BookingStatus.Active };

        booking.Cancel();

        booking.Status.Should().Be(BookingStatus.Cancelled);
    }

    [Fact]
    public void Cancel_ShouldThrow_WhenAlreadyCancelled()
    {
        var booking = new Booking { Status = BookingStatus.Cancelled };

        var act = () => booking.Cancel();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Booking already cancelled.");
    }
}
