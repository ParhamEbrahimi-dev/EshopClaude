# Basket.Api — 01 — Redis connection

**Status:** Not started

## Goal

Connect Basket.Api to its Redis resource, so later basket specs can read and write baskets and the connection shows up in health checks.

## Depends on

Nothing.

## Scope

- [ ] Register the Redis client with `AddRedisClient`.
- [ ] AppHost: Basket.Api gets `WithReference(redis)` + `WaitFor(redis)`.

## Explicit non-goals

- No basket endpoints and no basket model.
- No Redis data volume and no persistence settings.

## Definition of Done

- `dotnet build` passes.
- `aspire run`: every resource is healthy, and the Basket API starts only after redis is up.
- HTTP: the Basket API's `/health` passes, and the health check includes Redis.

## References

