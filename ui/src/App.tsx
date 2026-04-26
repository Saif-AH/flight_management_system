import { Route, Routes } from 'react-router';
import { AppLayout } from '@/components/layout/AppLayout';
import { ProtectedRoute } from '@/components/layout/ProtectedRoute';
import { LoginPage } from '@/features/auth/LoginPage';
import { AirportsPage } from '@/features/airports/AirportsPage';
import { BookingsPage } from '@/features/bookings/BookingsPage';
import { DashboardPage } from '@/features/dashboard/DashboardPage';
import { FlightsPage } from '@/features/flights/FlightsPage';

export function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route element={<ProtectedRoute />}>
        <Route element={<AppLayout />}>
          <Route index element={<DashboardPage />} />
          <Route path="airports" element={<AirportsPage />} />
          <Route path="flights" element={<FlightsPage />} />
          <Route path="bookings" element={<BookingsPage />} />
        </Route>
      </Route>
    </Routes>
  );
}
