import * as React from 'react';
import { cn } from '@/lib/utils';

type ToastVariant = 'success' | 'error' | 'info';
type Toast = { id: string; title: string; description?: string; variant: ToastVariant };
type ToastContextValue = { toast: (toast: Omit<Toast, 'id'>) => void };

const ToastContext = React.createContext<ToastContextValue | undefined>(undefined);

export function ToastProvider({ children }: { children: React.ReactNode }) {
  const [toasts, setToasts] = React.useState<Toast[]>([]);
  const toast = React.useCallback((payload: Omit<Toast, 'id'>) => {
    const id = crypto.randomUUID();
    setToasts((prev) => [...prev, { ...payload, id }]);
    window.setTimeout(() => setToasts((prev) => prev.filter((item) => item.id !== id)), 4500);
  }, []);

  return (
    <ToastContext.Provider value={{ toast }}>
      {children}
      <div className="fixed right-4 top-4 z-[60] flex w-96 max-w-[calc(100vw-2rem)] flex-col gap-3">
        {toasts.map((item) => (
          <div key={item.id} className={cn('rounded-lg border bg-background p-4 shadow-lg', item.variant === 'success' && 'border-green-200', item.variant === 'error' && 'border-destructive/40')}>
            <div className="font-medium">{item.title}</div>
            {item.description ? <div className="mt-1 text-sm text-muted-foreground">{item.description}</div> : null}
          </div>
        ))}
      </div>
    </ToastContext.Provider>
  );
}

export function useToast() {
  const context = React.useContext(ToastContext);
  if (!context) throw new Error('useToast must be used within ToastProvider');
  return context;
}
