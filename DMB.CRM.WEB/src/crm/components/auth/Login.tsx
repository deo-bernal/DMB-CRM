import { FormEvent, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../../contexts/JWTAuthContext";
import BrandMark from "../layout/BrandMark";

export default function Login() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError("");
    try {
      await login(username, password);
      navigate("/", { replace: true });
    } catch (err: any) {
      setError(err?.response?.data?.message || "Invalid credentials.");
    }
  };

  return (
    <div className="auth-page">
      <form className="card" onSubmit={onSubmit}>
        <BrandMark />
        <h1>Sign in</h1>
        <p className="muted">Your DMB location workspace.</p>
        {error ? <p className="error">{error}</p> : null}
        <div className="field">
          <label>Email</label>
          <input value={username} onChange={(e) => setUsername(e.target.value)} required />
        </div>
        <div className="field">
          <label>Password</label>
          <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} required />
        </div>
        <button type="submit">Sign in</button>
        <p>
          <Link to="/register">Create account</Link> · <Link to="/forgot-password">Forgot password</Link>
        </p>
      </form>
    </div>
  );
}
