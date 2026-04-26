using Asp.Versioning;
using FlightManagementSystem.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlightManagementSystem.Presentation.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/diagnostics")]
public sealed class DiagnosticsController : ControllerBase
{
    private readonly IIpLookupService _ipLookupService;

    public DiagnosticsController(IIpLookupService ipLookupService)
    {
        _ipLookupService = ipLookupService;
    }

    [HttpGet("ip-info")]
    public async Task<IActionResult> GetIpInfo(CancellationToken cancellationToken)
    {
        var result = await _ipLookupService.GetIpInfoAsync(cancellationToken);

        return Content(result, "application/json");
    }
}