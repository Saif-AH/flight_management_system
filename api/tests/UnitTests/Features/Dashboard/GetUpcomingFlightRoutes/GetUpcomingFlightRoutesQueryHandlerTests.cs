using FlightManagementSystem.Application.Features.Dashboard.Queries.GetUpcomingFlightRoutes;
using FlightManagementSystem.Domain.Entities;
using FlightManagementSystem.Domain.Enums;
using FlightManagementSystem.UnitTests.Common;
using FluentAssertions;

namespace FlightManagementSystem.UnitTests.Features.Dashboard.GetUpcomingFlightRoutes;

public sealed class GetUpcomingFlightRoutesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnUpcomingFlightsOrderedByDeparture()
    {
        await using var context = TestApplicationDbContextFactory.Create();

        var baghdad = new Airport
        {
            Name = "Baghdad International Airport",
            Code = "BGW",
            City = "Baghdad",
            Country = "Iraq"
        };

        var dubai = new Airport
        {
            Name = "Dubai International Airport",
            Code = "DXB",
            City = "Dubai",
            Country = "UAE"
        };

        var istanbul = new Airport
        {
            Name = "Istanbul Airport",
            Code = "IST",
            City = "Istanbul",
            Country = "Turkey"
        };

        context.Airports.AddRange(baghdad, dubai, istanbul);

        context.Flights.AddRange(
            new Flight
            {
                FlightNumber = "IA300",
                DepartureAirport = baghdad,
                ArrivalAirport = istanbul,
                DepartureTimeUtc = DateTime.UtcNow.AddHours(6),
                ArrivalTimeUtc = DateTime.UtcNow.AddHours(9),
                TotalSeats = 200,
                AvailableSeats = 40,
                Status = FlightStatus.Scheduled
            },
            new Flight
            {
                FlightNumber = "IA200",
                DepartureAirport = baghdad,
                ArrivalAirport = dubai,
                DepartureTimeUtc = DateTime.UtcNow.AddHours(2),
                ArrivalTimeUtc = DateTime.UtcNow.AddHours(4),
                TotalSeats = 180,
                AvailableSeats = 60,
                Status = FlightStatus.Delayed
            },
            new Flight
            {
                FlightNumber = "IA100",
                DepartureAirport = dubai,
                ArrivalAirport = baghdad,
                DepartureTimeUtc = DateTime.UtcNow.AddDays(-1),
                ArrivalTimeUtc = DateTime.UtcNow.AddDays(-1).AddHours(2),
                TotalSeats = 180,
                AvailableSeats = 100,
                Status = FlightStatus.Completed
            });

        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetUpcomingFlightRoutesQueryHandler(context);

        var result = await handler.Handle(new GetUpcomingFlightRoutesQuery(1), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].FlightNumber.Should().Be("IA200");
        result[0].Route.Should().Be("BGW -> DXB");
        result[0].DepartureAirportName.Should().Be("Baghdad International Airport");
        result[0].ArrivalAirportName.Should().Be("Dubai International Airport");
    }

    [Fact]
    public async Task Handle_ShouldFallbackToDefaultLimit_WhenLimitIsInvalid()
    {
        await using var context = TestApplicationDbContextFactory.Create();

        var baghdad = new Airport
        {
            Name = "Baghdad International Airport",
            Code = "BGW",
            City = "Baghdad",
            Country = "Iraq"
        };

        var dubai = new Airport
        {
            Name = "Dubai International Airport",
            Code = "DXB",
            City = "Dubai",
            Country = "UAE"
        };

        context.Airports.AddRange(baghdad, dubai);

        for (var index = 0; index < 6; index++)
        {
            context.Flights.Add(new Flight
            {
                FlightNumber = $"IA{index + 1:000}",
                DepartureAirport = baghdad,
                ArrivalAirport = dubai,
                DepartureTimeUtc = DateTime.UtcNow.AddHours(index + 1),
                ArrivalTimeUtc = DateTime.UtcNow.AddHours(index + 3),
                TotalSeats = 180,
                AvailableSeats = 90,
                Status = FlightStatus.Scheduled
            });
        }

        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetUpcomingFlightRoutesQueryHandler(context);

        var result = await handler.Handle(new GetUpcomingFlightRoutesQuery(0), CancellationToken.None);

        result.Should().HaveCount(5);
    }
}
