# DMB CRM

GHL-shaped, multi-location CRM. New standalone product — not part of dmbportfolio.

- **API:** `DMB.CRM.API` on Render (Docker). Cold start is expected on Hobby.
- **Web:** `DMB.CRM.WEB` static CRA on a **new** Vercel project. Talks to Render only.
- **Database:** existing Supabase project `dmbProject`, all tables prefixed `crm_`.

## Run locally

1. Apply SQL in the Supabase SQL editor, in order:
   - `database/postgres/00_crm_schema.sql`
   - `database/postgres/01_crm_functions.sql`
   - `database/postgres/02_crm_seed.sql`
2. Copy `DMB.CRM.API/appsettings.json` values into user secrets or env:
   - `ConnectionStrings__CrmDb` = Supabase pooler URI
   - `Jwt__Secret` = a long random string **different** from dmbportfolio
   - `App__FrontendUrl` = `http://localhost:3000`
3. Start API:

```bash
dotnet run --project DMB.CRM.API
```

4. Start web:

```bash
cd DMB.CRM.WEB
copy .env.example .env
npm install
npm start
```

5. Apply the seed, then sign in as `deobernal@gmail.com` / `Test@123`. That user is the super-admin owner of the demo location, with sample company, contacts, and one opportunity.

## Hosting

See `docs/free-tier.md` and `docs/environment-variables.md`.
