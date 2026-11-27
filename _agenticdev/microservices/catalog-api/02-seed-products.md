# Catalog.Api — 02 — Seed products

**Status:** In progress

## Goal

Fill the catalog with a small, fixed set of products so the list endpoint and the storefront have real-looking data.

## Context

- catalog-api/01 left `Program.cs` with a startup scope that resolves `CatalogDbContext` and calls `Database.EnsureCreated()`. Seeding runs in that same scope, right after `EnsureCreated()`.
- **Guard:** seed only when `Products` has no rows (`!db.Products.Any()`). If any row exists, nothing is inserted.
- **Ids** come from the identity column. The seed code doesn't set them.
- **Image URLs** are external placeholders (`https://placehold.co/600x400?text=<Name>`). There are no image assets and no static-file hosting in scope.
- The Postgres container has no data volume, so every `aspire run` starts empty. Restarting the whole AppHost proves nothing about duplicates, so the restart check below restarts only `catalogapi`.

## Depends on

catalog-api/01-persistence

## Seed data

| Name | Description | Price | ImageUrl |
|---|---|---|---|
| Classic Mug | A 350 ml ceramic mug for coffee or tea. | 12.50 | `https://placehold.co/600x400?text=Classic+Mug` |
| Canvas Tote Bag | A sturdy cotton canvas bag for everyday carry. | 18.00 | `https://placehold.co/600x400?text=Canvas+Tote+Bag` |
| Cotton T-Shirt | A soft, regular-fit t-shirt in 100% cotton. | 24.99 | `https://placehold.co/600x400?text=Cotton+T-Shirt` |
| Hoodie | A warm fleece-lined hoodie with a front pocket. | 49.99 | `https://placehold.co/600x400?text=Hoodie` |
| Baseball Cap | An adjustable six-panel cap with a curved brim. | 19.95 | `https://placehold.co/600x400?text=Baseball+Cap` |
| Notebook | An A5 dotted notebook with 120 pages. | 9.99 | `https://placehold.co/600x400?text=Notebook` |
| Water Bottle | A 750 ml insulated steel bottle. | 22.00 | `https://placehold.co/600x400?text=Water+Bottle` |
| Sticker Pack | A pack of 10 vinyl stickers. | 4.50 | `https://placehold.co/600x400?text=Sticker+Pack` |

## Scope

- [ ] Seed 6–8 products (name, description, price, image URL) at startup.
- [ ] Seed only when the products table is empty.

## Explicit non-goals

- No endpoints.
- No admin or import feature, and no seed configuration.
- No entity or schema changes.

## Tasks

1. Add `CatalogSeed.cs` to Catalog.Api: a static class with `Seed(CatalogDbContext db)` that returns when `db.Products.Any()` is true, otherwise adds the 8 products from the Seed data table and calls `SaveChanges()`.
2. In `Program.cs`, call `CatalogSeed.Seed(db)` right after `db.Database.EnsureCreated()`, inside the existing scope.
3. Build, run, and go through every Definition of Done check.

## Files touched

- `src/EshopClaude.Catalog.Api/CatalogSeed.cs` (new)
- `src/EshopClaude.Catalog.Api/Program.cs`: one call after `EnsureCreated()`

No entity, DbContext, AppHost, csproj or appsettings changes.

## Definition of Done

1. `dotnet build` in `src/` prints `Build succeeded` with 0 errors.
2. `aspire run` in `src/`: the dashboard shows every resource Running/Healthy: postgres, pgadmin, catalogdb, orderingdb, redis, catalogapi, basketapi, orderingapi, webfrontend.
3. pgAdmin Query Tool on `catalogdb`: `SELECT count(*) FROM "Products";` returns `8`.
4. pgAdmin Query Tool: `SELECT "Name", "Price", "ImageUrl" FROM "Products" ORDER BY "Id";` returns the 8 rows from the Seed data table, in that order, with no empty fields.
5. Dashboard → `catalogapi` → Restart (only this resource): it comes back Healthy, its console log has no exceptions, and check 3 still returns `8`.

## References

- PROJECT.md: "Catalog.Api" (Seed data).
- PLAN.md: "Step 3 — Microservices, spec by spec" › "3a. Catalog.Api".
