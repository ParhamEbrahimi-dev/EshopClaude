# Catalog.Api — 03 — List products

**Status:** In progress

## Goal

Expose the whole catalog over HTTP, so the storefront home page can list it.

## Context

- The endpoint is a minimal API mapped in `Program.cs`, injecting `CatalogDbContext`.
- It returns the `Product` entity as JSON (camelCase: `id`, `name`, `description`, `price`, `imageUrl`). No DTO: the entity's fields are exactly the PROJECT.md fields.
- The query is read-only (`AsNoTracking`) and ordered by `Id` for a stable order. That is not a user-facing sorting option.

## Depends on

catalog-api/02-seed-products

## Scope

- [ ] `GET /products` returns every product.

## Explicit non-goals

- No paging, filtering, search or sorting options.
- No single-product endpoint (that's catalog-api/04).

## Definition of Done

- `dotnet build` passes.
- `aspire run`: every resource is healthy.
- HTTP: `GET http://localhost:<catalogapi port>/products` returns `200` with a JSON array of the 8 seeded products, each with `id`, `name`, `description`, `price`, `imageUrl`.

## References

