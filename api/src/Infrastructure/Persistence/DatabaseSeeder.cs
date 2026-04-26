using FlightManagementSystem.Domain.Entities;
using FlightManagementSystem.Domain.Enums;
using FlightManagementSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementSystem.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new IdentityRole("User"));

        var adminEmail = "admin@flight.com";
        var userEmail = "user@flight.com";
        var travelerEmail = "traveler@flight.com";

        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Admin",
                EmailConfirmed = true
            };

            await EnsureSucceededAsync(
                userManager.CreateAsync(admin, "Admin123!"),
                $"Failed to seed user '{adminEmail}'.");
            await EnsureSucceededAsync(
                userManager.AddToRoleAsync(admin, "Admin"),
                $"Failed to assign Admin role to '{adminEmail}'.");
        }

        if (await userManager.FindByEmailAsync(userEmail) is null)
        {
            var user = new ApplicationUser
            {
                UserName = userEmail,
                Email = userEmail,
                FullName = "Test User",
                EmailConfirmed = true
            };

            await EnsureSucceededAsync(
                userManager.CreateAsync(user, "User123!"),
                $"Failed to seed user '{userEmail}'.");
            await EnsureSucceededAsync(
                userManager.AddToRoleAsync(user, "User"),
                $"Failed to assign User role to '{userEmail}'.");
        }

        if (await userManager.FindByEmailAsync(travelerEmail) is null)
        {
            var traveler = new ApplicationUser
            {
                UserName = travelerEmail,
                Email = travelerEmail,
                FullName = "Leila Hassan",
                EmailConfirmed = true
            };

            await EnsureSucceededAsync(
                userManager.CreateAsync(traveler, "Traveler123!"),
                $"Failed to seed user '{travelerEmail}'.");
            await EnsureSucceededAsync(
                userManager.AddToRoleAsync(traveler, "User"),
                $"Failed to assign User role to '{travelerEmail}'.");
        }

        if (!await context.Airports.AnyAsync())
        {
            var dxb = new Airport { Name = "Dubai International", Code = "DXB", City = "Dubai", Country = "UAE" };
            var lhr = new Airport { Name = "Heathrow", Code = "LHR", City = "London", Country = "UK" };
            var jfk = new Airport { Name = "John F. Kennedy", Code = "JFK", City = "New York", Country = "USA" };

            context.Airports.AddRange(dxb, lhr, jfk);
            await context.SaveChangesAsync();
        }

        if (!await context.Flights.AnyAsync())
        {
            var airports = await context.Airports.ToListAsync();
            var dxb = airports.First(x => x.Code == "DXB");
            var lhr = airports.First(x => x.Code == "LHR");

            context.Flights.Add(new Flight
            {
                FlightNumber = "EK001",
                DepartureAirportId = dxb.Id,
                ArrivalAirportId = lhr.Id,
                DepartureTimeUtc = DateTime.UtcNow.AddDays(20),
                ArrivalTimeUtc = DateTime.UtcNow.AddDays(20).AddHours(7),
                TotalSeats = 250,
                AvailableSeats = 250,
                Status = FlightStatus.Scheduled
            });

            await context.SaveChangesAsync();
        }

        if (!await context.Bookings.AnyAsync())
        {
            var admin = await userManager.FindByEmailAsync(adminEmail);
            var user = await userManager.FindByEmailAsync(userEmail);
            var traveler = await userManager.FindByEmailAsync(travelerEmail);

            if (admin is null || user is null || traveler is null)
                throw new InvalidOperationException("Seed users must exist before creating seed bookings.");

            var seedFlight = await context.Flights
                .OrderBy(x => x.DepartureTimeUtc)
                .FirstOrDefaultAsync(
                    x => x.Status == FlightStatus.Scheduled && x.DepartureTimeUtc > DateTime.UtcNow);

            if (seedFlight is not null)
            {
                var bookingPlans = new[]
                {
                    new { UserId = user.Id, PassengerName = "Test User", SeatsRequested = 2, Status = BookingStatus.Active, BookedAtUtc = DateTime.UtcNow.AddDays(-5) },
                    new { UserId = traveler.Id, PassengerName = "Leila Hassan", SeatsRequested = 1, Status = BookingStatus.Active, BookedAtUtc = DateTime.UtcNow.AddDays(-3) },
                    new { UserId = admin.Id, PassengerName = "System Admin", SeatsRequested = 1, Status = BookingStatus.Cancelled, BookedAtUtc = DateTime.UtcNow.AddDays(-1) }
                };

                var bookings = new List<Booking>();
                var remainingSeats = seedFlight.AvailableSeats;

                foreach (var plan in bookingPlans)
                {
                    var seatsBooked = plan.Status == BookingStatus.Active
                        ? Math.Min(plan.SeatsRequested, remainingSeats)
                        : plan.SeatsRequested;

                    if (plan.Status == BookingStatus.Active && seatsBooked <= 0)
                        continue;

                    bookings.Add(new Booking
                    {
                        FlightId = seedFlight.Id,
                        UserId = plan.UserId,
                        PassengerName = plan.PassengerName,
                        SeatsBooked = seatsBooked,
                        Status = plan.Status,
                        BookedAtUtc = plan.BookedAtUtc
                    });

                    if (plan.Status == BookingStatus.Active)
                        remainingSeats -= seatsBooked;
                }

                if (bookings.Count > 0)
                {
                    seedFlight.AvailableSeats = remainingSeats;
                    seedFlight.UpdatedAtUtc = DateTime.UtcNow;
                    context.Bookings.AddRange(bookings);
                    await context.SaveChangesAsync();
                }
            }
        }
    }

    private static async Task EnsureSucceededAsync(Task<IdentityResult> task, string errorMessage)
    {
        var result = await task;
        if (result.Succeeded)
            return;

        var errors = string.Join("; ", result.Errors.Select(x => x.Description));
        throw new InvalidOperationException($"{errorMessage} {errors}");
    }
}
