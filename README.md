# Jira to WhatsApp Task Assessment

A proof-of-concept that receives Jira ticket updates for Komal, assesses expected delivery as **BEFORE TIME**, **ON TIME**, or **NOT ON TIME**, and prepares a WhatsApp notification for Rahul.

## Core flow

Jira → Webhook → ASP.NET Core API → Assignee Filter → Assessment Service → Notification Service → Rahul's WhatsApp

A React/TypeScript dashboard displays processed ticket updates and assessments.

## Acceptance criteria

- Jira is the source system.
- Process updates for Jira tickets assigned to Komal.
- On every ticket update, generate exactly one assessment:
  - BEFORE TIME
  - ON TIME
  - NOT ON TIME
- Include the Jira update and assessment in the WhatsApp notification for Rahul.

## Tech stack

- Backend: ASP.NET Core Web API (.NET 8)
- Frontend: React + TypeScript
- Current POC storage: in-memory
- Current notification provider: mock/test implementation
- Production design: SQL/Azure SQL, queue/retry, WhatsApp Cloud API, secure secrets and monitoring

## Run backend

```bash
cd backend/JiraWhatsAppAssessment.Api
dotnet restore
dotnet run --urls http://localhost:5000
```

Health check: `http://localhost:5000/health`

## Run frontend

```bash
cd frontend
npm install
npm run dev
```

## Simulate Jira update

```bash
curl -X POST http://localhost:5000/api/jira/webhook \
  -H "Content-Type: application/json" \
  --data-binary "@sample-jira-webhook.json"
```

## Important POC note

This repository intentionally keeps external credentials out of source control. Jira and WhatsApp production credentials should be provided through environment variables/secret storage. The current POC uses deterministic assessment logic and a mock/test notification boundary so the end-to-end application flow can be demonstrated without exposing credentials.
