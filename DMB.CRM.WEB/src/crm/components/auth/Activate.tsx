import { useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import http from "../../services/http.service";

export default function Activate() {
  const [params] = useSearchParams();
  const [message, setMessage] = useState("Activating...");
  useEffect(() => {
    const token = params.get("token");
    if (!token) {
      setMessage("Missing activation token.");
      return;
    }
    http.post("/registration/activate", { token })
      .then((res) => setMessage(res.data.message))
      .catch((err) => setMessage(err?.response?.data?.message || "Activation failed."));
  }, [params]);

  return (
    <div className="auth-page">
      <div className="card">
        <h1>Activate account</h1>
        <p>{message}</p>
        <Link to="/login">Sign in</Link>
      </div>
    </div>
  );
}
