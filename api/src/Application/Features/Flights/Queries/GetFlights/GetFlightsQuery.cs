using FlightManagementSystem.Application.Common.Models;
using FlightManagementSystem.Application.Features.Flights.Common;
using FlightManagementSystem.Domain.Enums;
using MediatR;

namespace FlightManagementSystem.Application.Features.Flights.Queries.GetFlights;

public sealed record GetFlightsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    FlightStatus? Status = null,
    string? SortBy = "departureTimeUtc",
    string? SortOrder = "asc") : IRequest<PaginatedResult<FlightDto>>;
