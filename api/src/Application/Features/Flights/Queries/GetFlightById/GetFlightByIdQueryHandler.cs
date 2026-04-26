using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Application.Features.Flights.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Application.Features.Flights.Queries.GetFlightById;

public sealed class GetFlightByIdQueryHandler : IRequestHandler<GetFlightByIdQuery, FlightDto>
{
    private readonly IApplicationDbContext _context;

    public GetFlightByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FlightDto> Handle(GetFlightByIdQuery request, CancellationToken cancellationToken)
    {
        var flight = await _context.Flights
            .AsNoTracking()
            .Include(x => x.DepartureAirport)
            .Include(x => x.ArrivalAirport)
            .Where(x => x.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (flight is null)
            throw new KeyNotFoundException("Flight not found.");

        return flight;
    }
}
