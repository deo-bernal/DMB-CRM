-- DMB-CRM schema for existing Supabase project dmbProject.
-- Prefix crm_ so this never collides with dmbportfolio User / leads / n8n tables.
-- Run as postgres/service_role in the SQL editor: 00 -> 01 -> 02.

create extension if not exists pgcrypto;

-- ---------- Agency / access ----------
create table if not exists crm_agencies (
  id uuid primary key default gen_random_uuid(),
  name text not null,
  slug text not null unique,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

create table if not exists crm_locations (
  id uuid primary key default gen_random_uuid(),
  agency_id uuid not null references crm_agencies(id) on delete cascade,
  name text not null,
  timezone text not null default 'Asia/Manila',
  is_active boolean not null default true,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);
create index if not exists crm_locations_agency_idx on crm_locations(agency_id);

create table if not exists crm_users (
  id uuid primary key default gen_random_uuid(),
  agency_id uuid not null references crm_agencies(id) on delete cascade,
  username text not null,
  email text not null,
  first_name text not null default '',
  last_name text not null default '',
  password_hash text not null,
  password_salt text not null,
  contact_no text,
  activated boolean not null default false,
  is_super_admin boolean not null default false,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  unique (agency_id, username),
  unique (agency_id, email)
);

create table if not exists crm_user_locations (
  user_id uuid not null references crm_users(id) on delete cascade,
  location_id uuid not null references crm_locations(id) on delete cascade,
  role text not null check (role in ('owner', 'admin', 'user')),
  created_at timestamptz not null default now(),
  primary key (user_id, location_id)
);

create table if not exists crm_roles (
  id text primary key,
  name text not null
);

create table if not exists crm_permissions (
  id text primary key,
  name text not null
);

create table if not exists crm_role_permissions (
  role_id text not null references crm_roles(id) on delete cascade,
  permission_id text not null references crm_permissions(id) on delete cascade,
  primary key (role_id, permission_id)
);

create table if not exists crm_account_activation_tokens (
  id uuid primary key default gen_random_uuid(),
  user_id uuid not null references crm_users(id) on delete cascade,
  token_hash text not null unique,
  expires_at timestamptz not null,
  used_at timestamptz,
  created_at timestamptz not null default now()
);

create table if not exists crm_password_reset_tokens (
  id uuid primary key default gen_random_uuid(),
  user_id uuid not null references crm_users(id) on delete cascade,
  token_hash text not null unique,
  expires_at timestamptz not null,
  used_at timestamptz,
  created_at timestamptz not null default now()
);

create table if not exists crm_revoked_tokens (
  id uuid primary key default gen_random_uuid(),
  jti text not null unique,
  user_id uuid,
  expires_at timestamptz not null,
  created_at timestamptz not null default now()
);

-- ---------- CRM core ----------
create table if not exists crm_companies (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  name text not null,
  website text,
  phone text,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);
create index if not exists crm_companies_location_idx on crm_companies(location_id, updated_at desc);

create table if not exists crm_contacts (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  company_id uuid references crm_companies(id) on delete set null,
  first_name text not null default '',
  last_name text not null default '',
  email text,
  phone text,
  source text,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);
create index if not exists crm_contacts_location_idx on crm_contacts(location_id, updated_at desc);
create index if not exists crm_contacts_email_idx on crm_contacts(location_id, email);

create table if not exists crm_tags (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  name text not null,
  color text,
  unique (location_id, name)
);

create table if not exists crm_contact_tags (
  contact_id uuid not null references crm_contacts(id) on delete cascade,
  tag_id uuid not null references crm_tags(id) on delete cascade,
  primary key (contact_id, tag_id)
);

create table if not exists crm_custom_fields (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  entity_type text not null check (entity_type in ('contact', 'company', 'opportunity')),
  name text not null,
  field_type text not null default 'text',
  unique (location_id, entity_type, name)
);

create table if not exists crm_custom_field_values (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  field_id uuid not null references crm_custom_fields(id) on delete cascade,
  entity_id uuid not null,
  value text,
  unique (field_id, entity_id)
);

create table if not exists crm_pipelines (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  name text not null,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

create table if not exists crm_pipeline_stages (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  pipeline_id uuid not null references crm_pipelines(id) on delete cascade,
  name text not null,
  sort_order int not null default 0
);

create table if not exists crm_opportunities (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  pipeline_id uuid not null references crm_pipelines(id) on delete cascade,
  stage_id uuid not null references crm_pipeline_stages(id) on delete restrict,
  contact_id uuid references crm_contacts(id) on delete set null,
  company_id uuid references crm_companies(id) on delete set null,
  name text not null,
  value numeric(12,2) not null default 0,
  status text not null default 'open' check (status in ('open', 'won', 'lost')),
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);
create index if not exists crm_opportunities_location_idx on crm_opportunities(location_id, updated_at desc);

create table if not exists crm_tasks (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  contact_id uuid references crm_contacts(id) on delete set null,
  opportunity_id uuid references crm_opportunities(id) on delete set null,
  title text not null,
  due_at timestamptz,
  completed_at timestamptz,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

create table if not exists crm_notes (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  contact_id uuid references crm_contacts(id) on delete cascade,
  body text not null,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

create table if not exists crm_activities (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  contact_id uuid,
  opportunity_id uuid,
  activity_type text not null check (activity_type in ('call', 'email', 'sms', 'meeting', 'note', 'stage')),
  summary text not null,
  created_at timestamptz not null default now()
);
create index if not exists crm_activities_location_idx on crm_activities(location_id, created_at desc);

create table if not exists crm_calendars (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  name text not null,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

create table if not exists crm_appointments (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  calendar_id uuid not null references crm_calendars(id) on delete cascade,
  contact_id uuid references crm_contacts(id) on delete set null,
  title text not null,
  starts_at timestamptz not null,
  ends_at timestamptz not null,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

create table if not exists crm_conversations (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  contact_id uuid references crm_contacts(id) on delete set null,
  channel text not null check (channel in ('email', 'sms', 'form', 'manual')),
  subject text,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

create table if not exists crm_messages (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  conversation_id uuid not null references crm_conversations(id) on delete cascade,
  direction text not null check (direction in ('in', 'out')),
  body text not null,
  created_at timestamptz not null default now()
);

create table if not exists crm_workflows (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  name text not null,
  trigger_type text not null,
  is_active boolean not null default false,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

create table if not exists crm_workflow_steps (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  workflow_id uuid not null references crm_workflows(id) on delete cascade,
  sort_order int not null default 0,
  action_type text not null,
  config jsonb not null default '{}'::jsonb
);

create table if not exists crm_workflow_runs (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  workflow_id uuid not null references crm_workflows(id) on delete cascade,
  contact_id uuid,
  status text not null default 'pending',
  started_at timestamptz not null default now(),
  finished_at timestamptz
);

create table if not exists crm_email_templates (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  name text not null,
  subject text not null,
  body text not null,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

create table if not exists crm_email_sends (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  template_id uuid references crm_email_templates(id) on delete set null,
  contact_id uuid,
  subject text not null,
  status text not null default 'queued',
  created_at timestamptz not null default now()
);

create table if not exists crm_forms (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  name text not null,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

create table if not exists crm_form_submissions (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  form_id uuid not null references crm_forms(id) on delete cascade,
  contact_id uuid references crm_contacts(id) on delete set null,
  payload jsonb not null default '{}'::jsonb,
  created_at timestamptz not null default now()
);

create table if not exists crm_funnels (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  name text not null,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

create table if not exists crm_funnel_pages (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  funnel_id uuid not null references crm_funnels(id) on delete cascade,
  title text not null,
  slug text not null,
  unique (funnel_id, slug)
);

create table if not exists crm_media (
  id uuid primary key default gen_random_uuid(),
  location_id uuid not null references crm_locations(id) on delete cascade,
  url text not null,
  file_name text,
  created_at timestamptz not null default now()
);

create table if not exists crm_audit_log (
  id uuid primary key default gen_random_uuid(),
  location_id uuid references crm_locations(id) on delete set null,
  user_id uuid,
  action text not null,
  entity_type text,
  entity_id uuid,
  details jsonb,
  created_at timestamptz not null default now()
);

-- RLS: API uses the database user / service_role. Lock down PostgREST anon.
do $$
declare
  t text;
begin
  foreach t in array array[
    'crm_agencies','crm_locations','crm_users','crm_user_locations','crm_roles','crm_permissions','crm_role_permissions',
    'crm_account_activation_tokens','crm_password_reset_tokens','crm_revoked_tokens',
    'crm_companies','crm_contacts','crm_tags','crm_contact_tags','crm_custom_fields','crm_custom_field_values',
    'crm_pipelines','crm_pipeline_stages','crm_opportunities','crm_tasks','crm_notes','crm_activities',
    'crm_calendars','crm_appointments','crm_conversations','crm_messages',
    'crm_workflows','crm_workflow_steps','crm_workflow_runs',
    'crm_email_templates','crm_email_sends','crm_forms','crm_form_submissions',
    'crm_funnels','crm_funnel_pages','crm_media','crm_audit_log'
  ]
  loop
    execute format('alter table %I enable row level security', t);
    execute format('revoke all on table %I from anon, authenticated', t);
  end loop;
end $$;
