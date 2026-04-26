using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Application.Features.Dashboard.Common;
using FlightManagementSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Application.Features.Dashboard.Queries.GetDashboardStats;

public sealed class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;

        return new DashboardStatsDto
        {
            TotalFlights = await _context.Flights.CountAsync(cancellationToken),
            UpcomingFlights = await _context.Flights.CountAsync(
                x => x.DepartureTimeUtc >= nowUtc,
                cancellationToken),
            TotalAirports = await _context.Airports.CountAsync(cancellationToken),
            ActiveBookings = await _context.Bookings.CountAsync(
                x => x.Status == BookingStatus.Active,
                cancellationToken),
            ScheduledFlights = await _context.Flights.CountAsync(
                x => x.Status == FlightStatus.Scheduled,
                cancellationToken),
            DelayedFlights = await _context.Flights.CountAsync(
                x => x.Status == FlightStatus.Delayed,
                cancellationToken),
            CancelledFlights = await _context.Flights.CountAsync(
                x => x.Status == FlightStatus.Cancelled,
                cancellationToken),
            CompletedFlights = await _context.Flights.CountAsync(
                x => x.Status == FlightStatus.Completed,
                cancellationToken)
        };
    }
}
