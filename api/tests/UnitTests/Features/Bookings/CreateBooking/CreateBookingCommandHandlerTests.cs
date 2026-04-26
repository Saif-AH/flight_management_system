using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Application.Features.Bookings.Commands.CreateBooking;
using FlightManagementSystem.Domain.Entities;
using FlightManagementSystem.Domain.Enums;
using FlightManagementSystem.UnitTests.Common;
using FluentAssertions;
using Moq;

namespace FlightManagementSystem.UnitTests.Features.Bookings.CreateBooking;

public sealed class CreateBookingCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateBookingAndDecreaseAvailableSeats()
    {
        await using var context = TestApplicationDbContextFactory.Create();
        var flight = new Flight
        {
            FlightNumber = "EK001",
            DepartureAirportId = Guid.NewGuid(),
            ArrivalAirportId = Guid.NewGuid(),
            DepartureTimeUtc = DateTime.UtcNow.AddDays(10),
            ArrivalTimeUtc = DateTime.UtcNow.AddDays(10).AddHours(2),
            TotalSeats = 100,
            AvailableSeats = 100,
            Status = FlightStatus.Scheduled
        };
        context.Flights.Add(flight);
        await context.SaveChangesAsync(CancellationToken.None);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns("user-1");

        var handler = new CreateBookingCommandHandler(context, currentUser.Object);

        var bookingId = await handler.Handle(
            new CreateBookingCommand(flight.Id, "Jane Doe", 2),
            CancellationToken.None);

        bookingId.Should().NotBeEmpty();
        context.Bookings.Should().ContainSingle(x => x.Id == bookingId && x.UserId == "user-1");
        context.Flights.Single().AvailableSeats.Should().Be(98);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNotEnoughSeats()
    {
        await using var context = TestApplicationDbContextFactory.Create();
        context.Flights.Add(new Flight
        {
            FlightNumber = "EK001",
            DepartureAirportId = Guid.NewGuid(),
            ArrivalAirportId = Guid.NewGuid(),
            DepartureTimeUtc = DateTime.UtcNow.AddDays(10),
            ArrivalTimeUtc = DateTime.UtcNow.AddDays(10).AddHours(2),
            TotalSeats = 2,
            AvailableSeats = 1,
            Status = FlightStatus.Scheduled
        });
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateBookingCommandHandler(context, Mock.Of<ICurrentUserService>(x => x.UserId == "user-1"));

        var act = () => handler.Handle(
            new CreateBookingCommand(context.Flights.Single().Id, "Jane Doe", 2),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Not enough available seats.");
    }
}
