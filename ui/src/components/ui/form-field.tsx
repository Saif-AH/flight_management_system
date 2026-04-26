import type { FieldError } from 'react-hook-form';

export function FieldErrorText({ error }: { error?: FieldError | string }) {
  if (!error) return null;
  const message = typeof error === 'string' ? error : error.message;
  return message ? <p className="mt-1 text-sm text-destructive">{message}</p> : null;
}

export function Label({ htmlFor, children }: { htmlFor?: string; children: React.ReactNode }) {
  return <label htmlFor={htmlFor} className="text-sm font-medium leading-none">{children}</label>;
}
