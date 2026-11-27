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

### 2b–2e. Moved to the spec backlog

Catalog persistence, Catalog seed data, Ordering persistence and the Basket Redis connection are now specs in Step 3: `catalog-api/01`, `catalog-api/02`, `ordering-api/01` and `basket-api/01`.

---

## Step 3 — Microservices, spec by spec

Each service is built as a sequence of small specs, one feature branch per spec, following `_agenticdev/WOW.md`. Scope, non-goals and Definition of Done live in the spec files, and status lives in `_agenticdev/microservices/README.md`.

**Build order:** specs run in number order inside each service. Across services, `basket-api/03-add-item` needs `catalog-api/04-get-product`, and `basket-api/04-checkout` needs `ordering-api/02-create-order`. Suggested sequence: Catalog, then Ordering, then Basket.

### 3a. Catalog.Api

**Owns:** the product catalog, stored in `catalogdb`.

Specs in `_agenticdev/microservices/catalog-api/`:
1. `01-persistence.md`
2. `02-seed-products.md`
3. `03-list-products.md`
4. `04-get-product.md`

### 3b. Ordering.Api

**Owns:** placed orders and their line items, stored in `orderingdb`.

Specs in `_agenticdev/microservices/ordering-api/`:
1. `01-persistence.md`
2. `02-create-order.md`
3. `03-get-order.md`
4. `04-list-orders.md`

### 3c. Basket.Api

**Owns:** the demo user's basket in Redis, and checkout orchestration (calls Catalog.Api to add items and Ordering.Api to check out).

Specs in `_agenticdev/microservices/basket-api/`:
1. `01-redis-connection.md`
2. `02-get-basket.md`
3. `03-add-item.md`
4. `04-checkout.md`

---

## Step 4 — Service communication

Covered by the Basket.Api specs: `03-add-item.md` (Basket.Api → Catalog.Api) and `04-checkout.md` (Basket.Api → Ordering.Api). There is no separate work in this step.

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
