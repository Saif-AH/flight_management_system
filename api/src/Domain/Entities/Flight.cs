using FlightManagementSystem.Domain.Common;
using FlightManagementSystem.Domain.Enums;

namespace FlightManagementSystem.Domain.Entities;

public class Flight : BaseEntity
{
    public string FlightNumber { get; set; } = default!;

    public Guid DepartureAirportId { get; set; }
    public Airport DepartureAirport { get; set; } = default!;

    public Guid ArrivalAirportId { get; set; }
    public Airport ArrivalAirport { get; set; } = default!;

    public DateTime DepartureTimeUtc { get; set; }
    public DateTime ArrivalTimeUtc { get; set; }

    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }

    public FlightStatus Status { get; set; } = FlightStatus.Scheduled;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}