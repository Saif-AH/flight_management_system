namespace FlightManagementSystem.Application.Common.Interfaces;

public interface IIpLookupService
{
    Task<string> GetIpInfoAsync(CancellationToken cancellationToken);
}