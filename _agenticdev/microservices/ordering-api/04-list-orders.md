# Ordering.Api — 04 — List orders

**Status:** Not started

## Goal

List a buyer's orders, newest first. The storefront's order history page uses this.

## Depends on

ordering-api/03-get-order

## Scope

- [ ] `GET /orders?buyerId=demo-user` returns that buyer's orders, newest first.

## Explicit non-goals

- No paging and no filters other than buyerId.
- No changes to the create or get endpoints.

## Definition of Done

- `dotnet build` passes.
- `aspire run`: every resource is healthy.
- HTTP: create two orders, then `GET /orders?buyerId=demo-user` returns both with the most recent first.
- HTTP: an unknown buyerId returns an empty list.

## References

