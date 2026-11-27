# Catalog.Api — 01 — Persistence

**Status:** Not started

## Goal

Give Catalog.Api its own database schema in `catalogdb`, so later specs have somewhere to put and read products. The Product entity, the DbContext, the AppHost reference and the schema creation stay in one spec because none can be checked alone: the only proof is the products table in pgAdmin, and that needs all four.

## Depends on

Nothing.

## Scope

- [ ] Product entity with the fields listed in PROJECT.md.
- [ ] `CatalogDbContext` registered with `AddNpgsqlDbContext` against `catalogdb`.
- [ ] AppHost: Catalog.Api gets `WithReference(catalogdb)` + `WaitFor(catalogdb)`.
- [ ] Create the schema at startup.

## Explicit non-goals

- No seed data.
- No endpoints besides the existing placeholder and health endpoints.
- No stock, category or other fields beyond PROJECT.md.

## Definition of Done

- `dotnet build` passes.
- `aspire run`: every resource is healthy, and the Catalog API starts only after `catalogdb` is up.
- pgAdmin: `catalogdb` has an empty products table with the PROJECT.md columns.
- Restarting the AppHost doesn't fail or recreate anything.

## References

