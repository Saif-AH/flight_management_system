import { useQuery } from '@tanstack/react-query';
import { CalendarClock, MapPin, Plane, Ticket } from 'lucide-react';
import { dashboardApi, flightsApi } from '@/api/endpoints';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { EmptyState } from '@/components/ui/empty-state';
import { Skeleton } from '@/components/ui/skeleton';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { formatDateTime } from '@/lib/utils';

export function DashboardPage() {
  const stats = useQuery({ queryKey: ['dashboard', 'stats'], queryFn: dashboardApi.stats });
  const upcoming = useQuery({ queryKey: ['flights', 'upcoming', 5], queryFn: () => flightsApi.upcoming(5) });
  const cards = [
    { title: 'Airports', value: stats.data?.totalAirports, icon: MapPin },
    { title: 'Flights', value: stats.data?.totalFlights, icon: Plane },
    { title: 'Bookings', value: stats.data?.totalBookings, icon: Ticket }
  ];
  return (
    <div className="space-y-6">
      <div><h2 className="text-3xl font-bold tracking-tight">Dashboard</h2><p className="text-muted-foreground">Operational overview and the next 5 departures.</p></div>
      <div className="grid gap-4 md:grid-cols-3">{cards.map((card) => <Card key={card.title}><CardHeader className="flex flex-row items-center justify-between pb-2"><CardTitle className="text-sm font-medium">Total {card.title}</CardTitle><card.icon className="h-4 w-4 text-muted-foreground" /></CardHeader><CardContent>{stats.isLoading ? <Skeleton className="h-8 w-24" /> : <div className="text-3xl font-bold">{card.value ?? 0}</div>}</CardContent></Card>)}</div>
      <Card><CardHeader className="flex flex-row items-center gap-2"><CalendarClock className="h-5 w-5 text-primary" /><CardTitle>Upcoming departures</CardTitle></CardHeader><CardContent>{upcoming.isLoading ? <div className="space-y-3"><Skeleton className="h-10" /><Skeleton className="h-10" /><Skeleton className="h-10" /></div> : upcoming.data?.length ? <Table><TableHeader><TableRow><TableHead>Flight</TableHead><TableHead>Route</TableHead><TableHead>Departure</TableHead><TableHead>Seats</TableHead><TableHead>Status</TableHead></TableRow></TableHeader><TableBody>{upcoming.data.map((flight) => <TableRow key={flight.id}><TableCell className="font-medium">{flight.flightNumber}</TableCell><TableCell>{flight.originAirport?.code || flight.originAirport?.name || '—'} → {flight.destinationAirport?.code || flight.destinationAirport?.name || '—'}</TableCell><TableCell>{formatDateTime(flight.departureTimeUtc)}</TableCell><TableCell>{flight.availableSeats}/{flight.capacity}</TableCell><TableCell><Badge variant="secondary">{flight.status}</Badge></TableCell></TableRow>)}</TableBody></Table> : <EmptyState title="No upcoming departures" />}</CardContent></Card>
    </div>
  );
}
