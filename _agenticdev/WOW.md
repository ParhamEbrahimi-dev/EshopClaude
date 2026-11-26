# EshopClaude — Way of Working

How our team and Claude Code work on this repo. Scope: `PROJECT.md`. Build order: `PLAN.md`. Conventions and architecture rules: `CLAUDE.md`.

## Principles

- One spec = one sub-feature = one feature branch = one merge to `main`.
- A spec changes exactly one microservice and its own AppHost registration.
- `main` always builds and runs. Every merge leaves a working solution.
- Every spec is self-contained: a fresh session only needs to read that one file.

## Branches and merges

- One spec, one feature branch, branched from `main`.
- Branch name: `feature/<service>-<NN>-<short-name>` (e.g. `feature/catalog-01-list-products`).
- The spec file is the first commit on the branch, before any code.
- Merge into `main` with `git merge --no-ff`, then delete the branch.
- Feature branches stay local and are never pushed.
- No direct commits to `main`.

## Folder layout

```
_agenticdev/
  templates/spec-template.md       # every spec follows this
  microservices/
    README.md                      # status table for all specs
    <service>/NN-<short-name>.md   # e.g. catalog/01-list-products.md
```

- Specs are numbered per service (`01`, `02`, …) and never renumbered.
- Every spec follows `_agenticdev/templates/spec-template.md`.
- New spec: copy the template, keep every section, and fill it in before branching.
- `_agenticdev/microservices/README.md` holds the status table for all specs.

## The cycle, per spec

1. **Pick** the next spec from the README status table (see Ordering rules).
2. **Branch:** create `feature/<service>-<NN>-<short-name>` from `main`; commit the spec file first.
3. **Fresh session:** start a new Claude Code session (clear context).
4. **Plan:** in plan mode, Claude reads the spec and proposes a plan; a human approves it.
5. **Implement** in auto mode, following the approved plan.
6. **Verify manually:** build, `aspire run`, and every item in the spec's Definition of Done.
7. **Commit and merge:** commit on the branch, then `git merge --no-ff` into `main`.
8. **Mark Done** in the spec file and in the README status table.
9. **Delete** the feature branch.

## Ordering rules

- Specs run in number order inside a service.
- A spec does not start until every spec it depends on is merged to `main`.

## Session hygiene

- One spec, one fresh session. Clear the context between specs.
- Plan mode before implementing; a human approves the plan.
- Before any commit:
  - `dotnet build` passes.
  - `aspire run` starts every resource healthy.
  - The spec's Definition of Done passes.
