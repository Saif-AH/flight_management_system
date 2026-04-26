using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Application.Features.Airports.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Application.Features.Airports.Queries.GetAirportById;

public sealed class GetAirportByIdQueryHandler : IRequestHandler<GetAirportByIdQuery, AirportDto>
{
    private readonly IApplicationDbContext _context;

    public GetAirportByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AirportDto> Handle(GetAirportByIdQuery request, CancellationToken cancellationToken)
    {
        var airport = await _context.Airports
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new AirportDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                City = x.City,
                Country = x.Country
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (airport is null)
            throw new KeyNotFoundException("Airport not found.");

        return airport;
    }
}
