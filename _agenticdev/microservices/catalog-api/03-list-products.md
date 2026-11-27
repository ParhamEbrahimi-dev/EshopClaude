# Catalog.Api — 03 — List products

**Status:** Not started

## Goal

Expose the whole catalog over HTTP, so the storefront home page can list it.

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
- HTTP: `GET /products` returns 200 with all seeded products and every PROJECT.md field.

## References

