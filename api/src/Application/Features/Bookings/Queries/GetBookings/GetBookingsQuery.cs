using FlightManagementSystem.Application.Common.Models;
using FlightManagementSystem.Application.Features.Bookings.Common;
using FlightManagementSystem.Domain.Enums;
using MediatR;

namespace FlightManagementSystem.Application.Features.Bookings.Queries.GetBookings;

public sealed record GetBookingsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    BookingStatus? Status = null) : IRequest<PaginatedResult<BookingDto>>;
