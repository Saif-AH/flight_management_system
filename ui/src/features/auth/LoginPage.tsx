import { zodResolver } from '@hookform/resolvers/zod';
import { Plane } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { Navigate, useLocation, useNavigate } from 'react-router';
import { z } from 'zod';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { FieldErrorText, Label } from '@/components/ui/form-field';
import { Input } from '@/components/ui/input';
import { useAuth } from './AuthProvider';

const schema = z.object({ email: z.string().email('Enter a valid email'), password: z.string().min(6, 'Password must be at least 6 characters') });
type LoginValues = z.infer<typeof schema>;

type LocationState = { from?: { pathname?: string } };

export function LoginPage() {
  const { isAuthenticated, login, isLoggingIn, loginError } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const state = location.state as LocationState | null;
  const form = useForm<LoginValues>({ resolver: zodResolver(schema), defaultValues: { email: '', password: '' } });

  if (isAuthenticated) return <Navigate to="/" replace />;

  const onSubmit = async (values: LoginValues) => {
    await login(values);
    navigate(state?.from?.pathname ?? '/', { replace: true });
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-muted/40 p-6">
      <Card className="w-full max-w-md">
        <CardHeader className="text-center">
          <div className="mx-auto mb-4 flex h-12 w-12 items-center justify-center rounded-full bg-primary text-primary-foreground"><Plane className="h-6 w-6" /></div>
          <CardTitle className="text-2xl">Admin login</CardTitle>
          <CardDescription>Sign in to manage airports, flights, and reservations.</CardDescription>
        </CardHeader>
        <CardContent>
          <form className="space-y-4" onSubmit={form.handleSubmit(onSubmit)}>
            <div className="space-y-2"><Label htmlFor="email">Email</Label><Input id="email" type="email" autoComplete="email" disabled={isLoggingIn} {...form.register('email')} /><FieldErrorText error={form.formState.errors.email} /></div>
            <div className="space-y-2"><Label htmlFor="password">Password</Label><Input id="password" type="password" autoComplete="current-password" disabled={isLoggingIn} {...form.register('password')} /><FieldErrorText error={form.formState.errors.password} /></div>
            {loginError ? <p className="rounded-md bg-destructive/10 p-3 text-sm text-destructive">{loginError}</p> : null}
            <Button className="w-full" type="submit" disabled={isLoggingIn}>{isLoggingIn ? 'Signing in...' : 'Login'}</Button>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
