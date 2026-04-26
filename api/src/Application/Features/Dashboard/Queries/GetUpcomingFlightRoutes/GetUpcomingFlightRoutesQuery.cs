using FlightManagementSystem.Application.Features.Dashboard.Common;
using MediatR;

namespace FlightManagementSystem.Application.Features.Dashboard.Queries.GetUpcomingFlightRoutes;

public sealed record GetUpcomingFlightRoutesQuery(int Limit = 5) : IRequest<IReadOnlyList<UpcomingFlightRouteDto>>;
