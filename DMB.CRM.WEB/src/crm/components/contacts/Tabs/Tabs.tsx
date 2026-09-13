import { useState } from "react";
import type { Contact } from "../../../models";

export default function ContactTabs({ contact }: { contact: Contact }) {
  const [tab, setTab] = useState<"overview" | "tags">("overview");
  return (
    <div>
      <div className="row-actions" style={{ margin: "1rem 0" }}>
        <button className={tab === "overview" ? "" : "secondary"} onClick={() => setTab("overview")}>Overview</button>
        <button className={tab === "tags" ? "" : "secondary"} onClick={() => setTab("tags")}>Tags</button>
      </div>
      {tab === "overview" ? (
        <div className="card" style={{ width: "100%" }}>
          <p><strong>Company:</strong> {contact.companyName || "—"}</p>
          <p><strong>Source:</strong> {contact.source || "—"}</p>
        </div>
      ) : (
        <div className="card" style={{ width: "100%" }}>
          {contact.tags.length === 0 ? <p className="muted">No tags</p> : contact.tags.map((t) => <p key={t.id}>{t.name}</p>)}
        </div>
      )}
    </div>
  );
}
