import { useCallback, useEffect, useState } from 'react';
import { getStoredUser, login as apiLogin, logout as apiLogout } from '../services/api';

export function useAuth() {
  const [user, setUser] = useState(() => getStoredUser());

  useEffect(() => {
    setUser(getStoredUser());
  }, []);

  const login = useCallback(async (username, password) => {
    const data = await apiLogin(username, password);
    setUser(getStoredUser());
    return data;
  }, []);

  const logout = useCallback(() => {
    apiLogout();
    setUser(null);
  }, []);

  return { user, isAuthenticated: Boolean(user), login, logout };
}
