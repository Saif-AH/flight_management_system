using FlightManagementSystem.Application.Features.Dashboard.Queries.GetDashboardStats;
using FlightManagementSystem.Domain.Entities;
using FlightManagementSystem.Domain.Enums;
using FlightManagementSystem.UnitTests.Common;
using FluentAssertions;

namespace FlightManagementSystem.UnitTests.Features.Dashboard.GetDashboardStats;

public sealed class GetDashboardStatsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnAggregatedDashboardCounts()
    {
        await using var context = TestApplicationDbContextFactory.Create();

        var departureAirport = new Airport
        {
            Name = "Baghdad International Airport",
            Code = "BGW",
            City = "Baghdad",
            Country = "Iraq"
        };

        var arrivalAirport = new Airport
        {
            Name = "Dubai International Airport",
            Code = "DXB",
            City = "Dubai",
            Country = "UAE"
        };

        context.Airports.AddRange(departureAirport, arrivalAirport);

        var scheduledFlight = new Flight
        {
            FlightNumber = "IA101",
            DepartureAirport = departureAirport,
            ArrivalAirport = arrivalAirport,
            DepartureTimeUtc = DateTime.UtcNow.AddHours(4),
            ArrivalTimeUtc = DateTime.UtcNow.AddHours(6),
            TotalSeats = 180,
            AvailableSeats = 50,
            Status = FlightStatus.Scheduled
        };

        var delayedFlight = new Flight
        {
            FlightNumber = "IA202",
            DepartureAirport = arrivalAirport,
            ArrivalAirport = departureAirport,
            DepartureTimeUtc = DateTime.UtcNow.AddHours(8),
            ArrivalTimeUtc = DateTime.UtcNow.AddHours(10),
            TotalSeats = 180,
            AvailableSeats = 80,
            Status = FlightStatus.Delayed
        };

        var cancelledFlight = new Flight
        {
            FlightNumber = "IA303",
            DepartureAirport = departureAirport,
            ArrivalAirport = arrivalAirport,
            DepartureTimeUtc = DateTime.UtcNow.AddDays(-1),
            ArrivalTimeUtc = DateTime.UtcNow.AddDays(-1).AddHours(2),
            TotalSeats = 150,
            AvailableSeats = 150,
            Status = FlightStatus.Cancelled
        };

        var completedFlight = new Flight
        {
            FlightNumber = "IA404",
            DepartureAirport = arrivalAirport,
            ArrivalAirport = departureAirport,
            DepartureTimeUtc = DateTime.UtcNow.AddDays(-2),
            ArrivalTimeUtc = DateTime.UtcNow.AddDays(-2).AddHours(2),
            TotalSeats = 150,
            AvailableSeats = 0,
            Status = FlightStatus.Completed
        };

        context.Flights.AddRange(scheduledFlight, delayedFlight, cancelledFlight, completedFlight);
        context.Bookings.AddRange(
            new Booking
            {
                Flight = scheduledFlight,
                UserId = "user-1",
                PassengerName = "Jane Doe",
                SeatsBooked = 2,
                Status = BookingStatus.Active
            },
            new Booking
            {
                Flight = cancelledFlight,
                UserId = "user-2",
                PassengerName = "John Doe",
                SeatsBooked = 1,
                Status = BookingStatus.Cancelled
            });

        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetDashboardStatsQueryHandler(context);

        var result = await handler.Handle(new GetDashboardStatsQuery(), CancellationToken.None);

        result.TotalFlights.Should().Be(4);
        result.UpcomingFlights.Should().Be(2);
        result.TotalAirports.Should().Be(2);
        result.ActiveBookings.Should().Be(1);
        result.ScheduledFlights.Should().Be(1);
        result.DelayedFlights.Should().Be(1);
        result.CancelledFlights.Should().Be(1);
        result.CompletedFlights.Should().Be(1);
    }
}
