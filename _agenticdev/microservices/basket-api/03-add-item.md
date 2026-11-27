# Basket.Api — 03 — Add item

**Status:** Not started

## Goal

Add a product to the basket and store a snapshot of its name and price, taken from Catalog.Api at the moment it's added. Adding a new item and incrementing an existing one are one endpoint with one rule, so they stay together. This replaces PLAN.md's placeholder add-item with the real Catalog lookup.

## Depends on

basket-api/02-get-basket, catalog-api/04-get-product

## Scope

- [ ] Typed `HttpClient` for Catalog.Api through Aspire service discovery.
- [ ] AppHost: Basket.Api gets `WithReference` + `WaitFor` on Catalog.Api.
- [ ] `POST /baskets/demo-user/items` (`productId`, `quantity`) calls `GET /products/{id}` and stores productName and unitPrice.
- [ ] If the product is already in the basket, increase its quantity instead of adding a second line.
- [ ] Return an error when Catalog.Api returns 404 for the product.

## Explicit non-goals

- No remove or change-quantity endpoints.
- No stock or quantity validation.
- No retries or fallbacks beyond the Aspire defaults.
- No checkout.

## Definition of Done

- `dotnet build` passes.
- `aspire run`: every resource is healthy.
- HTTP: adding a valid productId stores an item with the correct name and price (check with `GET /baskets/demo-user`).
- HTTP: adding the same productId again increases the quantity, with no duplicate line.
- HTTP: an unknown productId returns an error, and the basket is unchanged.
- Traces: the add-item trace shows the Basket.Api → Catalog.Api hop.

## References

