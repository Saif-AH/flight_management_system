using FlightManagementSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Application.Features.Airports.Commands.UpdateAirport;

public sealed class UpdateAirportCommandHandler : IRequestHandler<UpdateAirportCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateAirportCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateAirportCommand request, CancellationToken cancellationToken)
    {
        var airport = await _context.Airports
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Airport not found.");

        var codeExists = await _context.Airports
            .AnyAsync(x => x.Id != request.Id && x.Code == request.Code, cancellationToken);

        if (codeExists)
            throw new InvalidOperationException("Airport code already exists.");

        airport.Name = request.Name;
        airport.Code = request.Code;
        airport.City = request.City;
        airport.Country = request.Country;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
