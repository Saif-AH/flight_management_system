using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Application.Common.Models;
using FlightManagementSystem.Application.Features.Bookings.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Application.Features.Bookings.Queries.GetMyBookings;

public sealed class GetMyBookingsQueryHandler : IRequestHandler<GetMyBookingsQuery, PaginatedResult<BookingDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyBookingsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedResult<BookingDto>> Handle(GetMyBookingsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Bookings
            .AsNoTracking()
            .Include(x => x.Flight)
                .ThenInclude(x => x.DepartureAirport)
            .Include(x => x.Flight)
                .ThenInclude(x => x.ArrivalAirport)
            .Where(x => x.UserId == _currentUser.UserId)
            .AsQueryable();

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
