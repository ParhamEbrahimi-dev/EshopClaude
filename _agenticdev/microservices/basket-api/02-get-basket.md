# Basket.Api — 02 — Get basket

**Status:** Not started

## Goal

Read the demo user's basket from Redis using the JSON shape in PROJECT.md, and return an empty basket when none exists. This sets up the basket model and the Redis read that add-item and checkout build on.

## Depends on

basket-api/01-redis-connection

## Scope

- [ ] Basket and basket item model matching the PROJECT.md JSON shape.
- [ ] `GET /baskets/demo-user` reads the `demo-user` key from Redis.
- [ ] Return an empty basket (`buyerId` plus an empty items list) when the key is missing.

## Explicit non-goals

- No write endpoints (add-item is basket-api/03).
- No calls to Catalog.Api or Ordering.Api.
- No placeholder add endpoint that takes a snapshot from the caller.

## Definition of Done

- `dotnet build` passes.
- `aspire run`: every resource is healthy.
- HTTP: `GET /baskets/demo-user` returns an empty basket on a fresh start.
- Redis round trip: write a basket JSON to the `demo-user` key with `redis-cli`, and `GET /baskets/demo-user` returns it.

## References

