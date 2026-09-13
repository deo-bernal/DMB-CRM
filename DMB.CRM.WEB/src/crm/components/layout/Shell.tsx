import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../../../contexts/JWTAuthContext";

export default function Shell() {
  const { locations, locationId, setLocationId, logout } = useAuth();

  return (
    <div className="shell">
      <aside className="nav">
        <h2>DMB CRM</h2>
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
        <NavLink to="/" end>Dashboard</NavLink>
        <NavLink to="/contacts">Contacts</NavLink>
        <NavLink to="/companies">Companies</NavLink>
        <NavLink to="/tags">Tags</NavLink>
        <NavLink to="/opportunities">Opportunities</NavLink>
        <button className="secondary" style={{ marginTop: "1rem", width: "100%" }} onClick={() => void logout()}>
          Sign out
        </button>
      </aside>
      <main className="main">
        <Outlet />
      </main>
    </div>
  );
}
