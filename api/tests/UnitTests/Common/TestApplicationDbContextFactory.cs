using FlightManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.UnitTests.Common;

internal static class TestApplicationDbContextFactory
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
