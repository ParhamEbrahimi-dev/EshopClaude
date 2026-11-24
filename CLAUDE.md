# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

EshopClaude is a minimal e-commerce demo (.NET 10 + .NET Aspire) built to show agentic, spec-driven coding with a microservices architecture. The scope is deliberately small — see "Source of truth" below before adding anything not already specified there.

## Source of truth

- `_agenticdev/PROJECT.md` — the design doc: entities, endpoints, basket shape, checkout flow, storefront pages. Read this before implementing any feature; it defines exact scope (and explicit non-goals: no auth, no payments, no message broker, no retries/resilience beyond Aspire defaults, no pagination/search, no automated tests, no basket editing, etc.).
- `_agenticdev/PLAN.md` — the build sequenced into steps (1: skeleton, 2: persistence, 3: per-service endpoints, 4: service-to-service wiring, 5: storefront pages, 6: end-to-end verification), each with its own verification method. Check this to see what step the repo is currently on and what "done" means for the current step before moving to the next.

Current state (per git history): Step 1 (project skeleton) is complete — three API projects exist with placeholder `/` endpoints and are wired into AppHost, but no persistence, no business endpoints, and no real storefront pages yet.

## Workflow

One feature branch per `PLAN.md` step. Branch off `main`, do the step's tasks, run its "Verify" checklist, then merge back — `main` must stay green at all times (never leave it mid-step or failing to build). Don't bundle multiple plan steps into one branch, and don't merge a step whose verification hasn't passed.

## Commands

All commands run from `src/`.

- Build: `dotnet build EshopClaude.sln`
- Run the whole system (Aspire orchestration — Postgres, Redis, all APIs, Web): `dotnet run --project EshopClaude.AppHost`
- Run a single service standalone (no orchestrated dependencies): `dotnet run --project EshopClaude.Catalog.Api` (or `EshopClaude.Basket.Api`, `EshopClaude.Ordering.Api`, `EshopClaude.Web`)
- There is no test project and no automated tests in this repo (explicitly out of scope per `PROJECT.md`). Verification is manual: build, run under Aspire, check the Aspire dashboard (resource health, logs, traces), and exercise HTTP endpoints / the browser — see the "Verify" section of each step in `PLAN.md`.

## Architecture

Four runnable projects plus one shared library, orchestrated by .NET Aspire:

- **EshopClaude.AppHost** (`src/EshopClaude.AppHost/AppHost.cs`) — the Aspire orchestrator. Declares every resource (API projects, Web, and eventually Postgres/Redis) and the dependency graph between them (`WithReference`/`WaitFor`). This is the only entry point that starts the full system — always run it, not the individual services, when testing cross-service behavior.
- **EshopClaude.Catalog.Api** — owns product data (Postgres `catalogdb` once Step 2 lands).
- **EshopClaude.Basket.Api** — owns the current basket (Redis-backed) and orchestrates checkout by calling Catalog.Api (price/name snapshot) and Ordering.Api (order creation) over HTTP.
- **EshopClaude.Ordering.Api** — owns placed orders (Postgres `orderingdb` once Step 2 lands).
- **EshopClaude.Web** — Blazor Server (InteractiveServer render mode) storefront under `Components/`. Calls all three APIs directly over HTTP via Aspire service discovery; there is no API gateway.
- **EshopClaude.ServiceDefaults** (`Extensions.cs`) — shared `AddServiceDefaults()`/`MapDefaultEndpoints()` extensions referenced by every service: OpenTelemetry, health checks (`/health`, `/alive`), service discovery, and standard HTTP resilience. Any new service project must reference this and call `builder.AddServiceDefaults()` / `app.MapDefaultEndpoints()` to stay consistent with the others.

Key architectural rules from the design doc (don't deviate without updating `PROJECT.md`):

- **No API gateway, no message broker.** All inter-service calls are synchronous HTTP through Aspire service discovery (e.g. `https+http://catalogapi`), configured via `ConfigureHttpClientDefaults` in `ServiceDefaults`.
- **Single hard-coded buyer.** There is no auth/login; `BuyerId = "demo-user"` is used everywhere a buyer id is needed (Redis basket key, orders). Don't add session/cookie plumbing.
- **Basket snapshots price/name at add-time.** Basket.Api calls Catalog.Api's `GET /products/{id}` once per add; viewing/checkout never re-calls Catalog.Api.
- **Checkout is orchestrated by Basket.Api**, not Web: Web → `POST /baskets/demo-user/checkout` → Basket.Api reads Redis basket → `POST /orders` on Ordering.Api → Basket.Api clears the Redis key → returns order id to Web. No saga/compensation — a failure just surfaces as an error.
- Target framework is `net10.0` across all projects; the AppHost SDK is Aspire 17.0.0 (`src/aspire.config.json`).
