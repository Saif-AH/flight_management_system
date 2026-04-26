using FlightManagementSystem.Application.Common.Models;
using FlightManagementSystem.Application.Features.Bookings.Common;
using FlightManagementSystem.Domain.Enums;
using MediatR;

namespace FlightManagementSystem.Application.Features.Bookings.Queries.GetMyBookings;

public sealed record GetMyBookingsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    BookingStatus? Status = null) : IRequest<PaginatedResult<BookingDto>>;
