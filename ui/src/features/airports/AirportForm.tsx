import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { Button } from '@/components/ui/button';
import { FieldErrorText, Label } from '@/components/ui/form-field';
import { Input } from '@/components/ui/input';
import type { Airport, AirportFormValues } from '@/types/api';

const schema = z.object({
  code: z.string().min(3).max(10).transform((v) => v.toUpperCase()),
  name: z.string().min(2),
  city: z.string().min(2),
  country: z.string().min(2),
  timezone: z.string().min(2)
});

export function AirportForm({ airport, serverErrors, isSubmitting, onSubmit, onCancel }: { airport?: Airport; serverErrors: Record<string, string[]>; isSubmitting: boolean; onSubmit: (values: AirportFormValues) => void; onCancel: () => void }) {
  const form = useForm<AirportFormValues>({
    resolver: zodResolver(schema),
    defaultValues: airport ? { code: airport.code, name: airport.name, city: airport.city, country: airport.country, timezone: airport.timezone } : { code: '', name: '', city: '', country: '', timezone: 'UTC' }
  });
  const fieldServerError = (name: keyof AirportFormValues) => serverErrors[name]?.join(', ');
  return (
    <form className="grid gap-4 sm:grid-cols-2" onSubmit={form.handleSubmit(onSubmit)}>
      <div className="space-y-2"><Label htmlFor="code">Code</Label><Input id="code" disabled={isSubmitting} {...form.register('code')} /><FieldErrorText error={form.formState.errors.code ?? fieldServerError('code')} /></div>
      <div className="space-y-2"><Label htmlFor="timezone">Timezone</Label><Input id="timezone" disabled={isSubmitting} {...form.register('timezone')} /><FieldErrorText error={form.formState.errors.timezone ?? fieldServerError('timezone')} /></div>
      <div className="space-y-2 sm:col-span-2"><Label htmlFor="name">Name</Label><Input id="name" disabled={isSubmitting} {...form.register('name')} /><FieldErrorText error={form.formState.errors.name ?? fieldServerError('name')} /></div>
      <div className="space-y-2"><Label htmlFor="city">City</Label><Input id="city" disabled={isSubmitting} {...form.register('city')} /><FieldErrorText error={form.formState.errors.city ?? fieldServerError('city')} /></div>
      <div className="space-y-2"><Label htmlFor="country">Country</Label><Input id="country" disabled={isSubmitting} {...form.register('country')} /><FieldErrorText error={form.formState.errors.country ?? fieldServerError('country')} /></div>
      <div className="flex justify-end gap-3 sm:col-span-2"><Button type="button" variant="outline" onClick={onCancel} disabled={isSubmitting}>Cancel</Button><Button type="submit" disabled={isSubmitting}>{isSubmitting ? 'Saving...' : 'Save airport'}</Button></div>
    </form>
  );
}
