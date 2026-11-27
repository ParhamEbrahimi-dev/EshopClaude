# Ordering.Api — 02 — Create order

**Status:** Not started

## Goal

Let a caller create an order from a buyer id and a list of line items. Basket.Api's checkout will use this.

## Depends on

ordering-api/01-persistence

## Scope

- [ ] `POST /orders` accepts `buyerId` and items (productId, productName, unitPrice, quantity).
- [ ] Set `CreatedDate`, and compute `Total` as the sum of unitPrice × quantity.
- [ ] Save the order and its items, then return the created order with its id.

## Explicit non-goals

- No read endpoints (those are ordering-api/03 and /04).
- No validation against Catalog and no price re-checks.
- No payment and no status.

## Definition of Done

- `dotnet build` passes.
- `aspire run`: every resource is healthy.
- HTTP: `POST /orders` with a sample payload of 2+ items returns a success status, the new id and the correct total.
- pgAdmin: the order and its items are in `orderingdb` with matching values.

## References

