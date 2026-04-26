using FlightManagementSystem.Domain.Enums;

namespace FlightManagementSystem.Application.Features.Dashboard.Common;

public sealed class UpcomingFlightRouteDto
{
    public Guid FlightId { get; set; }
    public string FlightNumber { get; set; } = default!;
    public string DepartureAirportCode { get; set; } = default!;
    public string DepartureAirportName { get; set; } = default!;
    public string ArrivalAirportCode { get; set; } = default!;
    public string ArrivalAirportName { get; set; } = default!;
    public string Route { get; set; } = default!;
    public DateTime DepartureTimeUtc { get; set; }
    public DateTime ArrivalTimeUtc { get; set; }
    public FlightStatus Status { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
}
