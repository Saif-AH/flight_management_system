# Flight Management System

Full-stack flight management system with a .NET 9 backend API and a React + Vite admin dashboard.

## Tech Stack

- Backend: ASP.NET Core 9 Web API
- Database: PostgreSQL 16
- Frontend: React 19, Vite, TypeScript, Tailwind CSS
- Containers: Docker Compose

## Project Structure

```text
flight_management_system/
├── api/                       # .NET 9 backend
│   ├── FlightManagementSystem.sln
│   └── src/
│       ├── Application/
│       ├── Domain/
│       ├── Infrastructure/
│       └── Presentation/      # Web API, Dockerfile, docker-compose.yml
└── ui/                        # React + Vite frontend
```

## Prerequisites

- Docker Desktop
- Node.js and npm
- .NET 9 SDK, only needed if you want to run the backend without Docker

## Backend: Run With Docker

From the `api` directory, start the .NET backend and PostgreSQL database:

```powershell
cd api
docker compose -f .\src\Presentation\docker-compose.yml up --build
```

The API will be available at:

```text
http://localhost:5105
```

The frontend expects the API base URL to be:

```text
http://localhost:5105/api/v1
```

If you run the Docker command from the repository root instead, use:

```powershell
docker compose -f .\api\src\Presentation\docker-compose.yml up --build
```

To stop the containers:

```powershell
docker compose -f .\src\Presentation\docker-compose.yml down
```

Run that stop command from the `api` directory. Add `-v` if you also want to remove the PostgreSQL volume and reset the database:

```powershell
docker compose -f .\src\Presentation\docker-compose.yml down -v
```

## Frontend: Run React + Vite

Open a second terminal and run the frontend from the `ui` directory:

```powershell
cd ui
npm install
npm run dev
```

The Vite dev server will be available at:

```text
http://localhost:5173
```

The frontend environment file is `ui/.env`:

```env
VITE_API_BASE_URL=http://localhost:5105/api/v1
```

## Useful Frontend Commands

```powershell
npm run dev      # Start Vite development server
npm run build    # Type-check and build for production
npm run lint     # Run ESLint
npm run preview  # Preview the production build locally
```

## Development Flow

1. Start the backend and database with Docker from `api`.
2. Start the frontend with Vite from `ui`.
3. Open `http://localhost:5173` in the browser.
4. Log in to access the protected dashboard routes.

## Notes

- Dashboard routes are protected and redirect unauthenticated users to `/login`.
- The frontend automatically attaches the access token to API requests.
- If an API request returns `401`, the frontend attempts token refresh. If refresh fails, tokens are cleared and the user is redirected to `/login`.
