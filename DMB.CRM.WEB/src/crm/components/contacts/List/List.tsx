import { useEffect } from "react";
import { Link } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import type { AppDispatch, RootState } from "../../../../store";
import { fetchContacts } from "../../../slices/contactSlice";
import { useAuth } from "../../../../contexts/JWTAuthContext";

export default function ContactList() {
  const dispatch = useDispatch<AppDispatch>();
  const { items } = useSelector((s: RootState) => s.contacts);
  const { locationId } = useAuth();

  useEffect(() => {
    if (locationId) void dispatch(fetchContacts());
  }, [dispatch, locationId]);

  return (
    <div>
      <div className="toolbar">
        <h1>Contacts</h1>
        <Link to="/contacts/create"><button>New contact</button></Link>
      </div>
      <table>
        <thead>
          <tr><th>Name</th><th>Email</th><th>Phone</th><th>Company</th><th>Tags</th></tr>
        </thead>
        <tbody>
          {items.map((c) => (
            <tr key={c.id}>
              <td><Link to={`/contacts/view/${c.id}`}>{c.fullName || "Untitled"}</Link></td>
              <td>{c.email}</td>
              <td>{c.phone}</td>
              <td>{c.companyName}</td>
              <td>{c.tags.map((t) => t.name).join(", ")}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
