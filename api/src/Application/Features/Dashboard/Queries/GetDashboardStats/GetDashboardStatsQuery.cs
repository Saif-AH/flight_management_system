using FlightManagementSystem.Application.Features.Dashboard.Common;
using MediatR;

namespace FlightManagementSystem.Application.Features.Dashboard.Queries.GetDashboardStats;

public sealed record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;
