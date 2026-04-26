using FlightManagementSystem.Domain.Common;
using FlightManagementSystem.Domain.Enums;

namespace FlightManagementSystem.Domain.Entities;

public class Booking : BaseEntity
{
    public Guid FlightId { get; set; }
    public Flight Flight { get; set; } = default!;

    public string UserId { get; set; } = default!;
    public string PassengerName { get; set; } = default!;

    public int SeatsBooked { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Active;
    public DateTime BookedAtUtc { get; set; } = DateTime.UtcNow;

    public bool CanBeCancelledByUser(DateTime nowUtc)
        => Status == BookingStatus.Active &&
           Flight.DepartureTimeUtc.Subtract(nowUtc).TotalDays >= 7;

    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Booking already cancelled.");

        Status = BookingStatus.Cancelled;
    }
}