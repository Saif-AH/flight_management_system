using FlightManagementSystem.Domain.Enums;

namespace FlightManagementSystem.Application.Features.Bookings.Common;

public sealed class BookingDto
{
    public Guid Id { get; set; }
    public Guid FlightId { get; set; }
    public string FlightNumber { get; set; } = default!;
    public string PassengerName { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public int SeatsBooked { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime BookedAtUtc { get; set; }
    public DateTime DepartureTimeUtc { get; set; }
    public DateTime ArrivalTimeUtc { get; set; }
    public string DepartureAirportCode { get; set; } = default!;
    public string ArrivalAirportCode { get; set; } = default!;
}
