using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Application.Common.Models;
using FlightManagementSystem.Application.Features.Bookings.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Application.Features.Bookings.Queries.GetBookings;

public sealed class GetBookingsQueryHandler : IRequestHandler<GetBookingsQuery, PaginatedResult<BookingDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBookingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<BookingDto>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Bookings
            .AsNoTracking()
            .Include(x => x.Flight)
                .ThenInclude(x => x.DepartureAirport)
            .Include(x => x.Flight)
                .ThenInclude(x => x.ArrivalAirport)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x =>
                x.PassengerName.ToLower().Contains(search) ||
                x.UserId.ToLower().Contains(search) ||
                x.Flight.FlightNumber.ToLower().Contains(search) ||
                x.Flight.DepartureAirport.Code.ToLower().Contains(search) ||
                x.Flight.ArrivalAirport.Code.ToLower().Contains(search));
        }

        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status.Value);

        query = query.OrderByDescending(x => x.BookedAtUtc);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new BookingDto
            {
                Id = x.Id,
                FlightId = x.FlightId,
                FlightNumber = x.Flight.FlightNumber,
                PassengerName = x.PassengerName,
                UserId = x.UserId,
                SeatsBooked = x.SeatsBooked,
                Status = x.Status,
                BookedAtUtc = x.BookedAtUtc,
                DepartureTimeUtc = x.Flight.DepartureTimeUtc,
                ArrivalTimeUtc = x.Flight.ArrivalTimeUtc,
                DepartureAirportCode = x.Flight.DepartureAirport.Code,
                ArrivalAirportCode = x.Flight.ArrivalAirport.Code
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<BookingDto>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
