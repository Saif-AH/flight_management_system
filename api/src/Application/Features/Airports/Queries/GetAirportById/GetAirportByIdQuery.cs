using FlightManagementSystem.Application.Features.Airports.Common;
using MediatR;

namespace FlightManagementSystem.Application.Features.Airports.Queries.GetAirportById;

public sealed record GetAirportByIdQuery(Guid Id) : IRequest<AirportDto>;
