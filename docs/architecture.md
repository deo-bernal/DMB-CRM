# Architecture

DMB CRM copies **EMS folder layers**, not EMS travel domain.

```
DMB.CRM.WEB (CRA static)  →  DMB.CRM.API (.NET)  →  Supabase dmbProject (crm_* tables)
```

## Layers

| Project | Role |
| --- | --- |
| `DMB.CRM.API` | Thin controllers. `[Route("api/[controller]")]`, `[Authorize]`, inject `I*Service` only. |
| `DMB.CRM.Service` | `Interface/{Domain}` + `Implementation/{Domain}`. |
| `DMB.CRM.DATA` | EF Core `CrmContext` (Npgsql), entities, repositories, AutoMapper profiles. |
| `DMB.CRM.MODEL` | DTOs, `Roles.cs`, enums. |
| `DMB.CRM.WEB` | `src/crm/` domain folders (List / View / Create / Tabs), Redux slices, JWT context. |
| `DMB.CRM.Test` | Unit tests. |

`Program.cs` uses EMS-style `#region` DI blocks.

## Multi-location auth

- Login identity lives in `crm_users` (HMAC-SHA512 hash + salt, same pattern as dmbportfolio).
- JWT claims: `jti`, `Name`, `NameIdentifier` (user id), `Email`, `agencyId`.
- After login the API returns memberships and a current `locationId`.
- Every location-scoped request must send `X-Location-Id`. `LocationContextFilter` checks `crm_user_locations` (or super-admin agency access).
- Location roles: `owner`, `admin`, `user`. Web `ProtectedRoute` gates write screens.

Do not reuse dmbportfolio `User` / `leads` for CRM login.
