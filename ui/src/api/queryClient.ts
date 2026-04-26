import { QueryClient } from '@tanstack/react-query';
import { getProblemMessage } from './http';

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      retry: 1,
      refetchOnWindowFocus: false
    },
    mutations: {
      retry: false,
      onError: (error) => console.error(getProblemMessage(error))
    }
  }
});
