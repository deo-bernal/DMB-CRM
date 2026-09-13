import { FormEvent, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import type { AppDispatch, RootState } from "../../../../store";
import { fetchTags, saveTag } from "../../../slices/tagSlice";
import { useAuth } from "../../../../contexts/JWTAuthContext";

export default function TagList() {
  const dispatch = useDispatch<AppDispatch>();
  const items = useSelector((s: RootState) => s.tags.items);
  const { locationId } = useAuth();
  const [name, setName] = useState("");
  const [color, setColor] = useState("#2563eb");

  useEffect(() => {
    if (locationId) void dispatch(fetchTags());
  }, [dispatch, locationId]);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    await dispatch(saveTag({ name, color })).unwrap();
    setName("");
  };

  return (
    <div>
      <h1>Tags</h1>
      <form className="form-grid" onSubmit={onSubmit} style={{ marginBottom: "1.2rem" }}>
        <input placeholder="Name" value={name} onChange={(e) => setName(e.target.value)} required />
        <input type="color" value={color} onChange={(e) => setColor(e.target.value)} />
        <button type="submit">Add tag</button>
      </form>
      <table>
        <thead><tr><th>Name</th><th>Color</th></tr></thead>
        <tbody>
          {items.map((t) => (
            <tr key={t.id}><td>{t.name}</td><td>{t.color}</td></tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
