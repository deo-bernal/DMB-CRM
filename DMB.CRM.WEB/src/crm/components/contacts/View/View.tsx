import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import http from "../../../services/http.service";
import type { Contact } from "../../../models";
import ContactTabs from "../Tabs/Tabs";

export default function ContactView() {
  const { id } = useParams();
  const [contact, setContact] = useState<Contact | null>(null);

  useEffect(() => {
    if (id) http.get<Contact>(`/contact/${id}`).then((res) => setContact(res.data));
  }, [id]);

  if (!contact) {
    return (
      <div>
        <div className="toolbar">
          <h1>Contact</h1>
        </div>
      </div>
    );
  }

  return (
    <div>
      <div className="toolbar">
        <h1>{contact.fullName}</h1>
        <Link to={`/contacts/create/${contact.id}`}><button className="secondary">Edit</button></Link>
      </div>
      <p className="muted">{contact.email} {contact.phone}</p>
      <ContactTabs contact={contact} />
    </div>
  );
}
