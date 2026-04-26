import * as React from 'react';
import { useMutation } from '@tanstack/react-query';
import { authApi } from '@/api/endpoints';
import { getProblemMessage } from '@/api/http';
import { tokenStorage } from '@/lib/storage';
import type { AuthUser, LoginRequest } from '@/types/api';

type AuthContextValue = {
  user: AuthUser | null;
  isAuthenticated: boolean;
  login: (values: LoginRequest) => Promise<void>;
  logout: () => void;
  isLoggingIn: boolean;
  loginError?: string;
};

const AuthContext = React.createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = React.useState<AuthUser | null>(() => tokenStorage.getUser());
  const loginMutation = useMutation({
    mutationFn: authApi.login,
    onSuccess: (data) => {
      const authenticatedUser = tokenStorage.setAuth(data);
      setUser(authenticatedUser);
    }
  });

  const login = React.useCallback(async (values: LoginRequest) => {
    await loginMutation.mutateAsync(values);
  }, [loginMutation]);

  const logout = React.useCallback(() => {
    void authApi.logout().catch(() => undefined);
    tokenStorage.clear();
    setUser(null);
  }, []);

  return (
    <AuthContext.Provider value={{ user, isAuthenticated: Boolean(tokenStorage.getAccessToken() && user), login, logout, isLoggingIn: loginMutation.isPending, loginError: loginMutation.error ? getProblemMessage(loginMutation.error) : undefined }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = React.useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
}
