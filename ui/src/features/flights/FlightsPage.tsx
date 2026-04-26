import * as React from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Edit, Plus, Trash2 } from 'lucide-react';
import { airportsApi, flightsApi } from '@/api/endpoints';
import { getFieldErrors, getProblemMessage } from '@/api/http';
import { AlertDialog, Dialog } from '@/components/ui/dialog';
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
import type { Flight, FlightFormValues, FlightStatus } from '@/types/api';
import { FlightForm } from './FlightForm';

const statuses: FlightStatus[] = ['Scheduled', 'Delayed', 'Cancelled', 'Departed', 'Arrived'];

export function FlightsPage() {
  const [page, setPage] = React.useState(1);
  const [airportId, setAirportId] = React.useState('');
  const [status, setStatus] = React.useState('');
  const [fromDate, setFromDate] = React.useState('');
  const [toDate, setToDate] = React.useState('');
  const [editing, setEditing] = React.useState<Flight | undefined>();
  const [deleteTarget, setDeleteTarget] = React.useState<Flight | undefined>();
  const [isFormOpen, setIsFormOpen] = React.useState(false);
  const [serverErrors, setServerErrors] = React.useState<Record<string, string[]>>({});
  const queryClient = useQueryClient();
  const { toast } = useToast();
  const params = { page, pageSize: 10, airportId, status, fromDate: fromDate ? new Date(fromDate).toISOString() : undefined, toDate: toDate ? new Date(toDate).toISOString() : undefined, sortBy: 'departureTimeUtc', sortDirection: 'asc' as const };
  const flights = useQuery({ queryKey: ['flights', params], queryFn: () => flightsApi.list(params) });
  const airports = useQuery({ queryKey: ['airports', 'filters'], queryFn: () => airportsApi.list({ page: 1, pageSize: 100, sortBy: 'code', sortDirection: 'asc' }) });
  const saveMutation = useMutation({
    mutationFn: (values: FlightFormValues) => editing ? flightsApi.update(editing.id, values) : flightsApi.create(values),
    onSuccess: async () => { await queryClient.invalidateQueries({ queryKey: ['flights'] }); setIsFormOpen(false); setEditing(undefined); toast({ variant: 'success', title: 'Flight saved' }); },
    onError: (error) => { setServerErrors(getFieldErrors(error)); toast({ variant: 'error', title: 'Could not save flight', description: getProblemMessage(error) }); }
  });
  const deleteMutation = useMutation({
    mutationFn: (id: string) => flightsApi.remove(id),
    onSuccess: async () => { await queryClient.invalidateQueries({ queryKey: ['flights'] }); setDeleteTarget(undefined); toast({ variant: 'success', title: 'Flight deleted' }); },
    onError: (error) => toast({ variant: 'error', title: 'Could not delete flight', description: getProblemMessage(error) })
  });
  const openCreate = () => { setEditing(undefined); setServerErrors({}); setIsFormOpen(true); };
  const openEdit = (flight: Flight) => { setEditing(flight); setServerErrors({}); setIsFormOpen(true); };
  const airportLabel = (flight: Flight, field: 'originAirport' | 'destinationAirport') => flight[field]?.code || flight[field]?.name || '—';

  return (
    <div className="space-y-6">
      <div className="flex flex-col justify-between gap-4 sm:flex-row sm:items-center"><div><h2 className="text-3xl font-bold tracking-tight">Flights</h2><p className="text-muted-foreground">Filter schedules, maintain flights, and monitor seat availability.</p></div><Button onClick={openCreate}><Plus className="h-4 w-4" /> New flight</Button></div>
      <Card><CardHeader><CardTitle>Flight schedule</CardTitle></CardHeader><CardContent><div className="mb-4 grid gap-3 md:grid-cols-4"><Select value={airportId} onChange={(e) => { setAirportId(e.target.value); setPage(1); }}><option value="">All airports</option>{airports.data?.items.map((airport) => <option key={airport.id} value={airport.id}>{airport.code} - {airport.city}</option>)}</Select><Select value={status} onChange={(e) => { setStatus(e.target.value); setPage(1); }}><option value="">All statuses</option>{statuses.map((item) => <option key={item} value={item}>{item}</option>)}</Select><Input type="date" value={fromDate} onChange={(e) => { setFromDate(e.target.value); setPage(1); }} /><Input type="date" value={toDate} onChange={(e) => { setToDate(e.target.value); setPage(1); }} /></div>{flights.isLoading ? <TableSkeleton columns={7} /> : flights.data?.items.length ? <><Table><TableHeader><TableRow><TableHead>Flight</TableHead><TableHead>Route</TableHead><TableHead>Departure</TableHead><TableHead>Arrival</TableHead><TableHead>Status</TableHead><TableHead>Seats</TableHead><TableHead className="text-right">Actions</TableHead></TableRow></TableHeader><TableBody>{flights.data.items.map((flight) => <TableRow key={flight.id}><TableCell className="font-medium">{flight.flightNumber}</TableCell><TableCell>{airportLabel(flight, 'originAirport')} → {airportLabel(flight, 'destinationAirport')}</TableCell><TableCell>{formatDateTime(flight.departureTimeUtc)}</TableCell><TableCell>{formatDateTime(flight.arrivalTimeUtc)}</TableCell><TableCell><Badge variant={flight.status === 'Cancelled' ? 'destructive' : 'secondary'}>{flight.status}</Badge></TableCell><TableCell>{flight.availableSeats}/{flight.capacity}</TableCell><TableCell className="text-right"><Button variant="ghost" size="icon" onClick={() => openEdit(flight)}><Edit className="h-4 w-4" /></Button><Button variant="ghost" size="icon" onClick={() => setDeleteTarget(flight)}><Trash2 className="h-4 w-4 text-destructive" /></Button></TableCell></TableRow>)}</TableBody></Table><Pagination page={page} totalPages={flights.data.totalPages} onPageChange={setPage} /></> : <EmptyState title="No flights found" description="Create a flight or change the filters." />}</CardContent></Card>
      <Dialog open={isFormOpen} title={editing ? 'Edit flight' : 'Create flight'} onOpenChange={setIsFormOpen}><FlightForm flight={editing} serverErrors={serverErrors} isSubmitting={saveMutation.isPending} onSubmit={(values) => saveMutation.mutate(values)} onCancel={() => setIsFormOpen(false)} /></Dialog>
      <AlertDialog open={Boolean(deleteTarget)} title="Delete flight?" description={`This will remove flight ${deleteTarget?.flightNumber ?? ''}.`} confirmText="Delete" loading={deleteMutation.isPending} onCancel={() => setDeleteTarget(undefined)} onConfirm={() => deleteTarget && deleteMutation.mutate(deleteTarget.id)} />
    </div>
  );
}
