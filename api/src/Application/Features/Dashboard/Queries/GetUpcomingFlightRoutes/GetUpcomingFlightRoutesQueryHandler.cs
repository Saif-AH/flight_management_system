using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Application.Features.Dashboard.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Application.Features.Dashboard.Queries.GetUpcomingFlightRoutes;

public sealed class GetUpcomingFlightRoutesQueryHandler
    : IRequestHandler<GetUpcomingFlightRoutesQuery, IReadOnlyList<UpcomingFlightRouteDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUpcomingFlightRoutesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<UpcomingFlightRouteDto>> Handle(
        GetUpcomingFlightRoutesQuery request,
        CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;
        var limit = request.Limit <= 0 ? 5 : request.Limit;

        return await _context.Flights
            .AsNoTracking()
            .Include(x => x.DepartureAirport)
            .Include(x => x.ArrivalAirport)
            .Where(x => x.DepartureTimeUtc >= nowUtc)
            .OrderBy(x => x.DepartureTimeUtc)
            .Take(limit)
            .Select(x => new UpcomingFlightRouteDto
            {
                FlightId = x.Id,
                FlightNumber = x.FlightNumber,
                DepartureAirportCode = x.DepartureAirport.Code,
                DepartureAirportName = x.DepartureAirport.Name,
                ArrivalAirportCode = x.ArrivalAirport.Code,
                ArrivalAirportName = x.ArrivalAirport.Name,
                Route = x.DepartureAirport.Code + " -> " + x.ArrivalAirport.Code,
                DepartureTimeUtc = x.DepartureTimeUtc,
                ArrivalTimeUtc = x.ArrivalTimeUtc,
                Status = x.Status,
                TotalSeats = x.TotalSeats,
                AvailableSeats = x.AvailableSeats
            })
            .ToListAsync(cancellationToken);
    }
}
