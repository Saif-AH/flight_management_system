using FlightManagementSystem.Domain.Enums;

namespace FlightManagementSystem.Application.Features.Flights.Common;

public sealed class FlightDto
{
    public Guid Id { get; set; }
    public string FlightNumber { get; set; } = default!;
    public Guid DepartureAirportId { get; set; }
    public string DepartureAirportCode { get; set; } = default!;
    public string DepartureAirportName { get; set; } = default!;
    public Guid ArrivalAirportId { get; set; }
    public string ArrivalAirportCode { get; set; } = default!;
    public string ArrivalAirportName { get; set; } = default!;
    public DateTime DepartureTimeUtc { get; set; }
    public DateTime ArrivalTimeUtc { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public FlightStatus Status { get; set; }
}
