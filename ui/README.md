# Flight Admin Dashboard

React admin dashboard for the Flight Management System assignment.

## Stack

- Vite
- React 19 + TypeScript
- React Router
- Tailwind CSS with shadcn/ui-style components
- TanStack Query for `useQuery` and `useMutation`
- Axios with JWT bearer token and refresh-token retry on `401`
- React Hook Form + Zod validation

## Run locally

```bash
cp .env.example .env
npm install
npm run dev
```

The app expects the API at `VITE_API_BASE_URL`, defaulting to `http://localhost:5000/api/v1`.

## Implemented routes

- `/login` - admin login; redirects to dashboard after success.
- `/` - stat cards and next 5 upcoming departures.
- `/airports` - searchable, paginated table; create, edit, delete with confirmation.
- `/flights` - airport/date/status filters; create, edit, delete; seat availability display.
- `/bookings` - reservations list with filters; admin cancellation ignores the 7-day user rule.

## Backend endpoint assumptions

The frontend assumes REST endpoints under `/api/v1`:

- `POST /auth/login`
- `POST /auth/refresh`
- `POST /auth/logout`
- `GET/POST /airports`, `PUT/DELETE /airports/{id}`
- `GET/POST /flights`, `PUT/DELETE /flights/{id}`, `GET /flights/upcoming?take=5`
- `GET /bookings`, `POST /bookings/{id}/cancel`
- `GET /dashboard/stats`

List endpoints should return:

```ts
{
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
```

Server validation errors are displayed inline when returned as RFC 7807 `errors`.
