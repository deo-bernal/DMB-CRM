import { FormEvent, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import type { AppDispatch, RootState } from "../../../../store";
import { fetchOpportunities, fetchPipelines, moveOpportunity, saveOpportunity } from "../../../slices/pipelineSlice";
import { fetchContacts } from "../../../slices/contactSlice";
import { useAuth } from "../../../../contexts/JWTAuthContext";

export default function OpportunityList() {
  const dispatch = useDispatch<AppDispatch>();
  const { pipelines, opportunities } = useSelector((s: RootState) => s.pipelines);
  const contacts = useSelector((s: RootState) => s.contacts.items);
  const { locationId } = useAuth();
  const pipeline = pipelines[0];
  const [name, setName] = useState("");
  const [value, setValue] = useState("0");
  const [contactId, setContactId] = useState("");

  useEffect(() => {
    if (!locationId) return;
    void dispatch(fetchPipelines());
    void dispatch(fetchOpportunities());
    void dispatch(fetchContacts());
  }, [dispatch, locationId]);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!pipeline) return;
    await dispatch(saveOpportunity({
      body: {
        name,
        value: Number(value),
        pipelineId: pipeline.id,
        stageId: pipeline.stages[0]?.id,
        contactId: contactId || undefined,
        status: "open",
      },
    })).unwrap();
    setName("");
  };

  return (
    <div>
      <h1>Opportunities</h1>
      <form className="form-grid" onSubmit={onSubmit} style={{ marginBottom: "1.2rem" }}>
        <input placeholder="Deal name" value={name} onChange={(e) => setName(e.target.value)} required />
        <input placeholder="Value" value={value} onChange={(e) => setValue(e.target.value)} />
        <select value={contactId} onChange={(e) => setContactId(e.target.value)}>
          <option value="">No contact</option>
          {contacts.map((c) => <option key={c.id} value={c.id}>{c.fullName}</option>)}
        </select>
        <button type="submit">Add opportunity</button>
      </form>
      <div className="board">
        {(pipeline?.stages ?? []).map((stage) => (
          <div className="column" key={stage.id}>
            <strong>{stage.name}</strong>
            {opportunities.filter((o) => o.stageId === stage.id).map((o) => (
              <div className="card-item" key={o.id}>
                <div>{o.name}</div>
                <div className="muted">{o.value}</div>
                <select
                  value={o.stageId}
                  onChange={(e) => void dispatch(moveOpportunity({ id: o.id, stageId: e.target.value }))}
                >
                  {(pipeline?.stages ?? []).map((s) => <option key={s.id} value={s.id}>{s.name}</option>)}
                </select>
              </div>
            ))}
          </div>
        ))}
      </div>
    </div>
  );
}
