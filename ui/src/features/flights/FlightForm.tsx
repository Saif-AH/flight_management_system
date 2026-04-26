import { zodResolver } from '@hookform/resolvers/zod';
import { useQuery } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { airportsApi } from '@/api/endpoints';
import { Button } from '@/components/ui/button';
import { FieldErrorText, Label } from '@/components/ui/form-field';
import { Input } from '@/components/ui/input';
import { Select } from '@/components/ui/select';
import type { Flight, FlightFormValues, FlightStatus } from '@/types/api';

const statuses: FlightStatus[] = ['Scheduled', 'Delayed', 'Cancelled', 'Departed', 'Arrived'];
const schema = z.object({
  flightNumber: z.string().min(2),
  originAirportId: z.string().min(1, 'Origin is required'),
  destinationAirportId: z.string().min(1, 'Destination is required'),
  departureTimeUtc: z.string().min(1),
  arrivalTimeUtc: z.string().min(1),
  status: z.enum(['Scheduled', 'Delayed', 'Cancelled', 'Departed', 'Arrived']),
  capacity: z.coerce.number().int().positive()
}).refine((v) => v.originAirportId !== v.destinationAirportId, { message: 'Origin and destination must be different', path: ['destinationAirportId'] });

function toLocalInput(value?: string) {
  if (!value) return '';
  const date = new Date(value);
  return new Date(date.getTime() - date.getTimezoneOffset() * 60000).toISOString().slice(0, 16);
}

function toUtc(value: string) {
  return new Date(value).toISOString();
}

export function FlightForm({ flight, serverErrors, isSubmitting, onSubmit, onCancel }: { flight?: Flight; serverErrors: Record<string, string[]>; isSubmitting: boolean; onSubmit: (values: FlightFormValues) => void; onCancel: () => void }) {
  const airports = useQuery({ queryKey: ['airports', 'select'], queryFn: () => airportsApi.list({ page: 1, pageSize: 100, sortBy: 'code', sortDirection: 'asc' }) });
  const form = useForm<FlightFormValues>({
    resolver: zodResolver(schema.transform((v) => ({ ...v, departureTimeUtc: toUtc(v.departureTimeUtc), arrivalTimeUtc: toUtc(v.arrivalTimeUtc) }))),
    defaultValues: flight ? { flightNumber: flight.flightNumber, originAirportId: flight.originAirportId, destinationAirportId: flight.destinationAirportId, departureTimeUtc: toLocalInput(flight.departureTimeUtc), arrivalTimeUtc: toLocalInput(flight.arrivalTimeUtc), status: flight.status, capacity: flight.capacity } : { flightNumber: '', originAirportId: '', destinationAirportId: '', departureTimeUtc: '', arrivalTimeUtc: '', status: 'Scheduled', capacity: 180 }
  });
  const fieldServerError = (name: keyof FlightFormValues) => serverErrors[name]?.join(', ');
  const options = airports.data?.items ?? [];
  return (
    <form className="grid gap-4 sm:grid-cols-2" onSubmit={form.handleSubmit(onSubmit)}>
      <div className="space-y-2"><Label htmlFor="flightNumber">Flight number</Label><Input id="flightNumber" disabled={isSubmitting} {...form.register('flightNumber')} /><FieldErrorText error={form.formState.errors.flightNumber ?? fieldServerError('flightNumber')} /></div>
      <div className="space-y-2"><Label htmlFor="status">Status</Label><Select id="status" disabled={isSubmitting} {...form.register('status')}>{statuses.map((s) => <option key={s} value={s}>{s}</option>)}</Select><FieldErrorText error={form.formState.errors.status ?? fieldServerError('status')} /></div>
      <div className="space-y-2"><Label htmlFor="originAirportId">Origin</Label><Select id="originAirportId" disabled={isSubmitting || airports.isLoading} {...form.register('originAirportId')}><option value="">Select origin</option>{options.map((airport) => <option key={airport.id} value={airport.id}>{airport.code} - {airport.city}</option>)}</Select><FieldErrorText error={form.formState.errors.originAirportId ?? fieldServerError('originAirportId')} /></div>
      <div className="space-y-2"><Label htmlFor="destinationAirportId">Destination</Label><Select id="destinationAirportId" disabled={isSubmitting || airports.isLoading} {...form.register('destinationAirportId')}><option value="">Select destination</option>{options.map((airport) => <option key={airport.id} value={airport.id}>{airport.code} - {airport.city}</option>)}</Select><FieldErrorText error={form.formState.errors.destinationAirportId ?? fieldServerError('destinationAirportId')} /></div>
      <div className="space-y-2"><Label htmlFor="departureTimeUtc">Departure</Label><Input id="departureTimeUtc" type="datetime-local" disabled={isSubmitting} {...form.register('departureTimeUtc')} /><FieldErrorText error={form.formState.errors.departureTimeUtc ?? fieldServerError('departureTimeUtc')} /></div>
      <div className="space-y-2"><Label htmlFor="arrivalTimeUtc">Arrival</Label><Input id="arrivalTimeUtc" type="datetime-local" disabled={isSubmitting} {...form.register('arrivalTimeUtc')} /><FieldErrorText error={form.formState.errors.arrivalTimeUtc ?? fieldServerError('arrivalTimeUtc')} /></div>
      <div className="space-y-2"><Label htmlFor="capacity">Capacity</Label><Input id="capacity" type="number" min="1" disabled={isSubmitting} {...form.register('capacity')} /><FieldErrorText error={form.formState.errors.capacity ?? fieldServerError('capacity')} /></div>
      <div className="flex justify-end gap-3 sm:col-span-2"><Button type="button" variant="outline" onClick={onCancel} disabled={isSubmitting}>Cancel</Button><Button type="submit" disabled={isSubmitting}>{isSubmitting ? 'Saving...' : 'Save flight'}</Button></div>
    </form>
  );
}
