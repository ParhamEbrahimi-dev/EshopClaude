# Catalog.Api — 04 — Get product

**Status:** Not started

## Goal

Expose a single product by id. Basket.Api uses it to snapshot name and price when an item is added.

## Depends on

catalog-api/03-list-products

## Scope

- [ ] `GET /products/{id}` returns one product.
- [ ] Return 404 when the id doesn't exist.

## Explicit non-goals

- No create, update or delete endpoints.
- No changes to `GET /products`.

## Definition of Done

- `dotnet build` passes.
- `aspire run`: every resource is healthy.
- HTTP: `GET /products/{id}` with a seeded id returns 200 and that product.
- HTTP: `GET /products/{id}` with an unknown id returns 404.

## References

