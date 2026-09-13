# Schema (`crm_` on dmbProject)

Apply `database/postgres/00_crm_schema.sql`, then `01_crm_functions.sql`, then `02_crm_seed.sql`.

## Agency / access

- `crm_agencies` — SaaS tenant (seed: DMB)
- `crm_locations` — GHL sub-accounts
- `crm_users` — login identity (hash/salt, activated, flags)
- `crm_user_locations` — user ↔ location + role (`owner`, `admin`, `user`)
- `crm_roles`, `crm_permissions`, `crm_role_permissions`
- `crm_account_activation_tokens`, `crm_password_reset_tokens`, `crm_revoked_tokens`

## CRM core

- `crm_companies`, `crm_contacts`, `crm_tags`, `crm_contact_tags`
- `crm_custom_fields`, `crm_custom_field_values`
- `crm_pipelines`, `crm_pipeline_stages`, `crm_opportunities`
- `crm_tasks`, `crm_notes`, `crm_activities`
- `crm_calendars`, `crm_appointments`
- `crm_conversations`, `crm_messages`
- `crm_workflows`, `crm_workflow_steps`, `crm_workflow_runs`
- `crm_email_templates`, `crm_email_sends`
- `crm_forms`, `crm_form_submissions`
- `crm_funnels`, `crm_funnel_pages`
- `crm_media` (URLs only)
- `crm_audit_log`

Every child table has `location_id` and an index on `(location_id, updated_at)` where it applies.

## Views and functions

- Views: `crm_v_contact_list`, `crm_v_pipeline_board`, `crm_v_location_dashboard`
- Functions: `crm_fn_move_opportunity`, `crm_fn_log_activity`, `crm_fn_location_stats`

## RLS

RLS is enabled on all `crm_*` tables. `anon` / `authenticated` are revoked. The Render API uses the database password / service role. Never put the service role in the CRA bundle.
