import { FormEvent, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import http from "../../services/http.service";
import BrandMark from "../layout/BrandMark";

export default function ResetPassword() {
  const [params] = useSearchParams();
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError("");
    try {
      const res = await http.post("/auth/reset-password", {
        token: params.get("token"),
        newPassword,
        confirmPassword,
      });
      setMessage(res.data.message);
    } catch (err: any) {
      setError(err?.response?.data?.message || "Reset failed.");
    }
  };

  return (
    <div className="auth-page">
      <form className="card" onSubmit={onSubmit}>
        <BrandMark />
        <h1>Reset password</h1>
        {message ? <p>{message}</p> : null}
        {error ? <p className="error">{error}</p> : null}
        <div className="field"><label>New password</label><input type="password" value={newPassword} onChange={(e) => setNewPassword(e.target.value)} required /></div>
        <div className="field"><label>Confirm</label><input type="password" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)} required /></div>
        <button type="submit">Save password</button>
        <p><Link to="/login">Back to sign in</Link></p>
      </form>
    </div>
  );
}
