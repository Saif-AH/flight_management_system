using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Application.Common.Models;
using FlightManagementSystem.Application.Features.Flights.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Application.Features.Flights.Queries.GetFlights;

public sealed class GetFlightsQueryHandler : IRequestHandler<GetFlightsQuery, PaginatedResult<FlightDto>>
{
    private readonly IApplicationDbContext _context;

    public GetFlightsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<FlightDto>> Handle(GetFlightsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Flights
            .AsNoTracking()
            .Include(x => x.DepartureAirport)
            .Include(x => x.ArrivalAirport)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x =>
                x.FlightNumber.ToLower().Contains(search) ||
                x.DepartureAirport.Code.ToLower().Contains(search) ||
                x.DepartureAirport.Name.ToLower().Contains(search) ||
                x.ArrivalAirport.Code.ToLower().Contains(search) ||
                x.ArrivalAirport.Name.ToLower().Contains(search));
        }

        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status.Value);

        query = (request.SortBy?.ToLower(), request.SortOrder?.ToLower()) switch
        {
            ("flightnumber", "desc") => query.OrderByDescending(x => x.FlightNumber),
            ("flightnumber", _) => query.OrderBy(x => x.FlightNumber),
            ("arrivaltimeutc", "desc") => query.OrderByDescending(x => x.ArrivalTimeUtc),
            ("arrivaltimeutc", _) => query.OrderBy(x => x.ArrivalTimeUtc),
            ("status", "desc") => query.OrderByDescending(x => x.Status),
            ("status", _) => query.OrderBy(x => x.Status),
            ("departuretimeutc", "desc") => query.OrderByDescending(x => x.DepartureTimeUtc),
            _ => query.OrderBy(x => x.DepartureTimeUtc)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new FlightDto
            {
                Id = x.Id,
                FlightNumber = x.FlightNumber,
                DepartureAirportId = x.DepartureAirportId,
                DepartureAirportCode = x.DepartureAirport.Code,
                DepartureAirportName = x.DepartureAirport.Name,
                ArrivalAirportId = x.ArrivalAirportId,
                ArrivalAirportCode = x.ArrivalAirport.Code,
                ArrivalAirportName = x.ArrivalAirport.Name,
                DepartureTimeUtc = x.DepartureTimeUtc,
                ArrivalTimeUtc = x.ArrivalTimeUtc,
                TotalSeats = x.TotalSeats,
                AvailableSeats = x.AvailableSeats,
                Status = x.Status
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<FlightDto>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
