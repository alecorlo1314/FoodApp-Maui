-- product_metrics is empty; refuse to discard historical UUID product IDs.
do $$
begin
  if exists (select 1 from public.product_metrics) then
    raise exception 'product_metrics must be empty before changing product_id to bigint';
  end if;
end $$;
alter table public.product_metrics
  alter column product_id type bigint using null::bigint,
  add column user_id uuid not null references auth.users(id),
  add column payment_id text not null,
  add constraint product_metrics_product_id_fkey foreign key (product_id) references public.foods(id),
  add constraint product_metrics_quantity_positive check (quantity_ordered > 0),
  add constraint product_metrics_payment_nonempty check (length(trim(payment_id)) > 0),
  add constraint product_metrics_payment_product_unique unique (user_id, payment_id, product_id);
create index product_metrics_product_id_idx on public.product_metrics(product_id);
alter table public.product_metrics enable row level security;
revoke all on public.product_metrics from anon, authenticated;
grant select, insert on public.product_metrics to authenticated;
grant usage, select on sequence public.product_metrics_id_seq to authenticated;
create policy product_metrics_select_own on public.product_metrics for select
  to authenticated using ((select auth.uid()) = user_id);
create policy product_metrics_insert_own on public.product_metrics for insert
  to authenticated with check ((select auth.uid()) = user_id);
