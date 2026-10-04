# Architecture

## POC

```text
Jira
  │ ticket update
  ▼
Jira Webhook
  ▼
ASP.NET Core Web API
  ├─ Filter: assignee = Komal
  ├─ Store processed update
  ├─ Assessment Service
  │    ├─ BEFORE TIME
  │    ├─ ON TIME
  │    └─ NOT ON TIME
  └─ Notification Service
       ▼
    Rahul / WhatsApp

React Dashboard → ASP.NET Core API → processed ticket history
```

## Production evolution

For production, replace in-memory persistence with SQL Server/Azure SQL and place a durable queue between webhook ingestion and downstream processing. Use retry/idempotency to prevent lost or duplicate notifications. Store Jira/WhatsApp credentials in a secret manager and add structured monitoring.

Suggested Azure mapping:
- React: Static Web Apps or App Service
- API: Azure App Service
- Queue: Azure Service Bus
- Database: Azure SQL
- Secrets: Azure Key Vault
- Monitoring: Application Insights

The assessment should remain explainable. An AI/LLM can be introduced later using Jira status, due date, latest update, blockers and progress context, with deterministic fallback rules.
