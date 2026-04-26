using DotNet.Testcontainers.Builders;
using FlightManagementSystem.Infrastructure.Identity;
using FlightManagementSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace FlightManagementSystem.IntegrationTests.Infrastructure;

public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer? _postgres;
    public bool DockerAvailable => _postgres is not null;

    public async Task InitializeAsync()
    {
        try
        {
            _postgres = new PostgreSqlBuilder("postgres:16-alpine")
                .WithDatabase("flight_management_tests")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

            await _postgres.StartAsync();
        }
        catch (DockerUnavailableException)
        {
            return;
        }

        await ResetDatabaseAsync();
    }

    public new async Task DisposeAsync()
    {
        if (_postgres is not null)
            await _postgres.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _postgres?.GetConnectionString() ?? "InMemory",
                ["Jwt:Issuer"] = "FlightManagementSystem",
                ["Jwt:Audience"] = "FlightManagementSystemUsers",
                ["Jwt:Key"] = "THIS_IS_A_SUPER_SECRET_KEY_CHANGE_IT_123456789",
                ["Jwt:AccessTokenMinutes"] = "30",
                ["Jwt:RefreshTokenDays"] = "7"
            });
        });
    }

    public async Task ResetDatabaseAsync()
    {
        if (!DockerAvailable)
            return;

        using var scope = Services.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.EnsureDeletedAsync();

        if (context.Database.IsRelational())
            await context.Database.MigrateAsync();
        else
            await context.Database.EnsureCreatedAsync();

        await DatabaseSeeder.SeedAsync(context, userManager, roleManager);
    }

    public bool RequireDocker()
    {
        if (!DockerAvailable)
            return false;

        return true;
    }
}
