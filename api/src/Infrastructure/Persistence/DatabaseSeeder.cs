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

        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Admin",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Admin");
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

            await userManager.CreateAsync(user, "User123!");
            await userManager.AddToRoleAsync(user, "User");
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
    }
}