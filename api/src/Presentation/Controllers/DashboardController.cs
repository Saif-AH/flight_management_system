using Asp.Versioning;
using FlightManagementSystem.Application.Features.Dashboard.Queries.GetDashboardStats;
using FlightManagementSystem.Application.Features.Dashboard.Queries.GetUpcomingFlightRoutes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightManagementSystem.Presentation.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/dashboard")]
[Authorize(Policy = "AdminOnly")]
public sealed class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetDashboardStatsQuery(), cancellationToken));

    [HttpGet("upcoming-flights")]
    public async Task<IActionResult> GetUpcomingFlights(
        [FromQuery] GetUpcomingFlightRoutesQuery query,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(query, cancellationToken));
}
