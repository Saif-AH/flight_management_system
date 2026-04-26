using FlightManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Application.Common.Interfaces;

public interface  IApplicationDbContext
{
    DbSet<Airport> Airports { get; }
    DbSet<Flight> Flights { get; }
    DbSet<Booking> Bookings { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}