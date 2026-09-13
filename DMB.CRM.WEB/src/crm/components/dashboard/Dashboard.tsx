import { useEffect, useState } from "react";
import http from "../../services/http.service";
import type { LocationStats } from "../../models";
import { useAuth } from "../../../contexts/JWTAuthContext";

export default function Dashboard() {
  const { locationId } = useAuth();
  const [stats, setStats] = useState<LocationStats | null>(null);

  useEffect(() => {
    if (!locationId) return;
    http.get<LocationStats>("/location/stats").then((res) => setStats(res.data));
  }, [locationId]);

  return (
    <div>
      <h1>Location dashboard</h1>
      <div className="stats">
        <div className="stat"><div className="muted">Contacts</div><strong>{stats?.contactCount ?? 0}</strong></div>
        <div className="stat"><div className="muted">Companies</div><strong>{stats?.companyCount ?? 0}</strong></div>
        <div className="stat"><div className="muted">Open opportunities</div><strong>{stats?.openOpportunityCount ?? 0}</strong></div>
        <div className="stat"><div className="muted">Pipeline value</div><strong>{stats?.openPipelineValue ?? 0}</strong></div>
      </div>
    </div>
  );
}
