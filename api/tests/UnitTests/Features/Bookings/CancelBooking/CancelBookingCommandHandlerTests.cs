using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Application.Features.Bookings.Commands.CancelBooking;
using FlightManagementSystem.Domain.Entities;
using FlightManagementSystem.Domain.Enums;
using FlightManagementSystem.UnitTests.Common;
using FluentAssertions;
using Moq;

namespace FlightManagementSystem.UnitTests.Features.Bookings.CancelBooking;

public sealed class CancelBookingCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCancelBookingAndRestoreSeats_WhenUserOwnsBooking()
    {
        await using var context = TestApplicationDbContextFactory.Create();
        var flight = new Flight
        {
            FlightNumber = "EK001",
            DepartureAirportId = Guid.NewGuid(),
            ArrivalAirportId = Guid.NewGuid(),
            DepartureTimeUtc = DateTime.UtcNow.AddDays(10),
            ArrivalTimeUtc = DateTime.UtcNow.AddDays(10).AddHours(3),
            TotalSeats = 100,
            AvailableSeats = 95,
            Status = FlightStatus.Scheduled
        };
        var booking = new Booking
        {
            Flight = flight,
            FlightId = flight.Id,
            UserId = "user-1",
            PassengerName = "Jane Doe",
            SeatsBooked = 5
        };

        context.Bookings.Add(booking);
        await context.SaveChangesAsync(CancellationToken.None);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns("user-1");
        currentUser.Setup(x => x.IsInRole("Admin")).Returns(false);

        var handler = new CancelBookingCommandHandler(context, currentUser.Object);

        await handler.Handle(new CancelBookingCommand(booking.Id), CancellationToken.None);

        context.Bookings.Single().Status.Should().Be(BookingStatus.Cancelled);
        context.Flights.Single().AvailableSeats.Should().Be(100);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserDoesNotOwnBookingAndIsNotAdmin()
    {
        await using var context = TestApplicationDbContextFactory.Create();
        var flight = new Flight
        {
            FlightNumber = "EK001",
            DepartureAirportId = Guid.NewGuid(),
            ArrivalAirportId = Guid.NewGuid(),
            DepartureTimeUtc = DateTime.UtcNow.AddDays(10),
            ArrivalTimeUtc = DateTime.UtcNow.AddDays(10).AddHours(3),
            TotalSeats = 100,
            AvailableSeats = 95,
            Status = FlightStatus.Scheduled
        };
        var booking = new Booking
        {
            Flight = flight,
            FlightId = flight.Id,
            UserId = "owner-id",
            PassengerName = "Jane Doe",
            SeatsBooked = 5
        };

        context.Bookings.Add(booking);
        await context.SaveChangesAsync(CancellationToken.None);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns("other-user");
        currentUser.Setup(x => x.IsInRole("Admin")).Returns(false);

        var handler = new CancelBookingCommandHandler(context, currentUser.Object);

        var act = () => handler.Handle(new CancelBookingCommand(booking.Id), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("You cannot cancel this booking.");
    }
}
