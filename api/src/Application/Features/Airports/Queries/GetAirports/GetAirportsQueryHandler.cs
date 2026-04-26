using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Application.Common.Models;
using FlightManagementSystem.Application.Features.Airports.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Application.Features.Airports.Queries.GetAirports;

public sealed class GetAirportsQueryHandler : IRequestHandler<GetAirportsQuery, PaginatedResult<AirportDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAirportsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<AirportDto>> Handle(GetAirportsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Airports.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(search) ||
                x.Code.ToLower().Contains(search) ||
                x.City.ToLower().Contains(search) ||
                x.Country.ToLower().Contains(search));
        }

        query = (request.SortBy?.ToLower(), request.SortOrder?.ToLower()) switch
        {
            ("code", "desc") => query.OrderByDescending(x => x.Code),
            ("code", _)      => query.OrderBy(x => x.Code),
            ("city", "desc") => query.OrderByDescending(x => x.City),
            ("city", _)      => query.OrderBy(x => x.City),
            ("name", "desc") => query.OrderByDescending(x => x.Name),
            _                => query.OrderBy(x => x.Name)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new AirportDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                City = x.City,
                Country = x.Country
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<AirportDto>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}