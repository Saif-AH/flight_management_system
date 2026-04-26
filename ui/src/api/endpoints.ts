import { http } from './http';
import type { Airport, AirportFormValues, Booking, DashboardStats, Flight, FlightFormValues, LoginRequest, LoginResponse, PagedResult } from '@/types/api';

export type PageParams = { page?: number; pageSize?: number; search?: string; sortBy?: string; sortDirection?: 'asc' | 'desc' };
export type FlightFilters = PageParams & { airportId?: string; fromDate?: string; toDate?: string; status?: string };
export type BookingFilters = PageParams & { status?: string; flightId?: string; email?: string };

type ApiPagedResult<T> = {
  items: T[];
  page?: number;
  pageNumber?: number;
  pageSize: number;
  totalCount: number;
  totalPages?: number;
};

type ApiFlight = {
  id: string;
  flightNumber: string;
  departureAirportId: string;
  departureAirportCode?: string;
  departureAirportName?: string;
  arrivalAirportId: string;
  arrivalAirportCode?: string;
  arrivalAirportName?: string;
  departureTimeUtc: string;
  arrivalTimeUtc: string;
  totalSeats: number;
  availableSeats: number;
  status: Flight['status'];
};

const normalizePagedResult = <T>(data: ApiPagedResult<T>): PagedResult<T> => ({
  items: data.items,
  page: data.page ?? data.pageNumber ?? 1,
  pageSize: data.pageSize,
  totalCount: data.totalCount,
  totalPages: data.totalPages ?? Math.max(Math.ceil(data.totalCount / data.pageSize), 1)
});

const normalizeFlight = (flight: ApiFlight): Flight => ({
  id: flight.id,
  flightNumber: flight.flightNumber,
  originAirportId: flight.departureAirportId,
  destinationAirportId: flight.arrivalAirportId,
  originAirport: {
    id: flight.departureAirportId,
    code: flight.departureAirportCode ?? '',
    name: flight.departureAirportName ?? '',
    city: '',
    country: '',
    timezone: ''
  },
  destinationAirport: {
    id: flight.arrivalAirportId,
    code: flight.arrivalAirportCode ?? '',
    name: flight.arrivalAirportName ?? '',
    city: '',
    country: '',
    timezone: ''
  },
  departureTimeUtc: flight.departureTimeUtc,
  arrivalTimeUtc: flight.arrivalTimeUtc,
  status: flight.status,
  capacity: flight.totalSeats,
  availableSeats: flight.availableSeats,
  bookedSeats: flight.totalSeats - flight.availableSeats
});

const toFlightPayload = (payload: FlightFormValues) => ({
  flightNumber: payload.flightNumber,
  departureAirportId: payload.originAirportId,
  arrivalAirportId: payload.destinationAirportId,
  departureTimeUtc: payload.departureTimeUtc,
  arrivalTimeUtc: payload.arrivalTimeUtc,
  status: payload.status,
  totalSeats: payload.capacity
});

export const authApi = {
  login: async (payload: LoginRequest) => (await http.post<LoginResponse>('/auth/login', payload)).data,
  logout: async () => (await http.post('/auth/logout')).data
};

export const airportsApi = {
  list: async (params: PageParams) => normalizePagedResult((await http.get<ApiPagedResult<Airport>>('/airports', { params })).data),
  create: async (payload: AirportFormValues) => (await http.post<Airport>('/airports', payload)).data,
  update: async (id: string, payload: AirportFormValues) => (await http.put<Airport>(`/airports/${id}`, payload)).data,
  remove: async (id: string) => (await http.delete(`/airports/${id}`)).data
};

export const flightsApi = {
  list: async (params: FlightFilters) => {
    const data = normalizePagedResult((await http.get<ApiPagedResult<ApiFlight>>('/flights', { params })).data);
    return { ...data, items: data.items.map(normalizeFlight) };
  },
  upcoming: async (take = 5) => (await http.get<ApiFlight[]>('/flights/upcoming', { params: { take } })).data.map(normalizeFlight),
  create: async (payload: FlightFormValues) => normalizeFlight((await http.post<ApiFlight>('/flights', toFlightPayload(payload))).data),
  update: async (id: string, payload: FlightFormValues) => normalizeFlight((await http.put<ApiFlight>(`/flights/${id}`, toFlightPayload(payload))).data),
  remove: async (id: string) => (await http.delete(`/flights/${id}`)).data
};

export const bookingsApi = {
  list: async (params: BookingFilters) => normalizePagedResult((await http.get<ApiPagedResult<Booking>>('/bookings', { params })).data),
  cancel: async (id: string) => (await http.post<Booking>(`/bookings/${id}/cancel`, { reason: 'Cancelled by admin' })).data
};

export const dashboardApi = {
  stats: async () => (await http.get<DashboardStats>('/dashboard/stats')).data
};
