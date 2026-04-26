import { Link, NavLink, Outlet, useNavigate } from 'react-router';
import { Gauge, LogOut, MapPin, Plane, Ticket } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { useAuth } from '@/features/auth/AuthProvider';
import { cn } from '@/lib/utils';

const navItems = [
  { href: '/', label: 'Dashboard', icon: Gauge },
  { href: '/airports', label: 'Airports', icon: MapPin },
  { href: '/flights', label: 'Flights', icon: Plane },
  { href: '/bookings', label: 'Bookings', icon: Ticket }
];

export function AppLayout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const handleLogout = () => {
    logout();
    navigate('/login', { replace: true });
  };

  return (
    <div className="min-h-screen bg-muted/30">
      <aside className="fixed inset-y-0 left-0 z-40 hidden w-72 border-r bg-background px-4 py-6 lg:block">
        <Link to="/" className="flex items-center gap-3 px-3 text-xl font-bold">
          <Plane className="h-7 w-7 text-primary" />
          Flight Admin
        </Link>
        <nav className="mt-8 space-y-1">
          {navItems.map((item) => (
            <NavLink key={item.href} to={item.href} end={item.href === '/'} className={({ isActive }) => cn('flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium text-muted-foreground hover:bg-accent hover:text-foreground', isActive && 'bg-primary text-primary-foreground hover:bg-primary hover:text-primary-foreground')}>
              <item.icon className="h-4 w-4" />
              {item.label}
            </NavLink>
          ))}
        </nav>
      </aside>
      <div className="lg:pl-72">
        <header className="sticky top-0 z-30 flex h-16 items-center justify-between border-b bg-background/90 px-6 backdrop-blur">
          <div>
            <p className="text-sm text-muted-foreground">Admin dashboard</p>
            <h1 className="font-semibold">Flight Management System</h1>
          </div>
          <div className="flex items-center gap-4">
            <div className="hidden text-right sm:block">
              <p className="text-sm font-medium">{user?.fullName ?? 'Admin'}</p>
              <p className="text-xs text-muted-foreground">{user?.email}</p>
            </div>
            <Button variant="outline" size="sm" onClick={handleLogout}><LogOut className="h-4 w-4" /> Logout</Button>
          </div>
        </header>
        <main className="p-6"><Outlet /></main>
      </div>
    </div>
  );
}
