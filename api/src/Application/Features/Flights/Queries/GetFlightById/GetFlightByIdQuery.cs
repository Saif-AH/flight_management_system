using FlightManagementSystem.Application.Features.Flights.Common;
using MediatR;

namespace FlightManagementSystem.Application.Features.Flights.Queries.GetFlightById;

public sealed record GetFlightByIdQuery(Guid Id) : IRequest<FlightDto>;
