import * as React from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { XCircle } from 'lucide-react';
import { bookingsApi } from '@/api/endpoints';
import { getProblemMessage } from '@/api/http';
import { AlertDialog } from '@/components/ui/dialog';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { EmptyState } from '@/components/ui/empty-state';
import { Input } from '@/components/ui/input';
import { Pagination } from '@/components/ui/pagination';
import { Select } from '@/components/ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { TableSkeleton } from '@/components/ui/table-skeleton';
import { useToast } from '@/components/ui/toast';
import { formatDateTime } from '@/lib/utils';
import type { Booking, BookingStatus } from '@/types/api';

const statuses: BookingStatus[] = ['Active', 'Cancelled'];

export function BookingsPage() {
  const [page, setPage] = React.useState(1);
  const [status, setStatus] = React.useState('');
  const [search, setSearch] = React.useState('');
  const [cancelTarget, setCancelTarget] = React.useState<Booking | undefined>();
  const queryClient = useQueryClient();
  const { toast } = useToast();
  const params = { PageNumber: page, PageSize: 10, Search: search || undefined, Status: status || undefined };
  const bookings = useQuery({ queryKey: ['bookings', params], queryFn: () => bookingsApi.list(params) });
  const cancelMutation = useMutation({
    mutationFn: (id: string) => bookingsApi.cancel(id),
    onSuccess: async () => { await queryClient.invalidateQueries({ queryKey: ['bookings'] }); await queryClient.invalidateQueries({ queryKey: ['flights'] }); setCancelTarget(undefined); toast({ variant: 'success', title: 'Reservation cancelled' }); },
    onError: (error) => toast({ variant: 'error', title: 'Could not cancel reservation', description: getProblemMessage(error) })
  });
  const flightLabel = (booking: Booking) => booking.flight?.flightNumber ?? booking.flightId;

  return (
    <div className="space-y-6">
      <div><h2 className="text-3xl font-bold tracking-tight">Bookings</h2><p className="text-muted-foreground">View reservations and cancel any booking as an admin.</p></div>
      <Card><CardHeader><CardTitle>Reservations</CardTitle></CardHeader><CardContent><div className="mb-4 grid gap-3 md:grid-cols-2"><Input placeholder="Search bookings..." value={search} onChange={(e) => { setSearch(e.target.value); setPage(1); }} /><Select value={status} onChange={(e) => { setStatus(e.target.value); setPage(1); }}><option value="">All statuses</option>{statuses.map((item) => <option key={item} value={item}>{item}</option>)}</Select></div>{bookings.isLoading ? <TableSkeleton columns={8} /> : bookings.data?.items.length ? <><Table><TableHeader><TableRow><TableHead>Reservation</TableHead><TableHead>Passenger</TableHead><TableHead>Email</TableHead><TableHead>Flight</TableHead><TableHead>Seats</TableHead><TableHead>Booked</TableHead><TableHead>Status</TableHead><TableHead className="text-right">Actions</TableHead></TableRow></TableHeader><TableBody>{bookings.data.items.map((booking) => <TableRow key={booking.id}><TableCell className="font-medium">{booking.reservationCode}</TableCell><TableCell>{booking.passengerName}</TableCell><TableCell>{booking.passengerEmail}</TableCell><TableCell>{flightLabel(booking)}</TableCell><TableCell>{booking.seats}</TableCell><TableCell>{formatDateTime(booking.bookedAtUtc)}</TableCell><TableCell><Badge variant={booking.status === 'Cancelled' ? 'destructive' : 'secondary'}>{booking.status}</Badge></TableCell><TableCell className="text-right"><Button variant="ghost" size="sm" disabled={booking.status === 'Cancelled'} onClick={() => setCancelTarget(booking)}><XCircle className="h-4 w-4" /> Cancel</Button></TableCell></TableRow>)}</TableBody></Table><Pagination page={page} totalPages={bookings.data.totalPages} onPageChange={setPage} /></> : <EmptyState title="No bookings found" description="Change filters or wait for reservations." />}</CardContent></Card>
      <AlertDialog open={Boolean(cancelTarget)} title="Cancel reservation?" description={`Admin cancellation ignores the 7-day user cancellation rule for ${cancelTarget?.reservationCode ?? 'this booking'}.`} confirmText="Cancel booking" loading={cancelMutation.isPending} onCancel={() => setCancelTarget(undefined)} onConfirm={() => cancelTarget && cancelMutation.mutate(cancelTarget.id)} />
    </div>
  );
}
