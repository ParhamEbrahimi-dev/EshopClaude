# EshopClaude — Project Design

A minimal e-commerce demo built to show agentic coding with microservices: small scope, few tokens, fast delivery. Built on the .NET Aspire starter solution in `src/`.

## Goals / non-goals

**In scope:** list products, add to cart, checkout, view order (including order history).
**Out of scope:** login/identity, payments, deployment, API gateway, message broker / events, retries/fallbacks/resilience beyond Aspire's defaults, product search/filtering/categories, pagination, basket or stock quantity validation, automated tests, order status/state machine, basket editing (remove/change quantity), product detail pages, stock/inventory tracking.

## Topology

- **Catalog.Api** — owns product data. Backed by Postgres database `catalogdb`.
- **Basket.Api** — owns basket data and orchestrates checkout. Backed by Redis.
- **Ordering.Api** — owns order data. Backed by Postgres database `orderingdb`.
- **EshopClaude.Web** — Blazor Server (InteractiveServer) storefront. Calls all three APIs directly over HTTP via Aspire service discovery. No API gateway.
- **EshopClaude.AppHost** — orchestrates all of the above, plus one Postgres server resource (hosting both `catalogdb` and `orderingdb`, with pgAdmin enabled for inspecting both) and one Redis resource.
- The existing generic `EshopClaude.ApiService` placeholder project is removed; it's replaced by the three real services.

All inter-service calls are synchronous HTTP through Aspire service discovery (e.g. `https+http://catalogapi`). No message broker, no events.

## Basket identity

There is no login, so there is no real multi-user concept. A single hard-coded constant, `BuyerId = "demo-user"`, is used everywhere a buyer id is needed (Redis key for the basket, `BuyerId` column on orders, order-history query). This keeps the demo to a single, always-available basket with zero session/cookie plumbing.

## Catalog.Api

**Owns:** product catalog. Postgres database `catalogdb`.

**Product entity:**
| Field | Type | Notes |
|---|---|---|
| Id | int | PK |
| Name | string | |
| Description | string | |
| Price | decimal | |
| ImageUrl | string | |

No stock/inventory field — stock tracking is out of scope.

**Endpoints:**
- `GET /products` — list all products (no paging, no filtering/search).
- `GET /products/{id}` — get one product. Used by Basket.Api to snapshot name/price when an item is added.

**Seed data:** a small fixed set of products (e.g. 6–8) seeded at startup — enough to make the list page look real.

## Basket.Api

**Owns:** the current basket. Redis-backed, no Postgres.

**Basket shape (stored in Redis under key `demo-user`):**
```json
{
  "buyerId": "demo-user",
  "items": [
    { "productId": 1, "productName": "...", "unitPrice": 9.99, "quantity": 2 }
  ]
}
```
Price and name are **snapshotted at add-time** — Basket.Api calls `GET /products/{id}` on Catalog.Api once, when the item is added, and stores the result. Viewing the basket or checking out never calls Catalog.Api again; the basket is self-contained after that point.

**Endpoints:**
- `GET /baskets/demo-user` — return the current basket (empty if none exists yet).
- `POST /baskets/demo-user/items` — add a product (`productId`, `quantity`) to the basket. Calls Catalog.Api to resolve name/price; if the product already exists in the basket, increments quantity. No remove/update-quantity endpoint — add-only.
- `POST /baskets/demo-user/checkout` — orchestrates checkout (see below). Returns the new order id.

## Checkout flow (owned by Basket.Api)

Triggered by the storefront's "Place Order" button, which takes no input fields. Step by step:

1. Web calls `POST /baskets/demo-user/checkout`.
2. Basket.Api reads the current basket from Redis. (If it's empty, the endpoint just returns an error — no special handling beyond that.)
3. Basket.Api calls `POST /orders` on Ordering.Api, passing `buyerId` and the basket's items (productId, productName, unitPrice, quantity) as the order payload.
4. Ordering.Api creates the order (computing/storing the total) and returns the new order id.
5. Basket.Api deletes the Redis key for `demo-user`, clearing the basket.
6. Basket.Api returns the order id to Web.
7. Web redirects to the order confirmation page for that order id.

No retries, no compensation, no saga — a failure at any step simply surfaces as an error (Aspire defaults only), consistent with the rest of the demo.

## Ordering.Api

**Owns:** placed orders. Postgres database `orderingdb`.

**Order entity:**
| Field | Type | Notes |
|---|---|---|
| Id | int | PK |
| BuyerId | string | always `"demo-user"` |
| CreatedDate | DateTime | |
| Total | decimal | sum of line items, computed at creation |

**OrderItem entity (child of Order):**
| Field | Type | Notes |
|---|---|---|
| ProductId | int | |
| ProductName | string | snapshot, passed in from Basket.Api |
| UnitPrice | decimal | snapshot |
| Quantity | int | |

No `Status` field — an order simply exists once created, no state machine.

**Endpoints:**
- `POST /orders` — create an order from a buyerId + list of items. Called by Basket.Api during checkout. Returns the created order (including its id).
- `GET /orders/{id}` — get a single order. Used by the post-checkout confirmation page.
- `GET /orders?buyerId=demo-user` — list all orders for the buyer, newest first. Used by the order-history page. (Called directly by Web, not through Basket.Api.)

## Storefront (EshopClaude.Web) — pages and journey

- **`/` (Home / product list)** — replaces the current placeholder `Home.razor`. Lists all products (name, description, price, image) from Catalog.Api, each with an "Add to cart" button. No per-product detail page.
- **`/basket`** — shows current basket contents (from Basket.Api), running total, and a "Place Order" button. Add-only: no remove/quantity-edit controls.
- **Nav menu** — shows a live badge with the current basket item count (re-fetched from Basket.Api after navigation/add-to-cart). Template's Counter/Weather pages and nav entries are removed.
- **`/orders/{id}`** — order confirmation page, navigated to immediately after checkout; shows the order just placed (from Ordering.Api).
- **`/orders`** — order history list for `demo-user` (from Ordering.Api), each entry linking to its `/orders/{id}` detail.

**User journey:** Home (browse + add to cart) → Basket (review + Place Order) → Order confirmation (`/orders/{id}`) → optionally Order history (`/orders`) to look back at past orders.

## AppHost wiring

`EshopClaude.AppHost` adds:
- A Postgres server resource, with two databases: `catalogdb` (Catalog.Api) and `orderingdb` (Ordering.Api). pgAdmin is enabled on the Postgres server resource (`WithPgAdmin()`) so both databases can be browsed during development.
- A Redis resource (Basket.Api).
- The three API projects (Catalog.Api, Basket.Api, Ordering.Api), each referencing its data resource.
- The Web project, referencing all three APIs (`WithReference` + `WaitFor`), replacing its current sole reference to the generic `apiservice`.
- The generic `EshopClaude.ApiService` project and its AppHost registration are removed.
