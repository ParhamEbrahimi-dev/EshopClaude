# Ordering.Api — 03 — Get order

**Status:** Not started

## Goal

Return one order with its items. The storefront's order confirmation page uses this.

## Depends on

ordering-api/02-create-order

## Scope

- [ ] `GET /orders/{id}` returns the order with its items.
- [ ] Return 404 when the id doesn't exist.

## Explicit non-goals

- No list or history endpoint (that's ordering-api/04).
- No update or delete.

## Definition of Done

- `dotnet build` passes.
- `aspire run`: every resource is healthy.
- HTTP: create an order with `POST /orders`, then `GET /orders/{id}` returns it with matching items and total.
- HTTP: `GET /orders/{id}` with an unknown id returns 404.

## References

