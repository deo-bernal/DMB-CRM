import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import { useNavigate } from "react-router-dom";
import http from "../crm/services/http.service";
import type { LocationMembership, LoginResponse } from "../crm/models";

type AuthContextValue = {
  token: string | null;
  isAuthenticated: boolean;
  locations: LocationMembership[];
  locationId: string | null;
  currentRole: string;
  setLocationId: (id: string) => void;
  login: (username: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const navigate = useNavigate();
  const [token, setToken] = useState<string | null>(() => localStorage.getItem("crm_token"));
  const [locations, setLocations] = useState<LocationMembership[]>(() => {
    const raw = localStorage.getItem("crm_locations");
    return raw ? (JSON.parse(raw) as LocationMembership[]) : [];
  });
  const [locationId, setLocationIdState] = useState<string | null>(() => localStorage.getItem("crm_locationId"));

  const persistLocations = useCallback((next: LocationMembership[], current?: string) => {
    setLocations(next);
    localStorage.setItem("crm_locations", JSON.stringify(next));
    const selected = current && next.some((l) => l.locationId === current)
      ? current
      : next[0]?.locationId ?? null;
    setLocationIdState(selected);
    if (selected) localStorage.setItem("crm_locationId", selected);
    else localStorage.removeItem("crm_locationId");
  }, []);

  const setLocationId = useCallback((id: string) => {
    setLocationIdState(id);
    localStorage.setItem("crm_locationId", id);
  }, []);

  const login = useCallback(async (username: string, password: string) => {
    const res = await http.post<LoginResponse>("/auth/login", { username, password });
    localStorage.setItem("crm_token", res.data.token);
    setToken(res.data.token);
    persistLocations(res.data.locations ?? [], res.data.currentLocationId);
  }, [persistLocations]);

  const logout = useCallback(async () => {
    try {
      if (localStorage.getItem("crm_token")) {
        await http.post("/auth/logout");
      }
    } catch {
      // still clear local session
    }
    localStorage.removeItem("crm_token");
    localStorage.removeItem("crm_locations");
    localStorage.removeItem("crm_locationId");
    setToken(null);
    setLocations([]);
    setLocationIdState(null);
    navigate("/login", { replace: true });
  }, [navigate]);

  useEffect(() => {
    const onUnauthorized = () => {
      void logout();
    };
    window.addEventListener("crm:unauthorized", onUnauthorized);
    return () => window.removeEventListener("crm:unauthorized", onUnauthorized);
  }, [logout]);

  const currentRole = locations.find((l) => l.locationId === locationId)?.role ?? "user";

  const value = useMemo(
    () => ({
      token,
      isAuthenticated: Boolean(token),
      locations,
      locationId,
      currentRole,
      setLocationId,
      login,
      logout,
    }),
    [token, locations, locationId, currentRole, setLocationId, login, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside AuthProvider");
  return ctx;
}
