import axios, { AxiosError, type InternalAxiosRequestConfig } from 'axios';
import { tokenStorage } from '@/lib/storage';
import type { ApiProblem, RefreshResponse } from '@/types/api';

const baseURL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5105/api/v1';

export const http = axios.create({ baseURL, headers: { 'Content-Type': 'application/json' } });

type RetryConfig = InternalAxiosRequestConfig & { _retry?: boolean };
let refreshPromise: Promise<RefreshResponse> | null = null;
let isRedirectingToLogin = false;

function logoutAndRedirectToLogin() {
  tokenStorage.clear();
  if (isRedirectingToLogin || window.location.pathname === '/login') return;
  isRedirectingToLogin = true;
  window.location.assign('/login');
}

http.interceptors.request.use((config) => {
  const token = tokenStorage.getAccessToken();
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

http.interceptors.response.use(
  (response) => response,
  async (error: AxiosError<ApiProblem>) => {
    const original = error.config as RetryConfig | undefined;
    if (error.response?.status !== 401 || !original || original._retry || original.url?.includes('/auth/refresh')) {
      return Promise.reject(error);
    }
    const refreshToken = tokenStorage.getRefreshToken();
    if (!refreshToken) {
      logoutAndRedirectToLogin();
      return Promise.reject(error);
    }
    original._retry = true;
    refreshPromise ??= axios
      .post<RefreshResponse>(`${baseURL}/auth/refresh`, { refreshToken })
      .then((res) => {
        tokenStorage.setTokens(res.data);
        return res.data;
      })
      .finally(() => { refreshPromise = null; });

    try {
      const refreshed = await refreshPromise;
      original.headers.Authorization = `Bearer ${refreshed.accessToken}`;
      return http(original);
    } catch (refreshError) {
      logoutAndRedirectToLogin();
      return Promise.reject(refreshError);
    }
  }
);

export function getProblemMessage(error: unknown) {
  if (axios.isAxiosError<ApiProblem>(error)) {
    const problem = error.response?.data;
    return problem?.detail ?? problem?.title ?? error.message;
  }
  return error instanceof Error ? error.message : 'Unexpected error';
}

export function getFieldErrors(error: unknown) {
  if (axios.isAxiosError<ApiProblem>(error)) return error.response?.data?.errors ?? {};
  return {};
}
