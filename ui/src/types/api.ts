export type ApiProblem = {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  errors?: Record<string, string[]>;
};

export type PagedResult<T> = {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
};

export type SortDirection = 'asc' | 'desc';

export type Airport = {
  id: string;
  code: string;
  name: string;
  city: string;
  country: string;
  isDeleted?: boolean;
};

export type AirportFormValues = Omit<Airport, 'id' | 'isDeleted' | 'timezone'>;

export type FlightStatus = 'Scheduled' | 'Delayed' | 'Cancelled' | 'Departed' | 'Arrived';

export type Flight = {
  id: string;
  flightNumber: string;
  originAirportId: string;
  destinationAirportId: string;
  originAirport?: Airport;
  destinationAirport?: Airport;
  departureTimeUtc: string;
  arrivalTimeUtc: string;
  status: FlightStatus;
  capacity: number;
  availableSeats: number;
  bookedSeats?: number;
};

export type FlightFormValues = {
  flightNumber: string;
  originAirportId: string;
  destinationAirportId: string;
  departureTimeUtc: string;
  arrivalTimeUtc: string;
  status: FlightStatus;
  capacity: number;
};

export type BookingStatus = 'Active' | 'Cancelled';

export type Booking = {
  id: string;
  reservationCode: string;
  passengerName: string;
  passengerEmail: string;
  flightId: string;
  flight?: Flight;
  seats: number;
  status: BookingStatus;
  bookedAtUtc: string;
  cancelledAtUtc?: string | null;
};

export type DashboardStats = {
  totalFlights: number;
  upcomingFlights: number;
  totalAirports: number;
  activeBookings: number;
  scheduledFlights: number;
  delayedFlights: number;
  cancelledFlights: number;
  completedFlights: number;
};

export type LoginRequest = {
  email: string;
  password: string;
};

export type AuthUser = {
  id: string;
  email: string;
  fullName: string;
  roles: string[];
};

export type LoginResponse = {
  userId: string;
  userName: string;
  roles: string[];
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAtUtc: string;
};

export type RefreshResponse = Pick<LoginResponse, 'accessToken' | 'refreshToken' | 'accessTokenExpiresAtUtc'>;
