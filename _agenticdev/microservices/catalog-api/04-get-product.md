# Catalog.Api — 04 — Get product

**Status:** In progress

## Goal

Expose a single product by id. Basket.Api uses it to snapshot name and price when an item is added.

## Context

- The endpoint is a minimal API mapped in `Program.cs`, route `/products/{id:int}`, injecting `CatalogDbContext`.
- It returns the `Product` entity as JSON (camelCase: `id`, `name`, `description`, `price`, `imageUrl`), the same shape as `GET /products`. No DTO.
- The lookup is read-only (`AsNoTracking`, `FirstOrDefaultAsync`) and returns `TypedResults.Ok(product)` or `TypedResults.NotFound()`.

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
- HTTP: `GET http://localhost:<catalogapi port>/products/1` returns `200` with a JSON object for product `id: 1` (`Classic Mug`, price `12.50`) with `id`, `name`, `description`, `price`, `imageUrl`.
- HTTP: `GET http://localhost:<catalogapi port>/products/9999` returns `404`.

## References

