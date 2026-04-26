using FlightManagementSystem.Application.Common.Models;
using FlightManagementSystem.Application.Features.Airports.Common;
using MediatR;

namespace FlightManagementSystem.Application.Features.Airports.Queries.GetAirports;

public sealed record GetAirportsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    string? SortBy = "name",
    string? SortOrder = "asc") : IRequest<PaginatedResult<AirportDto>>;