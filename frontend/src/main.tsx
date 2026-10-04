import React, { useEffect, useState } from "react";
import { createRoot } from "react-dom/client";
import "./style.css";

type Ticket = {
  ticketKey: string;
  summary: string;
  assignee: string;
  status: string;
  dueDate?: string;
  latestUpdate: string;
  assessment: string;
  reason: string;
};

const API = import.meta.env.VITE_API_BASE_URL || "http://localhost:5000";

function App() {
  const [tickets, setTickets] = useState<Ticket[]>([]);

  const load = async () => {
    try {
      const response = await fetch(`${API}/api/tickets`);
      setTickets(await response.json());
    } catch {
      setTickets([]);
    }
  };

  useEffect(() => { load(); }, []);

  return <main>
    <header>
      <p className="eyebrow">ASSIGNMENT POC</p>
      <h1>Jira → WhatsApp Task Assessment</h1>
      <p>Updates for Komal are assessed as BEFORE TIME, ON TIME or NOT ON TIME and prepared for Rahul's WhatsApp.</p>
      <button onClick={load}>Refresh</button>
    </header>
    <section>
      {tickets.length === 0 ? <div className="empty">No Jira updates processed yet. Post the sample webhook, then refresh.</div> :
        tickets.map(t => <article key={t.ticketKey + t.latestUpdate}>
          <div className="row"><strong>{t.ticketKey}</strong><span className="badge">{t.assessment}</span></div>
          <h2>{t.summary}</h2>
          <p><b>Status:</b> {t.status} &nbsp; <b>Assignee:</b> {t.assignee}</p>
          <p><b>Latest update:</b> {t.latestUpdate}</p>
          <p><b>Reason:</b> {t.reason}</p>
        </article>)}
    </section>
  </main>;
}

createRoot(document.getElementById("root")!).render(<App />);
