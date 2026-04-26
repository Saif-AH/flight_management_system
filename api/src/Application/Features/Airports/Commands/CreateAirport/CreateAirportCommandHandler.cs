using FlightManagementSystem.Application.Common.Interfaces;
using FlightManagementSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Application.Features.Airports.Commands.CreateAirport;

public sealed class CreateAirportCommandHandler : IRequestHandler<CreateAirportCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateAirportCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateAirportCommand request, CancellationToken cancellationToken)
    {
        var codeExists = await _context.Airports
            .AnyAsync(x => x.Code == request.Code, cancellationToken);

        if (codeExists)
            throw new InvalidOperationException("Airport code already exists.");

        var airport = new Airport()
        {
            Name = request.Name,
            Code = request.Code,
            City = request.City,
            Country = request.Country
        };

        _context.Airports.Add(airport);
        await _context.SaveChangesAsync(cancellationToken);

        return airport.Id;
    }
}