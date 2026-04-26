using FlightManagementSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Application.Features.Airports.Commands.DeleteAirport;

public sealed class DeleteAirportCommandHandler : IRequestHandler<DeleteAirportCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteAirportCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteAirportCommand request, CancellationToken cancellationToken)
    {
        var airport = await _context.Airports
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Airport not found.");

        _context.Airports.Remove(airport);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
