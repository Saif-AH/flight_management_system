using System.Net;
using System.Net.Http.Json;
using FlightManagementSystem.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace FlightManagementSystem.IntegrationTests.Bookings;

[Collection(IntegrationTestCollection.Name)]
public sealed class BookingEndpointsTests
{
    private readonly ApiWebApplicationFactory _factory;

    public BookingEndpointsTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateBooking_ThenGetMyBookings_ShouldReturnCreatedBooking()
    {
        if (!_factory.RequireDocker())
            return;

        await _factory.ResetDatabaseAsync();
        using var client = _factory.CreateClient();

        var auth = await client.LoginAsync("user@flight.com", "User123!");
        client.SetBearerToken(auth.AccessToken);

        var flight = (await client.GetFlightsAsync()).Items.Single();

        var createResponse = await client.PostAsJsonAsync("/api/v1/bookings", new
        {
            FlightId = flight.Id,
            PassengerName = "Integration User",
            SeatsBooked = 2
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var myBookings = await client.GetMyBookingsAsync();

        myBookings.Items.Should().ContainSingle(x =>
            x.FlightId == flight.Id &&
            x.PassengerName == "Integration User" &&
            x.SeatsBooked == 2);
    }

    [Fact]
    public async Task GetBookings_ShouldReturnAllBookings_ForAdmin()
    {
        if (!_factory.RequireDocker())
            return;

        await _factory.ResetDatabaseAsync();

        using var userClient = _factory.CreateClient();
        var userAuth = await userClient.LoginAsync("user@flight.com", "User123!");
        userClient.SetBearerToken(userAuth.AccessToken);
        var flight = (await userClient.GetFlightsAsync()).Items.Single();
        await userClient.PostAsJsonAsync("/api/v1/bookings", new
        {
            FlightId = flight.Id,
            PassengerName = "Integration User",
            SeatsBooked = 1
        });

        using var adminClient = _factory.CreateClient();
        var adminAuth = await adminClient.LoginAsync("admin@flight.com", "Admin123!");
        adminClient.SetBearerToken(adminAuth.AccessToken);

        var allBookings = await adminClient.GetAllBookingsAsync();

        allBookings.Items.Should().Contain(x =>
            x.PassengerName == "Integration User" &&
            x.FlightId == flight.Id);
    }

    [Fact]
    public async Task CancelBooking_ShouldMarkBookingCancelled()
    {
        if (!_factory.RequireDocker())
            return;

        await _factory.ResetDatabaseAsync();
        using var client = _factory.CreateClient();

        var auth = await client.LoginAsync("user@flight.com", "User123!");
        client.SetBearerToken(auth.AccessToken);

        var flight = (await client.GetFlightsAsync()).Items.Single();
        await client.PostAsJsonAsync("/api/v1/bookings", new
        {
            FlightId = flight.Id,
            PassengerName = "Integration User",
            SeatsBooked = 1
        });

        var booking = (await client.GetMyBookingsAsync()).Items.Single();

        var cancelResponse = await client.PostAsync($"/api/v1/bookings/{booking.Id}/cancel", null);

        cancelResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var myBookingsAfterCancellation = await client.GetMyBookingsAsync();
        myBookingsAfterCancellation.Items.Single(x => x.Id == booking.Id).Status.ToString().Should().Be("Cancelled");
    }
}
