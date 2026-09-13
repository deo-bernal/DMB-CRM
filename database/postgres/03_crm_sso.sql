-- Google / LinkedIn / Facebook logins for crm_users.
-- Safe to re-run.

create table if not exists crm_external_logins (
  id uuid primary key default gen_random_uuid(),
  user_id uuid not null references crm_users(id) on delete cascade,
  provider text not null,
  provider_user_id text not null,
  created_at timestamptz not null default now(),
  unique (provider, provider_user_id),
  unique (user_id, provider)
);
create index if not exists crm_external_logins_user_idx on crm_external_logins(user_id);

create table if not exists crm_pending_external_logins (
  id uuid primary key default gen_random_uuid(),
  ticket text not null unique,
  provider text not null,
  provider_user_id text not null,
  first_name text not null default '',
  last_name text not null default '',
  email text,
  phone text,
  client text not null default 'web',
  return_path text,
  code_hash text,
  code_expires_at timestamptz,
  expires_at timestamptz not null,
  created_at timestamptz not null default now()
);
create index if not exists crm_pending_external_logins_expires_idx on crm_pending_external_logins(expires_at);
