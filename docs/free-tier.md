# Free-tier hosting

| Host | Limit that matters | CRM choice |
| --- | --- | --- |
| Supabase Free | 2 **active** projects | Reuse `dmbProject`. `crm_` tables only. Do not create a third project. |
| Vercel Hobby | 12 serverless functions **per project** | **New Vercel project**, root `DMB.CRM.WEB`. Static CRA (`react-scripts build`) + SPA rewrite to `index.html`. API base = Render URL. Do not copy portfolio `api/*.js`. |
| Render Hobby | 750 free instance-hours / month **per workspace**; sleep after 15 min idle | New Docker web service from `DMB.CRM.API/Dockerfile`. You already run `dmbportfolio-api` + n8n. **No keep-alive cron.** Accept ~30–60s cold start. |

If Render hours run out mid-month, **all** free web services in that workspace suspend.

## Vercel deploy

Create a new project (not the portfolio). Root Directory: `DMB.CRM.WEB`. Build: `npm run build`. Output: `build`. Env: `REACT_APP_CRM_API_URL`.

## Render deploy

New Web Service, Docker, Dockerfile path `DMB.CRM.API/Dockerfile`, context the `DMB-CRM` repo root. Set the API env vars from `environment-variables.md`.
