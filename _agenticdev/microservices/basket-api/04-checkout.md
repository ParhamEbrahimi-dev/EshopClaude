# Basket.Api — 04 — Checkout

**Status:** Not started

## Goal

Turn the basket into an order. Basket.Api orchestrates checkout: it reads the basket, creates the order in Ordering.Api, clears the basket and returns the order id. The parts stay in one spec because any partial version is broken, either creating orders without clearing the basket or doing nothing at all.

## Depends on

basket-api/03-add-item, ordering-api/02-create-order

## Scope

- [ ] Typed `HttpClient` for Ordering.Api through Aspire service discovery.
- [ ] AppHost: Basket.Api gets `WithReference` + `WaitFor` on Ordering.Api.
- [ ] `POST /baskets/demo-user/checkout` returns an error if the basket is empty.
- [ ] Call `POST /orders` with buyerId and the basket items.
- [ ] Delete the `demo-user` Redis key, then return the new order id.

## Explicit non-goals

- No retries, compensation or saga.
- No payment, address or other checkout input.
- No price re-check against Catalog.Api.

## Definition of Done

- `dotnet build` passes.
- `aspire run`: every resource is healthy.
- HTTP: checking out a populated basket returns an order id. `GET /orders/{id}` on Ordering.Api shows the matching items and total, and `GET /baskets/demo-user` is empty afterwards.
- HTTP: checking out an empty basket returns an error, and no order is created.
- Traces: the checkout trace shows the Basket.Api → Ordering.Api hop.

## References

