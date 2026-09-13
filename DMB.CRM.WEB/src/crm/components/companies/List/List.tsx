import { FormEvent, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import type { AppDispatch, RootState } from "../../../../store";
import { fetchCompanies, saveCompany } from "../../../slices/companySlice";
import { useAuth } from "../../../../contexts/JWTAuthContext";

export default function CompanyList() {
  const dispatch = useDispatch<AppDispatch>();
  const items = useSelector((s: RootState) => s.companies.items);
  const { locationId } = useAuth();
  const [name, setName] = useState("");
  const [website, setWebsite] = useState("");
  const [phone, setPhone] = useState("");

  useEffect(() => {
    if (locationId) void dispatch(fetchCompanies());
  }, [dispatch, locationId]);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    await dispatch(saveCompany({ body: { name, website, phone } })).unwrap();
    setName("");
    setWebsite("");
    setPhone("");
  };

  return (
    <div>
      <h1>Companies</h1>
      <form className="form-grid" onSubmit={onSubmit} style={{ marginBottom: "1.2rem" }}>
        <input placeholder="Name" value={name} onChange={(e) => setName(e.target.value)} required />
        <input placeholder="Website" value={website} onChange={(e) => setWebsite(e.target.value)} />
        <input placeholder="Phone" value={phone} onChange={(e) => setPhone(e.target.value)} />
        <button type="submit">Add company</button>
      </form>
      <table>
        <thead><tr><th>Name</th><th>Website</th><th>Phone</th></tr></thead>
        <tbody>
          {items.map((c) => (
            <tr key={c.id}><td>{c.name}</td><td>{c.website}</td><td>{c.phone}</td></tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
