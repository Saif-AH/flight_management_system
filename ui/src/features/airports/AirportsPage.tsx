import * as React from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Edit, Plus, Trash2 } from 'lucide-react';
import { airportsApi } from '@/api/endpoints';
import { getFieldErrors, getProblemMessage } from '@/api/http';
import { AlertDialog, Dialog } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { EmptyState } from '@/components/ui/empty-state';
import { Input } from '@/components/ui/input';
import { Pagination } from '@/components/ui/pagination';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { TableSkeleton } from '@/components/ui/table-skeleton';
import { useToast } from '@/components/ui/toast';
import type { Airport, AirportFormValues } from '@/types/api';
import { AirportForm } from './AirportForm';

export function AirportsPage() {
  const [page, setPage] = React.useState(1);
  const [search, setSearch] = React.useState('');
  const [editing, setEditing] = React.useState<Airport | undefined>();
  const [deleteTarget, setDeleteTarget] = React.useState<Airport | undefined>();
  const [isFormOpen, setIsFormOpen] = React.useState(false);
  const [serverErrors, setServerErrors] = React.useState<Record<string, string[]>>({});
  const queryClient = useQueryClient();
  const { toast } = useToast();
  const params = { page, pageSize: 10, search, sortBy: 'code', sortDirection: 'asc' as const };
  const airports = useQuery({ queryKey: ['airports', params], queryFn: () => airportsApi.list(params) });

  const saveMutation = useMutation({
    mutationFn: (values: AirportFormValues) => editing ? airportsApi.update(editing.id, values) : airportsApi.create(values),
    onSuccess: async () => { await queryClient.invalidateQueries({ queryKey: ['airports'] }); setIsFormOpen(false); setEditing(undefined); toast({ variant: 'success', title: 'Airport saved' }); },
    onError: (error) => { setServerErrors(getFieldErrors(error)); toast({ variant: 'error', title: 'Could not save airport', description: getProblemMessage(error) }); }
  });
  const deleteMutation = useMutation({
    mutationFn: (id: string) => airportsApi.remove(id),
    onSuccess: async () => { await queryClient.invalidateQueries({ queryKey: ['airports'] }); setDeleteTarget(undefined); toast({ variant: 'success', title: 'Airport deleted' }); },
    onError: (error) => toast({ variant: 'error', title: 'Could not delete airport', description: getProblemMessage(error) })
  });
  const openCreate = () => { setEditing(undefined); setServerErrors({}); setIsFormOpen(true); };
  const openEdit = (airport: Airport) => { setEditing(airport); setServerErrors({}); setIsFormOpen(true); };

  return (
    <div className="space-y-6">
      <div className="flex flex-col justify-between gap-4 sm:flex-row sm:items-center"><div><h2 className="text-3xl font-bold tracking-tight">Airports</h2><p className="text-muted-foreground">Search, create, edit, and soft-delete airports.</p></div><Button onClick={openCreate}><Plus className="h-4 w-4" /> New airport</Button></div>
      <Card><CardHeader><CardTitle>Airport directory</CardTitle></CardHeader><CardContent><div className="mb-4 flex gap-3"><Input placeholder="Search by code, city, country..." value={search} onChange={(e) => { setSearch(e.target.value); setPage(1); }} /></div>{airports.isLoading ? <TableSkeleton columns={6} /> : airports.data?.items.length ? <><Table><TableHeader><TableRow><TableHead>Code</TableHead><TableHead>Name</TableHead><TableHead>City</TableHead><TableHead>Country</TableHead><TableHead>Timezone</TableHead><TableHead className="text-right">Actions</TableHead></TableRow></TableHeader><TableBody>{airports.data.items.map((airport) => <TableRow key={airport.id}><TableCell className="font-medium">{airport.code}</TableCell><TableCell>{airport.name}</TableCell><TableCell>{airport.city}</TableCell><TableCell>{airport.country}</TableCell><TableCell>{airport.timezone}</TableCell><TableCell className="text-right"><Button variant="ghost" size="icon" onClick={() => openEdit(airport)}><Edit className="h-4 w-4" /></Button><Button variant="ghost" size="icon" onClick={() => setDeleteTarget(airport)}><Trash2 className="h-4 w-4 text-destructive" /></Button></TableCell></TableRow>)}</TableBody></Table><Pagination page={page} totalPages={airports.data.totalPages} onPageChange={setPage} /></> : <EmptyState title="No airports found" description="Create an airport or change your search." />}</CardContent></Card>
      <Dialog open={isFormOpen} title={editing ? 'Edit airport' : 'Create airport'} onOpenChange={setIsFormOpen}><AirportForm airport={editing} serverErrors={serverErrors} isSubmitting={saveMutation.isPending} onSubmit={(values) => saveMutation.mutate(values)} onCancel={() => setIsFormOpen(false)} /></Dialog>
      <AlertDialog open={Boolean(deleteTarget)} title="Delete airport?" description={`This will remove ${deleteTarget?.code ?? 'this airport'} from active lists.`} confirmText="Delete" loading={deleteMutation.isPending} onCancel={() => setDeleteTarget(undefined)} onConfirm={() => deleteTarget && deleteMutation.mutate(deleteTarget.id)} />
    </div>
  );
}
