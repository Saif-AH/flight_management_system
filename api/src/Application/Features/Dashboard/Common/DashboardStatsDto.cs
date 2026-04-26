namespace FlightManagementSystem.Application.Features.Dashboard.Common;

public sealed class DashboardStatsDto
{
    public int TotalFlights { get; set; }
    public int UpcomingFlights { get; set; }
    public int TotalAirports { get; set; }
    public int ActiveBookings { get; set; }
    public int ScheduledFlights { get; set; }
    public int DelayedFlights { get; set; }
    public int CancelledFlights { get; set; }
    public int CompletedFlights { get; set; }
}
