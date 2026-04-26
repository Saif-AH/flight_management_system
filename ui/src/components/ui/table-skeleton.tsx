import { Skeleton } from './skeleton';

export function TableSkeleton({ rows = 6, columns = 5 }: { rows?: number; columns?: number }) {
  return <div className="space-y-3">{Array.from({ length: rows }).map((_, row) => <div key={row} className="grid gap-3" style={{ gridTemplateColumns: `repeat(${columns}, minmax(0, 1fr))` }}>{Array.from({ length: columns }).map((__, column) => <Skeleton key={column} className="h-10" />)}</div>)}</div>;
}
