using FlightManagementSystem.Domain.Enums;
using FlightManagementSystem.Infrastructure.Identity;
using FlightManagementSystem.Infrastructure.Persistence;
using FlightManagementSystem.UnitTests.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace FlightManagementSystem.UnitTests.Infrastructure.Persistence;

public sealed class DatabaseSeederTests
{
    [Fact]
    public async Task SeedAsync_ShouldCreateSeedBookingsAndUpdateAvailableSeats()
    {
        await using var context = TestApplicationDbContextFactory.Create();
        using var userStore = new UserStore<ApplicationUser>(context);
        using var roleStore = new RoleStore<IdentityRole>(context);

        using var userManager = CreateUserManager(userStore);
        using var roleManager = CreateRoleManager(roleStore);

        await DatabaseSeeder.SeedAsync(context, userManager, roleManager);

        context.Bookings.Should().HaveCount(3);
        context.Bookings.Count(x => x.Status == BookingStatus.Active).Should().Be(2);
        context.Bookings.Count(x => x.Status == BookingStatus.Cancelled).Should().Be(1);

        var flight = context.Flights.Single(x => x.FlightNumber == "EK001");
        flight.AvailableSeats.Should().Be(247);

        context.Bookings.Should().Contain(x => x.PassengerName == "Test User" && x.SeatsBooked == 2);
        context.Bookings.Should().Contain(x => x.PassengerName == "Leila Hassan" && x.SeatsBooked == 1);
        context.Bookings.Should().Contain(x => x.PassengerName == "System Admin" && x.Status == BookingStatus.Cancelled);
    }

    [Fact]
    public async Task SeedAsync_ShouldBeIdempotentForBookings()
    {
        await using var context = TestApplicationDbContextFactory.Create();
        using var userStore = new UserStore<ApplicationUser>(context);
        using var roleStore = new RoleStore<IdentityRole>(context);

        using var userManager = CreateUserManager(userStore);
        using var roleManager = CreateRoleManager(roleStore);

        await DatabaseSeeder.SeedAsync(context, userManager, roleManager);
        await DatabaseSeeder.SeedAsync(context, userManager, roleManager);

        context.Bookings.Should().HaveCount(3);
        context.Flights.Single(x => x.FlightNumber == "EK001").AvailableSeats.Should().Be(247);
    }

    private static UserManager<ApplicationUser> CreateUserManager(IUserStore<ApplicationUser> userStore)
    {
        var options = Options.Create(new IdentityOptions
        {
            Password =
            {
                RequireDigit = true,
                RequireUppercase = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = false,
                RequiredLength = 6
            }
        });

        return new UserManager<ApplicationUser>(
            userStore,
            options,
            new PasswordHasher<ApplicationUser>(),
            new List<IUserValidator<ApplicationUser>> { new UserValidator<ApplicationUser>() },
            new List<IPasswordValidator<ApplicationUser>> { new PasswordValidator<ApplicationUser>() },
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null!,
            NullLogger<UserManager<ApplicationUser>>.Instance);
    }

    private static RoleManager<IdentityRole> CreateRoleManager(IRoleStore<IdentityRole> roleStore)
        => new(
            roleStore,
            new List<IRoleValidator<IdentityRole>> { new RoleValidator<IdentityRole>() },
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            NullLogger<RoleManager<IdentityRole>>.Instance);
}
