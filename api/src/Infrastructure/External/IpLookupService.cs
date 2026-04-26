using FlightManagementSystem.Application.Common.Interfaces;

namespace FlightManagementSystem.Infrastructure.External;

public sealed class IpLookupService : IIpLookupService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IpLookupService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> GetIpInfoAsync(CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("IpApiClient");
        var response = await client.GetAsync("json/8.8.8.8", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}