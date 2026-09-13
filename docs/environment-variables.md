# Environment variables

## API (Render / local)

| Name | Purpose |
| --- | --- |
| `ConnectionStrings__CrmDb` | Supabase **pooler** URI for `dmbProject` |
| `Jwt__Secret` | Long random secret. **Different** from dmbportfolio |
| `Jwt__Issuer` / `Jwt__Audience` | Default `dmbcrm` |
| `App__FrontendUrl` | CRM web origin (activation and reset links) |
| `Cors__Origins__0` | Allowed SPA origin |
| `Smtp__Host`, `Smtp__Port`, `Smtp__From`, `Smtp__Username`, `Smtp__Password` | Activation / reset email. First agency user can register without SMTP. |

## Web (Vercel / local)

| Name | Purpose |
| --- | --- |
| `REACT_APP_CRM_API_URL` | Public Render API base, including `/api` |

Do **not** put `Jwt:Secret`, database passwords, or the Supabase service role in any `REACT_APP_*` variable.
