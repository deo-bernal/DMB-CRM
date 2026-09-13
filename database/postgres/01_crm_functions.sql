-- Views and functions for DMB-CRM. Run after 00_crm_schema.sql.

create or replace view crm_v_contact_list as
select
  c.id,
  c.location_id,
  c.first_name,
  c.last_name,
  trim(concat(c.first_name, ' ', c.last_name)) as full_name,
  c.email,
  c.phone,
  c.source,
  c.company_id,
  co.name as company_name,
  c.created_at,
  c.updated_at
from crm_contacts c
left join crm_companies co on co.id = c.company_id;

create or replace view crm_v_pipeline_board as
select
  o.id,
  o.location_id,
  o.pipeline_id,
  p.name as pipeline_name,
  o.stage_id,
  s.name as stage_name,
  s.sort_order,
  o.contact_id,
  o.company_id,
  o.name,
  o.value,
  o.status,
  o.updated_at
from crm_opportunities o
join crm_pipelines p on p.id = o.pipeline_id
join crm_pipeline_stages s on s.id = o.stage_id;

create or replace view crm_v_location_dashboard as
select
  l.id as location_id,
  l.name as location_name,
  (select count(*) from crm_contacts c where c.location_id = l.id) as contact_count,
  (select count(*) from crm_companies co where co.location_id = l.id) as company_count,
  (select count(*) from crm_opportunities o where o.location_id = l.id and o.status = 'open') as open_opportunity_count,
  (select coalesce(sum(o.value), 0) from crm_opportunities o where o.location_id = l.id and o.status = 'open') as open_pipeline_value
from crm_locations l;

create or replace function crm_fn_log_activity(
  p_location_id uuid,
  p_contact_id uuid,
  p_opportunity_id uuid,
  p_activity_type text,
  p_summary text
) returns uuid
language plpgsql
as $$
declare
  v_id uuid;
begin
  insert into crm_activities (location_id, contact_id, opportunity_id, activity_type, summary)
  values (p_location_id, p_contact_id, p_opportunity_id, p_activity_type, p_summary)
  returning id into v_id;
  return v_id;
end;
$$;

create or replace function crm_fn_move_opportunity(
  p_opportunity_id uuid,
  p_stage_id uuid
) returns void
language plpgsql
as $$
declare
  v_location uuid;
  v_contact uuid;
  v_stage text;
begin
  select location_id, contact_id into v_location, v_contact
  from crm_opportunities
  where id = p_opportunity_id;

  if v_location is null then
    raise exception 'Opportunity not found';
  end if;

  select name into v_stage from crm_pipeline_stages where id = p_stage_id;
  if v_stage is null then
    raise exception 'Stage not found';
  end if;

  update crm_opportunities
  set stage_id = p_stage_id, updated_at = now()
  where id = p_opportunity_id;

  perform crm_fn_log_activity(v_location, v_contact, p_opportunity_id, 'stage', 'Moved to ' || v_stage);
end;
$$;

create or replace function crm_fn_location_stats(p_location_id uuid)
returns table (
  contact_count bigint,
  company_count bigint,
  open_opportunity_count bigint,
  open_pipeline_value numeric
)
language sql
stable
as $$
  select
    (select count(*) from crm_contacts where location_id = p_location_id),
    (select count(*) from crm_companies where location_id = p_location_id),
    (select count(*) from crm_opportunities where location_id = p_location_id and status = 'open'),
    (select coalesce(sum(value), 0) from crm_opportunities where location_id = p_location_id and status = 'open');
$$;
