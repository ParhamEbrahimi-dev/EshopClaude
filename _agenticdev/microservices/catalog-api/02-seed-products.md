# Catalog.Api — 02 — Seed products

**Status:** Not started

## Goal

Fill the catalog with a small, fixed set of products so the list endpoint and the storefront have real-looking data.

## Depends on

catalog-api/01-persistence

## Scope

- [ ] Seed 6–8 products (name, description, price, image URL) at startup.
- [ ] Seed only when the products table is empty.

## Explicit non-goals

- No endpoints.
- No admin or import feature, and no seed configuration.
- No entity or schema changes.

## Definition of Done

- `dotnet build` passes.
- `aspire run`: every resource is healthy.
- pgAdmin: the products table holds the 6–8 seeded rows.
- Restarting the AppHost doesn't duplicate rows (the row count stays the same).

## References

