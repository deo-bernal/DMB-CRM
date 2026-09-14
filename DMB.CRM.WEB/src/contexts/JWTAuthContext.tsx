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
import type { AuthProfile, LocationMembership, LoginResponse } from "../crm/models";
import { firstNameFromToken, isSuperAdminFromToken, userIdFromToken } from "../crm/utils/sessionUser";

type AuthContextValue = {
  token: string | null;
  isAuthenticated: boolean;
  firstName: string;
  userId: string;
  isSuperAdmin: boolean;
  locations: LocationMembership[];
  locationId: string | null;
  currentRole: string;
  setLocationId: (id: string) => void;
  login: (username: string, password: string) => Promise<void>;
  acceptSession: (token: string, locations?: LocationMembership[], currentLocationId?: string) => Promise<void>;
  logout: () => Promise<void>;
  refreshProfile: () => Promise<void>;
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
  const [userId, setUserId] = useState(() =>
    localStorage.getItem("crm_userId") || userIdFromToken(localStorage.getItem("crm_token"))
  );
  const [isSuperAdmin, setIsSuperAdmin] = useState(
    () => localStorage.getItem("crm_isSuperAdmin") === "true" || isSuperAdminFromToken(localStorage.getItem("crm_token"))
  );

  const persistFirstName = useCallback((name?: string | null, nextToken?: string | null) => {
    const next = (name ?? "").trim() || firstNameFromToken(nextToken ?? null);
    setFirstName(next);
    if (next) localStorage.setItem("crm_firstName", next);
    else localStorage.removeItem("crm_firstName");
  }, []);

  const persistIdentity = useCallback((profile?: Partial<AuthProfile> | null, nextToken?: string | null) => {
    persistFirstName(profile?.firstName, nextToken);
    const nextUserId = profile?.userId || userIdFromToken(nextToken ?? null);
    setUserId(nextUserId);
    if (nextUserId) localStorage.setItem("crm_userId", nextUserId);
    else localStorage.removeItem("crm_userId");
    const nextSuper = typeof profile?.isSuperAdmin === "boolean"
      ? profile.isSuperAdmin
      : isSuperAdminFromToken(nextToken ?? null);
    setIsSuperAdmin(nextSuper);
    localStorage.setItem("crm_isSuperAdmin", nextSuper ? "true" : "false");
  }, [persistFirstName]);

  const hydrateProfile = useCallback(async (nextToken?: string | null) => {
    try {
      const res = await http.get<AuthProfile>("/auth/me", { skipLoading: true });
      persistIdentity(res.data, nextToken);
    } catch {
      persistIdentity(null, nextToken ?? localStorage.getItem("crm_token"));
    }
  }, [persistIdentity]);

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
    persistIdentity({ firstName: res.data.firstName, isSuperAdmin: res.data.isSuperAdmin }, res.data.token);
    void hydrateProfile(res.data.token);
  }, [persistLocations, persistIdentity, hydrateProfile]);

  const acceptSession = useCallback(async (
    nextToken: string,
    locations?: LocationMembership[],
    currentLocationId?: string
  ) => {
    localStorage.setItem("crm_token", nextToken);
    setToken(nextToken);
    persistIdentity(null, nextToken);
    if (locations && locations.length > 0) {
      persistLocations(locations, currentLocationId);
      void hydrateProfile(nextToken);
      return;
    }

    const res = await http.get<LocationMembership[]>("/location/list");
    persistLocations(res.data ?? [], currentLocationId);
    void hydrateProfile(nextToken);
  }, [persistLocations, persistIdentity, hydrateProfile]);

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
    localStorage.removeItem("crm_userId");
    localStorage.removeItem("crm_isSuperAdmin");
    setToken(null);
    setFirstName("");
    setUserId("");
    setIsSuperAdmin(false);
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
      userId,
      isSuperAdmin,
      locations,
      locationId,
      currentRole,
      setLocationId,
      login,
      acceptSession,
      logout,
      refreshProfile: () => hydrateProfile(token),
    }),
    [token, firstName, userId, isSuperAdmin, locations, locationId, currentRole, setLocationId, login, acceptSession, logout, hydrateProfile]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside AuthProvider");
  return ctx;
}
