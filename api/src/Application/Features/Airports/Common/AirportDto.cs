namespace FlightManagementSystem.Application.Features.Airports.Common;

public sealed class AirportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Country { get; set; } = default!;
}