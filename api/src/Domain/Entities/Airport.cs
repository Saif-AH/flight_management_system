using FlightManagementSystem.Domain.Common;

namespace FlightManagementSystem.Domain.Entities;

public class Airport : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!; // e.g. LHR
    public string City { get; set; } = default!;
    public string Country { get; set; } = default!;

    public ICollection<Flight> DepartingFlights { get; set; } = new List<Flight>();
    public ICollection<Flight> ArrivingFlights { get; set; } = new List<Flight>();
}