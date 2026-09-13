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
import { firstNameFromToken } from "../crm/utils/sessionUser";

type AuthContextValue = {
  token: string | null;
  isAuthenticated: boolean;
  firstName: string;
  locations: LocationMembership[];
  locationId: string | null;
  currentRole: string;
  setLocationId: (id: string) => void;
  login: (username: string, password: string) => Promise<void>;
  acceptSession: (token: string, locations?: LocationMembership[], currentLocationId?: string) => Promise<void>;
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
  const [firstName, setFirstName] = useState(() =>
    localStorage.getItem("crm_firstName") || firstNameFromToken(localStorage.getItem("crm_token"))
  );

  const persistFirstName = useCallback((name?: string | null, token?: string | null) => {
    const next = (name ?? "").trim() || firstNameFromToken(token ?? null);
    setFirstName(next);
    if (next) localStorage.setItem("crm_firstName", next);
    else localStorage.removeItem("crm_firstName");
  }, []);

  const hydrateProfile = useCallback(async (token?: string | null) => {
    try {
      const res = await http.get<{ firstName?: string }>("/auth/me");
      persistFirstName(res.data.firstName, token);
    } catch {
      persistFirstName(null, token ?? localStorage.getItem("crm_token"));
    }
  }, [persistFirstName]);

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
    persistFirstName(res.data.firstName, res.data.token);
    if (!res.data.firstName) {
      void hydrateProfile(res.data.token);
    }
  }, [persistLocations, persistFirstName, hydrateProfile]);

  const acceptSession = useCallback(async (
    nextToken: string,
    locations?: LocationMembership[],
    currentLocationId?: string
  ) => {
    localStorage.setItem("crm_token", nextToken);
    setToken(nextToken);
    persistFirstName(null, nextToken);
    if (locations && locations.length > 0) {
      persistLocations(locations, currentLocationId);
      void hydrateProfile(nextToken);
      return;
    }

    const res = await http.get<LocationMembership[]>("/location/list");
    persistLocations(res.data ?? [], currentLocationId);
    void hydrateProfile(nextToken);
  }, [persistLocations, persistFirstName, hydrateProfile]);

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
    localStorage.removeItem("crm_firstName");
    setToken(null);
    setFirstName("");
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

  useEffect(() => {
    if (!token) return;
    void hydrateProfile(token);
  }, [token, hydrateProfile]);

  const currentRole = locations.find((l) => l.locationId === locationId)?.role ?? "user";

  const value = useMemo(
    () => ({
      token,
      isAuthenticated: Boolean(token),
      firstName,
      locations,
      locationId,
      currentRole,
      setLocationId,
      login,
      acceptSession,
      logout,
    }),
    [token, firstName, locations, locationId, currentRole, setLocationId, login, acceptSession, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside AuthProvider");
  return ctx;
}
