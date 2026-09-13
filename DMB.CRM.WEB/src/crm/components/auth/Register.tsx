import { FormEvent, useState } from "react";
import { Link } from "react-router-dom";
import http from "../../services/http.service";
import BrandMark from "../layout/BrandMark";
import PasswordField from "./PasswordField";
import SocialAuthButtons from "./SocialAuthButtons";

export default function Register() {
  const [form, setForm] = useState({
    email: "",
    firstName: "",
    lastName: "",
    password: "",
    contactNumber: "",
  });
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError("");
    try {
      const res = await http.post("/registration/register", form);
      setMessage(res.data.message);
    } catch (err: any) {
      setError(err?.response?.data?.message || "Registration failed.");
    }
  };

  return (
    <div className="auth-page">
      <form className="card" onSubmit={onSubmit}>
        <BrandMark />
        <h1>Create account</h1>
        <SocialAuthButtons />
        {message ? <p>{message}</p> : null}
        {error ? <p className="error">{error}</p> : null}
        <div className="field"><label>Email</label><input value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} required /></div>
        <div className="field"><label>First name</label><input value={form.firstName} onChange={(e) => setForm({ ...form, firstName: e.target.value })} required /></div>
        <div className="field"><label>Last name</label><input value={form.lastName} onChange={(e) => setForm({ ...form, lastName: e.target.value })} required /></div>
        <PasswordField
          label="Password"
          value={form.password}
          onChange={(password) => setForm({ ...form, password })}
          autoComplete="new-password"
        />
        <div className="field"><label>Phone</label><input value={form.contactNumber} onChange={(e) => setForm({ ...form, contactNumber: e.target.value })} /></div>
        <button type="submit">Create account</button>
        <div className="auth-links">
          <Link to="/login">Back to sign in</Link>
        </div>
      </form>
    </div>
  );
}
