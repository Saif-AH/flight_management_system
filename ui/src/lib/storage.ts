import type { AuthUser, LoginResponse, RefreshResponse } from '@/types/api';

const ACCESS_TOKEN_KEY = 'flight_admin_access_token';
const REFRESH_TOKEN_KEY = 'flight_admin_refresh_token';
const EXPIRES_AT_KEY = 'flight_admin_expires_at';
const USER_KEY = 'flight_admin_user';

export const tokenStorage = {
  getAccessToken: () => localStorage.getItem(ACCESS_TOKEN_KEY),
  getRefreshToken: () => localStorage.getItem(REFRESH_TOKEN_KEY),
  getUser: (): AuthUser | null => {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as AuthUser;
    } catch {
      localStorage.removeItem(USER_KEY);
      return null;
    }
  },
  getUserFromLogin: (payload: LoginResponse): AuthUser => ({
    id: payload.userId,
    email: payload.userName,
    fullName: payload.userName,
    roles: payload.roles
  }),
  setAuth: (payload: LoginResponse) => {
    const user = tokenStorage.getUserFromLogin(payload);
    localStorage.setItem(ACCESS_TOKEN_KEY, payload.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, payload.refreshToken);
    localStorage.setItem(EXPIRES_AT_KEY, payload.accessTokenExpiresAtUtc);
    localStorage.setItem(USER_KEY, JSON.stringify(user));
    return user;
  },
  setTokens: (payload: RefreshResponse) => {
    localStorage.setItem(ACCESS_TOKEN_KEY, payload.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, payload.refreshToken);
    localStorage.setItem(EXPIRES_AT_KEY, payload.accessTokenExpiresAtUtc);
  },
  clear: () => {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(EXPIRES_AT_KEY);
    localStorage.removeItem(USER_KEY);
  }
};
