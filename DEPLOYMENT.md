# Render Deployment

This repository is prepared for a public demo on Render.

## Option A — Blueprint

1. Sign in to Render and connect GitHub.
2. Create a new Blueprint.
3. Select this repository.
4. Render reads `render.yaml` and creates:
   - `jira-whatsapp-assessment-api`
   - `jira-whatsapp-assessment-dashboard`
5. Deploy the backend first.
6. Copy the backend public URL.
7. Set the frontend environment variable:
   `VITE_API_BASE_URL=https://<backend-name>.onrender.com`
8. Redeploy the frontend.

## Verify backend

Open:
`https://<backend-name>.onrender.com/health`

Expected response:
`{"status":"ok"}`

## Demo

Post `sample-jira-webhook.json` to:
`https://<backend-name>.onrender.com/api/jira/webhook`

Then open/refresh the dashboard. The ticket assessment should appear.

## POC limitation

The current store is in memory. A free/ephemeral service restart clears processed ticket history. The WhatsApp provider boundary is currently mocked, so the backend log demonstrates the prepared notification without requiring production credentials.
