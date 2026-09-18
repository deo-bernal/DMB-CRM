import { useState } from "react";
import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../../../contexts/JWTAuthContext";
import BrandMark from "./BrandMark";
import { writeRoles } from "../../enums/roles";

const SITE = "https://www.dmbwebsolutions.com";

export default function Shell() {
  const { locations, locationId, setLocationId, logout, firstName, currentRole, isSuperAdmin } = useAuth();
  const [signingOut, setSigningOut] = useState(false);
  const canManageUsers = isSuperAdmin || writeRoles.includes(currentRole as (typeof writeRoles)[number]);

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
        <a className="brand-wrap" href={`${SITE}/ai-automation`}>
          <BrandMark />
        </a>
        {firstName ? <div className="nav-greeting">Hi {firstName}</div> : null}
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
        <div className="nav-stack">
          <div className="nav-label">Workspace</div>
          <NavLink to="/" end>Dashboard</NavLink>
          <NavLink to="/contacts">Contacts</NavLink>
          <NavLink to="/companies">Companies</NavLink>
          <NavLink to="/tags">Tags</NavLink>
          <NavLink to="/opportunities">Opportunities</NavLink>
          {canManageUsers ? <NavLink to="/users">Manage users</NavLink> : null}

          <div className="nav-label">Your profile</div>
          <NavLink to="/account">Account</NavLink>
          <a href={`${SITE}/lms`}>LMS</a>
          <a href={`${SITE}/commerce`}>Commerce</a>
          <a href={`${SITE}/agent`}>Agent</a>
          <a href={`${SITE}/accent-sidebar/portfolio`}>Portfolio</a>
          <a href={SITE}>Website</a>
        </div>
        <button
          className="secondary"
          style={{ marginTop: "1.2rem", width: "100%" }}
          onClick={() => void onSignOut()}
          disabled={signingOut}
          aria-busy={signingOut}
        >
          {signingOut ? <><span className="btn-spinner" aria-hidden /> Signing out…</> : "Sign out"}
        </button>
        <a className="nav-also" href={`${SITE}/profiles#lots`}>
          Also: lots in Pampanga
        </a>
      </aside>
      <main className="main">
        <Outlet />
      </main>
    </div>
  );
}
