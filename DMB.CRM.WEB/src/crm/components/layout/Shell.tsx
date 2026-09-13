import { useState } from "react";
import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../../../contexts/JWTAuthContext";
import BrandMark from "./BrandMark";

export default function Shell() {
  const { locations, locationId, setLocationId, logout } = useAuth();
  const [signingOut, setSigningOut] = useState(false);

  const onSignOut = async () => {
    if (signingOut) return;
    setSigningOut(true);
    try {
      await logout();
    } catch {
      setSigningOut(false);
    }
  };

  return (
    <div className="shell">
      <aside className="nav">
        <a className="brand-wrap" href="https://www.dmbwebsolutions.com/ai-automation">
          <BrandMark />
        </a>
        <div className="location-switch">
          <label className="muted">Location</label>
          <select value={locationId ?? ""} onChange={(e) => setLocationId(e.target.value)}>
            {locations.map((l) => (
              <option key={l.locationId} value={l.locationId}>
                {l.name}
              </option>
            ))}
          </select>
        </div>
        <div className="nav-label">Workspace</div>
        <NavLink to="/" end>Dashboard</NavLink>
        <NavLink to="/contacts">Contacts</NavLink>
        <NavLink to="/companies">Companies</NavLink>
        <NavLink to="/tags">Tags</NavLink>
        <NavLink to="/opportunities">Opportunities</NavLink>
        <button
          className="secondary"
          style={{ marginTop: "1.2rem", width: "100%" }}
          onClick={() => void onSignOut()}
          disabled={signingOut}
          aria-busy={signingOut}
        >
          {signingOut ? <><span className="btn-spinner" aria-hidden /> Signing out…</> : "Sign out"}
        </button>
      </aside>
      <main className="main">
        <Outlet />
      </main>
    </div>
  );
}
