---
name: new-spec
description: Create or complete one feature spec under _agenticdev/microservices for the spec-driven workflow in _agenticdev/WOW.md. Drafts a new spec from the template, or audits an existing one and fills its gaps. Markdown only — no branches, commits or code.
argument-hint: <service> <NN> [slug] ["title"]
disable-model-invocation: true
allowed-tools: Read, Write, Edit, Glob
---

# /new-spec

Arguments: `$ARGUMENTS` → `<service>` (folder name, e.g. `catalog-api`), `<NN>` (two digits), optional `[slug]` (kebab-case), optional `["title"]`.

## Hard rules

- Write only Markdown under `_agenticdev/`. Never touch `src/`, never create a branch, never commit.
- Never overwrite an existing spec file — edit it in place, section by section.
- Leave **Status: Not started**. Never advance it, even if code seems to exist.
- Keep every section of `_agenticdev/templates/spec-template.md`, in its order.

## Steps

1. **Resolve.** Glob `_agenticdev/microservices/<service>/<NN>-*.md`. More than one match → stop and ask which.
2. **New spec** (no match). Require a slug; if missing, ask for it. Read the template, `_agenticdev/PROJECT.md` and `_agenticdev/PLAN.md`. Create `<service>/<NN>-<slug>.md` with Status `Not started`, drafting each section from the matching parts of PROJECT.md and PLAN.md. Title: the given one, else derived from the slug.
3. **Existing spec** (one match). Read it and report, section by section: **Complete**, **Stub** (template comment or empty) or **Too vague** (say what is missing to build from it). Then continue to fill the gaps.
4. **Status table.** Read `_agenticdev/microservices/README.md`. Fill **Depends on** from it: earlier specs of the same service, plus any other spec this one needs. If the spec has no row, add one in build order, with the branch named like the existing rows for that service and Status `Not started`.
5. **Ask before writing.** List the design decisions you cannot answer from the documents — typically field names and their types, how the database schema is created, and how the service gets its connection string. Ask them in chat, **one question per decision**, each with a recommended default. Wait for the answers.
6. **Write the spec.** Fill it with the answers:
   - **Context** → the Goal section: why it exists, what it unlocks.
   - **Data model** → a `## Data model` section after Scope: entities, fields, types, keys.
   - **Tasks** → the Scope checklist, one `- [ ]` per task, one service only.
   - **Files touched** → a `## Files touched` section after Data model: paths under `src/`, limited to this service and its AppHost registration.
   - **Definition of Done** → every line a runnable check: the exact command or HTTP request, and the expected result (status code, body, dashboard state, table rows). Always include `dotnet build` and `aspire run` with every resource healthy. No "it works".
7. **Report.** Print the spec path, whether the README row was added, and a list of anything still to be decided by hand.
