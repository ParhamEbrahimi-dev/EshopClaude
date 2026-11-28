---
name: start-spec
description: Start one spec of the WOW.md cycle (steps 2–4) — preflight, create its local feature branch from main, commit the spec file as the branch's first commit, then plan the implementation in plan mode and stop for approval. Never implements, never pushes.
argument-hint: <service> <NN>
disable-model-invocation: true
allowed-tools: Read, Glob, Bash(git status:*), Bash(git branch:*), Bash(git checkout:*), Bash(git add:*), Bash(git commit:*)
---

# /start-spec

## Repository state

- Current branch: !`git branch --show-current`
- Working tree:
!`git status --porcelain`

Arguments: `$ARGUMENTS` → `<service>` (folder name, e.g. `ordering-api`), `<NN>` (two digits).

## Hard rules

- Only these git commands: `status`, `branch`, `checkout`, `add`, `commit`. Never push, merge or rm. Feature branches stay local.
- Never write code. The skill ends at an approved-or-not plan.
- On refusal, change nothing, and say **which check failed** and **what to do next**.

## Steps

1. **Resolve.** Glob `_agenticdev/microservices/<service>/<NN>-*.md`. None → refuse: run `/new-spec <service> <NN> <slug>`. More than one → refuse and list them.
2. **Preflight.** Read the spec and `_agenticdev/microservices/README.md`. Refuse if any of these is true:
   - Current branch is not `main` → `git checkout main` first.
   - Working tree has entries other than the spec file → commit or stash them first.
   - A **Depends on** entry is not `Done` in the README table → finish that spec first (name it).
   - A section is empty, still holds a template comment (`<!-- … -->`) or `<placeholder>` → run `/new-spec <service> <NN>` first (name the sections).
   - Spec Status or its README row is `Done` → nothing to start; pick the next spec from the README.
3. **Branch name.** Apply the WOW.md rule `feature/<service>-<NN>-<short-name>`, with `<service>` = the folder name and `<short-name>` = the slug from the spec file name. If the README row's Branch column differs, refuse and show both. Never invent or ask for a name. If that branch already exists (`git branch --list <name>`), refuse.
4. **Create** the branch: `git checkout -b <branch> main`.
5. **Spec commit.** `git add` the spec file only, then commit:
   `Add spec <service>/<NN>-<slug>: <title> (spec commit, before implementation)`
   If the spec is already committed unchanged on `main`, make the same commit with `--allow-empty`, so the branch still opens with its spec commit. Say which one you did.
6. **Plan.** Enter plan mode. Plan only the spec's Scope/Tasks, within its Files touched, and respect every Explicit non-goal. Map each Definition of Done line to how it will be verified. Read the files in `src/` the plan touches; change none.
7. **Stop.** Present the plan for approval and end there. Do not implement anything; implementation is step 5 of the WOW.md cycle and starts only once the plan is approved.
