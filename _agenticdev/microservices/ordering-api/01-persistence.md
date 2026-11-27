# Ordering.Api — 01 — Persistence

**Status:** Not started

## Goal

Give Ordering.Api its own schema in `orderingdb` so orders can be stored. Order and OrderItem form one aggregate, and as with catalog-api/01, the entities, DbContext, AppHost reference and schema creation can only be checked together (the tables in pgAdmin), so they stay in one spec.

## Depends on

Nothing.

## Scope

- [ ] Order and OrderItem entities with the fields listed in PROJECT.md (OrderItem is a child of Order).
- [ ] `OrderingDbContext` registered with `AddNpgsqlDbContext` against `orderingdb`.
- [ ] AppHost: Ordering.Api gets `WithReference(orderingdb)` + `WaitFor(orderingdb)`.
- [ ] Create the schema at startup.

## Explicit non-goals

- No endpoints besides the existing placeholder and health endpoints.
- No Status field and no state machine.
- No seed data.

## Definition of Done

- `dotnet build` passes.
- `aspire run`: every resource is healthy, and the Ordering API starts only after `orderingdb` is up.
- pgAdmin: `orderingdb` has empty orders and order items tables with the PROJECT.md columns and a foreign key from order items to orders.
- Restarting the AppHost doesn't fail.

## References

