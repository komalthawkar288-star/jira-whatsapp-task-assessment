import React, { FormEvent, useEffect, useState } from "react";
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
  const [ticketKey, setTicketKey] = useState("JIRA-101");
  const [status, setStatus] = useState("In Progress");
  const [dueDate, setDueDate] = useState("");
  const [latestUpdate, setLatestUpdate] = useState("Development completed. Testing is in progress.");
  const [busy, setBusy] = useState(false);
  const [message, setMessage] = useState("");

  const load = async () => {
    try {
      const response = await fetch(`${API}/api/tickets`);
      if (!response.ok) throw new Error("Could not load ticket history.");
      setTickets(await response.json());
    } catch {
      setMessage("Could not load the API. Please retry in a moment.");
    }
  };

  useEffect(() => { load(); }, []);

  const postUpdate = async (e?: FormEvent) => {
    e?.preventDefault();
    setBusy(true);
    setMessage("");
    try {
      const response = await fetch(`${API}/api/jira/webhook`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          webhookEvent: "jira:issue_updated",
          issue: {
            key: ticketKey,
            fields: {
              summary: "Demo Jira Task",
              assignee: { displayName: "Komal" },
              status: { name: status },
              duedate: dueDate || null
            }
          },
          comment: { body: latestUpdate }
        })
      });
      if (!response.ok) throw new Error("Request failed");
      const result = await response.json();
      setMessage(`Processed successfully: ${result.assessment}`);
      await load();
    } catch {
      setMessage("Could not process the update. Please retry.");
    } finally {
      setBusy(false);
    }
  };

  const quickDemo = (type: "BEFORE" | "ON" | "NOT") => {
    const d = new Date();
    if (type === "BEFORE") {
      d.setDate(d.getDate() + 7);
      setLatestUpdate("Development completed. Testing is in progress.");
    } else if (type === "ON") {
      d.setDate(d.getDate() + 2);
      setLatestUpdate("Development is in progress and testing is planned.");
    } else {
      d.setDate(d.getDate() + 1);
      setLatestUpdate("Work is blocked due to an unresolved external dependency.");
    }
    setDueDate(d.toISOString().slice(0, 10));
    setMessage("Demo values loaded. Click Process Jira Update.");
  };

  return <main>
    <header>
      <p className="eyebrow">ASSIGNMENT POC</p>
      <h1>Jira → WhatsApp Task Assessment</h1>
      <p>Updates for Komal are assessed as BEFORE TIME, ON TIME or NOT ON TIME and prepared for Rahul's WhatsApp.</p>
    </header>

    <section className="simulator">
      <div>
        <p className="eyebrow">INTERACTIVE DEMO</p>
        <h2>Jira Update Simulator</h2>
        <p className="muted">This simulates the payload that a real Jira webhook sends to the backend.</p>
      </div>
      <div className="quick">
        <button type="button" onClick={() => quickDemo("BEFORE")}>Load BEFORE TIME case</button>
        <button type="button" onClick={() => quickDemo("ON")}>Load ON TIME case</button>
        <button type="button" onClick={() => quickDemo("NOT")}>Load NOT ON TIME case</button>
      </div>
      <form onSubmit={postUpdate}>
        <label>Ticket ID<input value={ticketKey} onChange={e => setTicketKey(e.target.value)} /></label>
        <label>Status<select value={status} onChange={e => setStatus(e.target.value)}><option>To Do</option><option>In Progress</option><option>Testing</option><option>Done</option></select></label>
        <label>Due Date<input type="date" value={dueDate} onChange={e => setDueDate(e.target.value)} /></label>
        <label className="wide">Latest Jira Update<textarea value={latestUpdate} onChange={e => setLatestUpdate(e.target.value)} rows={3} /></label>
        <button className="primary" disabled={busy}>{busy ? "Processing..." : "Process Jira Update"}</button>
      </form>
      {message && <p className="notice">{message}</p>}
    </section>

    <section className="history">
      <div className="historyTitle"><h2>Processed Updates</h2><button onClick={load}>Refresh</button></div>
      {tickets.length === 0 ? <div className="empty">No Jira updates processed yet. Use the simulator above.</div> :
        tickets.map((t, i) => <article key={t.ticketKey + t.latestUpdate + i}>
          <div className="row"><strong>{t.ticketKey}</strong><span className={`badge ${t.assessment.replaceAll(" ", "-").toLowerCase()}`}>{t.assessment}</span></div>
          <h3>{t.summary}</h3>
          <p><b>Status:</b> {t.status} &nbsp; <b>Assignee:</b> {t.assignee}</p>
          <p><b>Latest update:</b> {t.latestUpdate}</p>
          <p><b>Reason:</b> {t.reason}</p>
          <p className="whatsapp">WhatsApp: notification prepared for Rahul (mock/test mode)</p>
        </article>)}
    </section>
  </main>;
}

createRoot(document.getElementById("root")!).render(<App />);
