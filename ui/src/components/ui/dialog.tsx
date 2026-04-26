import * as React from 'react';
import { createPortal } from 'react-dom';
import { X } from 'lucide-react';
import { Button } from './button';
import { cn } from '@/lib/utils';

type DialogProps = {
  open: boolean;
  title: string;
  description?: string;
  onOpenChange: (open: boolean) => void;
  children: React.ReactNode;
};

function useBodyScrollLock(open: boolean) {
  React.useEffect(() => {
    if (!open) return;
    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = 'hidden';
    return () => {
      document.body.style.overflow = previousOverflow;
    };
  }, [open]);
}

export function Dialog({ open, title, description, onOpenChange, children }: DialogProps) {
  useBodyScrollLock(open);
  if (!open) return null;
  return createPortal(
    <div className="fixed inset-0 z-[100]">
      <div className="fixed inset-0 bg-black/50" aria-hidden="true" />
      <div className="fixed inset-0 z-[101] flex min-h-screen items-center justify-center overflow-y-auto p-4" role="dialog" aria-modal="true" onMouseDown={() => onOpenChange(false)}>
        <div className="w-full max-w-2xl overflow-hidden rounded-xl border bg-background shadow-lg" onMouseDown={(event) => event.stopPropagation()}>
          <div className="flex items-start justify-between gap-4 border-b p-6">
            <div>
              <h2 className="text-lg font-semibold">{title}</h2>
              {description ? <p className="mt-1 text-sm text-muted-foreground">{description}</p> : null}
            </div>
            <Button type="button" variant="ghost" size="icon" onClick={() => onOpenChange(false)} aria-label="Close dialog">
              <X className="h-4 w-4" />
            </Button>
          </div>
          <div className="max-h-[calc(100vh-10rem)] overflow-y-auto p-6">{children}</div>
        </div>
      </div>
    </div>,
    document.body
  );
}

type AlertDialogProps = {
  open: boolean;
  title: string;
  description: string;
  confirmText?: string;
  loading?: boolean;
  onConfirm: () => void;
  onCancel: () => void;
};

export function AlertDialog({ open, title, description, confirmText = 'Confirm', loading = false, onConfirm, onCancel }: AlertDialogProps) {
  useBodyScrollLock(open);
  if (!open) return null;
  return createPortal(
    <div className="fixed inset-0 z-[100]">
      <div className="fixed inset-0 bg-black/50" aria-hidden="true" />
      <div className="fixed inset-0 z-[101] flex min-h-screen items-center justify-center overflow-y-auto p-4" role="alertdialog" aria-modal="true" onMouseDown={onCancel}>
        <div className={cn('w-full max-w-md rounded-xl border bg-background p-6 shadow-lg')} onMouseDown={(event) => event.stopPropagation()}>
          <h2 className="text-lg font-semibold">{title}</h2>
          <p className="mt-2 text-sm text-muted-foreground">{description}</p>
          <div className="mt-6 flex justify-end gap-3">
            <Button type="button" variant="outline" onClick={onCancel} disabled={loading}>Cancel</Button>
            <Button type="button" variant="destructive" onClick={onConfirm} disabled={loading}>{loading ? 'Working...' : confirmText}</Button>
          </div>
        </div>
      </div>
    </div>,
    document.body
  );
}
