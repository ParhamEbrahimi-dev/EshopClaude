# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & run

Run from `src/`:

- Build: `dotnet build` (builds the solution)
- Run: `dotnet run --project EshopClaude.AppHost` (Aspire)

## Conventions

- Target framework: `net10.0` everywhere.
- Minimal APIs — no controllers.
- Every API project's `Program.cs` calls `AddServiceDefaults()` and `MapDefaultEndpoints()`.
- Project naming: `EshopClaude.<Name>` (e.g. `EshopClaude.Catalog.Api`).
- AppHost resource naming: `catalog-api`, `basket-api`, `ordering-api`, `webfrontend`.

## Architecture rules

- Three microservices — Catalog.Api, Basket.Api, Ordering.Api — plus a Blazor storefront, orchestrated by the Aspire AppHost.
- Database per service: one PostgreSQL server with `catalogdb` (Catalog) and `orderingdb` (Ordering); Redis for Basket. No service reads another service's database.
- Checkout is owned and orchestrated by Basket.Api.
- Services talk over synchronous HTTP through Aspire service discovery. No API gateway, no message broker, no events.
- Out of scope: login, identity, payments, deployment. Don't propose them.

## Workflow

- Foundation work is committed on `main`. From the first spec, one spec is one sub-feature on one feature branch, merged with a pull request.
- Before committing, the solution must build and `aspire run` must start every resource healthy.
- Detailed scope lives in `_agenticdev/PROJECT.md` and build order in `_agenticdev/PLAN.md`. Reference them, don't copy them.
