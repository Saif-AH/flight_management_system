using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FlightManagementSystem.Application.Common.Models;
using FlightManagementSystem.Application.Features.Auth.Common;
using FlightManagementSystem.Application.Features.Bookings.Common;
using FlightManagementSystem.Application.Features.Flights.Common;

namespace FlightManagementSystem.IntegrationTests.Infrastructure;

internal static class HttpTestExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<AuthResponse> LoginAsync(this HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            Email = email,
            Password = password
        });

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions))!;
    }

    public static void SetBearerToken(this HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public static async Task<PaginatedResult<FlightDto>> GetFlightsAsync(this HttpClient client)
    {
        var response = await client.GetAsync("/api/v1/flights");
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PaginatedResult<FlightDto>>(JsonOptions))!;
    }

    public static async Task<PaginatedResult<BookingDto>> GetMyBookingsAsync(this HttpClient client)
    {
        var response = await client.GetAsync("/api/v1/bookings/my");
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PaginatedResult<BookingDto>>(JsonOptions))!;
    }

    public static async Task<PaginatedResult<BookingDto>> GetAllBookingsAsync(this HttpClient client)
    {
        var response = await client.GetAsync("/api/v1/bookings");
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PaginatedResult<BookingDto>>(JsonOptions))!;
    }
}
