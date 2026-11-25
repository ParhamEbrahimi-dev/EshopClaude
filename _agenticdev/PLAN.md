# EshopClaude — Build Plan

Source of truth for scope/design: `_agenticdev/PROJECT.md`. This plan sequences the work into small, branch-sized steps. No code here — tasks and verification only.

## Workflow

Each numbered step below is done on its own feature branch, created off `main`. A step's branch is only merged back to `main` after its "Verify" checklist passes in full. `main` must stay green at all times — if a step turns out to need splitting, split it into further branches rather than merging partial/broken work.

Legend for verification:
- **Build** — `dotnet build` on the solution (or affected projects).
- **Aspire run** — `dotnet run` on `EshopClaude.AppHost`, check the Aspire dashboard (resources healthy, logs clean).
- **HTTP** — manual request via `curl`/`.http` file/REST client against a running service.
- **Traces** — inspect the Aspire dashboard's traces/structured logs for a request (used once cross-service calls exist).
- **Browser** — exercise the Blazor storefront manually.

---

## Step 1 — Project skeleton

Create the three microservice projects and wire the AppHost, without business logic yet. Establishes the shape of the solution before any feature work.

**Tasks:**
1. Add `Catalog.Api`, `Basket.Api`, `Ordering.Api` as new ASP.NET Core minimal API projects under `src/`, each referencing `EshopClaude.ServiceDefaults`.
2. Give each a trivial placeholder endpoint (e.g. `GET /` or `/health`) so each is runnable and visible in the dashboard.
3. Remove `EshopClaude.ApiService` (project + solution entry + AppHost registration + any reference from `EshopClaude.Web`).
4. Update `EshopClaude.AppHost` to register the three new projects (no data resources yet) and remove the `apiservice` reference from `EshopClaude.Web`'s wiring.
5. Add solution references so `dotnet build` covers all projects (update `.sln`).

**Projects touched:** `EshopClaude.AppHost`, `EshopClaude.ServiceDefaults`, new `Catalog.Api`, `Basket.Api`, `Ordering.Api`, `EshopClaude.Web` (reference removal), solution file.

**Verify:**
- Build: solution builds clean with no reference to `EshopClaude.ApiService`.
- Aspire run: all three new services + Web show up and go "Running" in the dashboard; no `apiservice` resource present.
- HTTP: placeholder endpoint on each of the three services responds.

---

## Step 2 — Data persistence design

Add the Postgres and Redis resources, EF Core wiring for the two databases, pgAdmin, and Catalog seed data. No business endpoints yet — just persistence plumbing and a provable connection.

Split into sub-steps, each on its own feature branch and merged only after its own Verify list passes.

### 2a. AppHost data infrastructure

**Tasks:**
1. In `EshopClaude.AppHost`: add the `Aspire.Hosting.PostgreSQL` and `Aspire.Hosting.Redis` hosting packages.
2. Add a single Postgres server resource with `WithPgAdmin()`, and two database resources `catalogdb` and `orderingdb` off of it.
3. Add a Redis resource (no data volume).
4. Add client integration packages only (no code): `Aspire.Npgsql.EntityFrameworkCore.PostgreSQL` in `Catalog.Api` and `Ordering.Api`, `Aspire.StackExchange.Redis` in `Basket.Api`.
5. No `WithReference`/`WaitFor` between the API resources and the data resources yet.

**Projects touched:** `EshopClaude.AppHost`, `Catalog.Api`, `Ordering.Api`, `Basket.Api` (csproj only).

**Verify:**
- Build: solution builds clean.
- Aspire run: postgres, pgadmin, `catalogdb`, `orderingdb`, redis, and all services show healthy in the dashboard.
- pgAdmin: the Postgres server is listed with `catalogdb` and `orderingdb` (both empty).
- Redis: `redis-cli PING` in the container returns `PONG`; no volume mounted.

### 2b. Catalog persistence

**Tasks:**
1. Add the Product entity (per PROJECT.md) and a `CatalogDbContext` in `Catalog.Api`, registered with `AddNpgsqlDbContext`.
2. AppHost: `Catalog.Api` → `WithReference(catalogdb)` + `WaitFor`.
3. Create the schema at startup (e.g. `EnsureCreated`/migration).

**Projects touched:** `EshopClaude.AppHost`, `Catalog.Api`.

**Verify:**
- Build. Aspire run: all resources healthy.
- pgAdmin: `catalogdb` contains the products table.

### 2c. Catalog seed data

**Tasks:**
1. Seed 6–8 products at `Catalog.Api` startup (only when the table is empty).

**Projects touched:** `Catalog.Api`.

**Verify:**
- Build. Aspire run: all resources healthy.
- pgAdmin: the products table is populated; restarting does not duplicate rows.

### 2d. Ordering persistence

**Tasks:**
1. Add Order + OrderItem entities and an `OrderingDbContext` in `Ordering.Api`, registered with `AddNpgsqlDbContext`.
2. AppHost: `Ordering.Api` → `WithReference(orderingdb)` + `WaitFor`.
3. Create the schema at startup.

**Projects touched:** `EshopClaude.AppHost`, `Ordering.Api`.

**Verify:**
- Build. Aspire run: all resources healthy.
- pgAdmin: `orderingdb` contains the (empty) orders and order items tables.

### 2e. Basket Redis connection

**Tasks:**
1. Register the Redis client in `Basket.Api` with `AddRedisClient` (no schema — just connection).
2. AppHost: `Basket.Api` → `WithReference(redis)` + `WaitFor`.

**Projects touched:** `EshopClaude.AppHost`, `Basket.Api`.

**Verify:**
- Build. Aspire run: all resources healthy, `basketapi` health check includes Redis and passes.
- Traces: none yet (no cross-service calls).

---

## Step 3 — Microservices, one by one

Build each service's real endpoints against its own data store, independently testable via HTTP before any service-to-service calls exist.

### 3a. Catalog.Api

**Tasks:**
1. Implement `GET /products` (list all).
2. Implement `GET /products/{id}` (single product, 404 if missing).

**Projects touched:** `Catalog.Api`.

**Verify:**
- Build.
- Aspire run.
- HTTP: `GET /products` returns the seeded list; `GET /products/{id}` returns a known product and 404 for a bad id.

### 3b. Ordering.Api

**Tasks:**
1. Implement `POST /orders` — create order + child order items from `buyerId` + items payload, compute `Total`, return created order with id.
2. Implement `GET /orders/{id}` — single order with items, 404 if missing.
3. Implement `GET /orders?buyerId=demo-user` — list orders for buyer, newest first.

**Projects touched:** `Ordering.Api`.

**Verify:**
- Build.
- Aspire run.
- HTTP: `POST /orders` with a sample payload returns a created order with correct computed total; `GET /orders/{id}` returns it; `GET /orders?buyerId=demo-user` lists it first.

### 3c. Basket.Api (standalone endpoints only — no Catalog/Ordering calls yet)

**Tasks:**
1. Implement `GET /baskets/demo-user` — read basket from Redis, return empty basket shape if none exists.
2. Implement basket read/write helpers against Redis using the JSON shape from PROJECT.md (no external HTTP calls yet — add item endpoint stores a caller-supplied snapshot directly, as a placeholder ahead of Step 4's real Catalog lookup).

**Projects touched:** `Basket.Api`.

**Verify:**
- Build.
- Aspire run.
- HTTP: `GET /baskets/demo-user` returns an empty basket initially; writing a basket directly to Redis (or via the placeholder add) and re-reading confirms round-trip.

---

## Step 4 — Service communication

Wire the real cross-service calls: Basket.Api resolving products from Catalog.Api, and the full checkout orchestration against Ordering.Api. This is where Aspire service discovery and `HttpClient` wiring matter most.

**Tasks:**
1. Add an `HttpClient` in `Basket.Api` configured for Aspire service discovery to `Catalog.Api` (e.g. `https+http://catalogapi`).
2. Implement `POST /baskets/demo-user/items`: call `GET /products/{id}` on Catalog.Api to snapshot name/price, then add-or-increment the item in the Redis basket.
3. Add an `HttpClient` in `Basket.Api` for Aspire service discovery to `Ordering.Api`.
4. Implement `POST /baskets/demo-user/checkout`: read basket from Redis → error if empty → `POST /orders` on Ordering.Api with buyerId + items → delete the Redis basket key → return the new order id.
5. Update `EshopClaude.AppHost` wiring so `Basket.Api` has `WithReference`/`WaitFor` on both `Catalog.Api` and `Ordering.Api`.

**Projects touched:** `Basket.Api`, `EshopClaude.AppHost`.

**Verify:**
- Build.
- Aspire run.
- HTTP: `POST /baskets/demo-user/items` with a valid productId adds a correctly-priced/named item; repeating it increments quantity instead of duplicating.
- HTTP: `POST /baskets/demo-user/checkout` on a populated basket creates an order in Ordering.Api (confirm via `GET /orders/{id}`) and empties the basket (confirm via `GET /baskets/demo-user`); checkout on an empty basket returns an error.
- Traces: inspect the Aspire dashboard trace for a checkout call — confirm it shows the Basket.Api → Catalog.Api (add-item) and Basket.Api → Ordering.Api (checkout) hops.

---

## Step 5 — Storefront pages

Build the Blazor pages against the now-complete API surface, one use case at a time, replacing the template's placeholder pages/nav.

**Tasks:**
1. Replace `Home.razor` with the product list page: fetch `GET /products` from Catalog.Api, render name/description/price/image, "Add to cart" button calling `POST /baskets/demo-user/items`.
2. Add `/basket` page: fetch `GET /baskets/demo-user`, show items + running total, "Place Order" button calling `POST /baskets/demo-user/checkout`, redirect to `/orders/{id}` on success.
3. Add `/orders/{id}` page: fetch `GET /orders/{id}` from Ordering.Api, show order confirmation details.
4. Add `/orders` page: fetch `GET /orders?buyerId=demo-user`, list orders newest-first, each linking to `/orders/{id}`.
5. Update nav menu: remove Counter/Weather template entries, add Basket (with live item-count badge re-fetched after navigation/add-to-cart) and Orders entries.
6. Register typed `HttpClient`s in `EshopClaude.Web` for Catalog.Api, Basket.Api, Ordering.Api via Aspire service discovery; confirm AppHost wiring (`WithReference`/`WaitFor`) covers all three.

**Projects touched:** `EshopClaude.Web`, `EshopClaude.AppHost` (reference wiring if not already complete from Step 4).

**Verify:**
- Build.
- Aspire run.
- Browser: load `/`, see seeded products; add a product to cart, see nav badge update; visit `/basket`, see item and total; place order, confirm redirect to `/orders/{id}` with correct details; visit `/orders`, see the new order listed; click into it to confirm detail page matches.

---

## Step 6 — End-to-end verification

No new functionality — confirm the whole journey holds together under Aspire, across a cold start, and that nothing in earlier steps regressed.

**Tasks:**
1. Cold-start the whole system (`dotnet run` on AppHost from a clean state) and confirm every resource (Postgres, pgAdmin, Redis, three APIs, Web) reaches "Running"/healthy with no startup errors in logs.
2. Walk the full user journey in the browser per PROJECT.md: browse Home → add multiple products (including the same product twice, to verify quantity increment) → review Basket → Place Order → land on order confirmation → visit Order history and confirm the order appears → open it from history and confirm details match.
3. Verify basket-empty edge case: attempt checkout immediately after a fresh start (no items added) and confirm it surfaces an error rather than creating an empty order.
4. Inspect the Aspire dashboard end-to-end: structured logs free of unhandled exceptions, traces showing the full Web → Basket.Api → {Catalog.Api, Ordering.Api} call graph for one checkout.
5. Spot-check pgAdmin: `orderingdb` contains the orders created during this walkthrough with correct totals and items.

**Projects touched:** none (verification only).

**Verify:**
- Aspire run.
- Browser: full journey above, start to finish.
- Traces: dashboard trace view for the checkout request.
- pgAdmin: data sanity check in `orderingdb`.
