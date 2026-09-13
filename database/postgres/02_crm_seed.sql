-- Demo agency, location, roles, super admin, and sample CRM rows.
-- Super admin password is HMAC-SHA512 hashed (same as AuthRepository). Plaintext: Test@123

insert into crm_agencies (id, name, slug)
values ('11111111-1111-1111-1111-111111111111', 'DMB Web Solutions', 'dmb')
on conflict (id) do nothing;

insert into crm_locations (id, agency_id, name, timezone)
values (
  '22222222-2222-2222-2222-222222222222',
  '11111111-1111-1111-1111-111111111111',
  'DMB Demo Location',
  'Asia/Manila'
)
on conflict (id) do nothing;

insert into crm_roles (id, name) values
  ('owner', 'Owner'),
  ('admin', 'Admin'),
  ('user', 'User')
on conflict (id) do nothing;

insert into crm_permissions (id, name) values
  ('contacts.read', 'Read contacts'),
  ('contacts.write', 'Write contacts'),
  ('opportunities.read', 'Read opportunities'),
  ('opportunities.write', 'Write opportunities')
on conflict (id) do nothing;

insert into crm_role_permissions (role_id, permission_id)
select r.id, p.id
from crm_roles r
cross join crm_permissions p
where r.id in ('owner', 'admin')
on conflict do nothing;

insert into crm_role_permissions (role_id, permission_id)
select 'user', p.id
from crm_permissions p
where p.id like '%.read'
on conflict do nothing;

insert into crm_users (
  id, agency_id, username, email, first_name, last_name,
  password_hash, password_salt, contact_no, activated, is_super_admin
)
values (
  '66666666-6666-6666-6666-666666666666',
  '11111111-1111-1111-1111-111111111111',
  'deobernal@gmail.com',
  'deobernal@gmail.com',
  'Deo',
  'Bernal',
  'ykwGEiGTSG2xYqbSsh89GupzhC6Mh2kjpIAykQboufcQeALD8n/Kg1enV2ioFHwrMX6WwHaoPP2rTQPgK1E06Q==',
  '2VokCELYP27oaSXSvmQ8lUBblRDoA+QoyhJHI58DJpb3UVDvqBbo9DnDO7d5BeqYBY7Z0cbXQymK3yDpKC32Kg1g0K2uzxrgeko6jBLXQ2JxPXqT1gwU4Et+6F8CcXgkr5bTjI+xNPlTskcHzuwVIS1J0Mh6unpVnvHZt5Z1T+s=',
  '+63 925 455 6063',
  true,
  true
)
on conflict (id) do update set
  username = excluded.username,
  email = excluded.email,
  first_name = excluded.first_name,
  last_name = excluded.last_name,
  password_hash = excluded.password_hash,
  password_salt = excluded.password_salt,
  activated = true,
  is_super_admin = true,
  updated_at = now();

insert into crm_user_locations (user_id, location_id, role)
values (
  '66666666-6666-6666-6666-666666666666',
  '22222222-2222-2222-2222-222222222222',
  'owner'
)
on conflict (user_id, location_id) do update set role = excluded.role;

insert into crm_pipelines (id, location_id, name)
values (
  '33333333-3333-3333-3333-333333333333',
  '22222222-2222-2222-2222-222222222222',
  'Sales'
)
on conflict (id) do nothing;

insert into crm_pipeline_stages (id, location_id, pipeline_id, name, sort_order)
values
  ('44444444-4444-4444-4444-444444444441', '22222222-2222-2222-2222-222222222222', '33333333-3333-3333-3333-333333333333', 'New', 0),
  ('44444444-4444-4444-4444-444444444442', '22222222-2222-2222-2222-222222222222', '33333333-3333-3333-3333-333333333333', 'Qualified', 1),
  ('44444444-4444-4444-4444-444444444443', '22222222-2222-2222-2222-222222222222', '33333333-3333-3333-3333-333333333333', 'Proposal', 2),
  ('44444444-4444-4444-4444-444444444444', '22222222-2222-2222-2222-222222222222', '33333333-3333-3333-3333-333333333333', 'Won', 3)
on conflict (id) do nothing;

insert into crm_tags (id, location_id, name, color)
values
  ('55555555-5555-5555-5555-555555555551', '22222222-2222-2222-2222-222222222222', 'Lead', '#2563eb'),
  ('55555555-5555-5555-5555-555555555552', '22222222-2222-2222-2222-222222222222', 'Client', '#059669')
on conflict (location_id, name) do nothing;

insert into crm_companies (id, location_id, name, website, phone)
values (
  '77777777-7777-7777-7777-777777777777',
  '22222222-2222-2222-2222-222222222222',
  'Marking Services Philippines',
  'https://www.markingservices.com',
  '+63 45 000 0000'
)
on conflict (id) do update set
  name = excluded.name,
  website = excluded.website,
  phone = excluded.phone,
  updated_at = now();

insert into crm_contacts (id, location_id, company_id, first_name, last_name, email, phone, source)
values
  (
    '88888888-8888-8888-8888-888888888881',
    '22222222-2222-2222-2222-222222222222',
    '77777777-7777-7777-7777-777777777777',
    'Youoa',
    'Xiong',
    'youoa.xiong@example.com',
    '+63 900 111 2222',
    'referral'
  ),
  (
    '88888888-8888-8888-8888-888888888882',
    '22222222-2222-2222-2222-222222222222',
    '77777777-7777-7777-7777-777777777777',
    'Alex',
    'Santos',
    'alex.santos@example.com',
    '+63 900 333 4444',
    'website'
  )
on conflict (id) do update set
  company_id = excluded.company_id,
  first_name = excluded.first_name,
  last_name = excluded.last_name,
  email = excluded.email,
  phone = excluded.phone,
  source = excluded.source,
  updated_at = now();

insert into crm_contact_tags (contact_id, tag_id)
values
  ('88888888-8888-8888-8888-888888888881', '55555555-5555-5555-5555-555555555551'),
  ('88888888-8888-8888-8888-888888888882', '55555555-5555-5555-5555-555555555552')
on conflict do nothing;

insert into crm_opportunities (id, location_id, pipeline_id, stage_id, contact_id, company_id, name, value, status)
values (
  '99999999-9999-9999-9999-999999999999',
  '22222222-2222-2222-2222-222222222222',
  '33333333-3333-3333-3333-333333333333',
  '44444444-4444-4444-4444-444444444442',
  '88888888-8888-8888-8888-888888888881',
  '77777777-7777-7777-7777-777777777777',
  'Website + CRM rollout',
  85000.00,
  'open'
)
on conflict (id) do update set
  stage_id = excluded.stage_id,
  contact_id = excluded.contact_id,
  company_id = excluded.company_id,
  name = excluded.name,
  value = excluded.value,
  status = excluded.status,
  updated_at = now();
