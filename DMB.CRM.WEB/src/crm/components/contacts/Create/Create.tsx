import { FormEvent, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import type { AppDispatch, RootState } from "../../../../store";
import { fetchCompanies } from "../../../slices/companySlice";
import { fetchTags } from "../../../slices/tagSlice";
import { saveContact } from "../../../slices/contactSlice";
import http from "../../../services/http.service";

export default function ContactCreate() {
  const { id } = useParams();
  const navigate = useNavigate();
  const dispatch = useDispatch<AppDispatch>();
  const companies = useSelector((s: RootState) => s.companies.items);
  const tags = useSelector((s: RootState) => s.tags.items);
  const [form, setForm] = useState({
    firstName: "",
    lastName: "",
    email: "",
    phone: "",
    source: "",
    companyId: "",
    tagIds: [] as string[],
  });

  useEffect(() => {
    void dispatch(fetchCompanies());
    void dispatch(fetchTags());
    if (id) {
      http.get(`/contact/${id}`).then((res) => {
        setForm({
          firstName: res.data.firstName,
          lastName: res.data.lastName,
          email: res.data.email ?? "",
          phone: res.data.phone ?? "",
          source: res.data.source ?? "",
          companyId: res.data.companyId ?? "",
          tagIds: (res.data.tags ?? []).map((t: { id: string }) => t.id),
        });
      });
    }
  }, [dispatch, id]);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    await dispatch(saveContact({
      id,
      body: { ...form, companyId: form.companyId || undefined },
    })).unwrap();
    navigate("/contacts");
  };

  return (
    <form className="form-grid" onSubmit={onSubmit}>
      <h1>{id ? "Edit contact" : "New contact"}</h1>
      <input placeholder="First name" value={form.firstName} onChange={(e) => setForm({ ...form, firstName: e.target.value })} />
      <input placeholder="Last name" value={form.lastName} onChange={(e) => setForm({ ...form, lastName: e.target.value })} />
      <input placeholder="Email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} />
      <input placeholder="Phone" value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })} />
      <input placeholder="Source" value={form.source} onChange={(e) => setForm({ ...form, source: e.target.value })} />
      <select value={form.companyId} onChange={(e) => setForm({ ...form, companyId: e.target.value })}>
        <option value="">No company</option>
        {companies.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
      </select>
      <select multiple value={form.tagIds} onChange={(e) => setForm({ ...form, tagIds: Array.from(e.target.selectedOptions).map((o) => o.value) })}>
        {tags.map((t) => <option key={t.id} value={t.id}>{t.name}</option>)}
      </select>
      <button type="submit">Save</button>
    </form>
  );
}
