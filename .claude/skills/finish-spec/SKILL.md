---
name: finish-spec
description: Finish one spec of the WOW.md cycle (steps 5–9) on its feature branch — implement the approved plan, build, walk the Definition of Done with the human, mark Done, then merge --no-ff into main after explicit confirmation, delete the branch and push main.
argument-hint: <service> <NN>
disable-model-invocation: true
allowed-tools: Read, Write, Edit, Glob, Bash(git:*), Bash(dotnet build:*)
---

# /finish-spec

## Repository state

- Current branch: !`git branch --show-current`
- Commits so far:
!`git log --oneline main..HEAD`
- Working tree:
!`git status --porcelain`

Arguments: `$ARGUMENTS` → `<service>` (folder name, e.g. `ordering-api`), `<NN>` (two digits).

## Hard rules

- Never push a feature branch. Push only `main`, only after the merge.
- Never `git add -A` or `git add .` — stage by explicit path.
- Never resolve a merge conflict yourself, never force anything, never skip hooks.
- Any failed check or any "no" from me stops the skill before the merge, with what failed and what to do next.

## Steps

1. **Preflight.** Glob `_agenticdev/microservices/<service>/<NN>-*.md` and derive the branch `feature/<service>-<NN>-<slug>` (WOW.md rule, slug from the file name). Stop if:
   - the current branch is `main` → never run on main;
   - `git branch --list <branch>` is empty → run `/start-spec <service> <NN>` first;
   - the current branch is not `<branch>` → `git checkout <branch>` first.
2. **Develop.** Implement the spec on this branch following the plan already approved in this session (no approved plan → stop and ask for one). Stay inside its Scope and Files touched, honour every Explicit non-goal and the CLAUDE.md conventions. Commit as you go, staging by path, messages naming the spec.
3. **Verify.** From `src/`, run `dotnet build`; any error stops the skill. Then walk the spec's Definition of Done line by line:
   - run the checks you can run yourself and record the result;
   - for checks you cannot run (`aspire run` healthy, a table or row in pgAdmin, a request in the `.http` file), ask me one at a time with the exact thing to look at and the expected result, and wait.
   Any "no" stops here.
4. **Mark Done.** Run `git rev-parse HEAD`. Set the spec's line to `**Status:** Done (<sha>)`.
5. **README.** Set this spec's row in `_agenticdev/microservices/README.md` to `Done`.
6. **Doc commit.** `git add <spec path> _agenticdev/microservices/README.md`, then commit `Mark spec <service>/<NN>-<slug> Done`.
7. **Confirm, then merge.** Show me: branch `<branch>` → target `main`, the commits from `git log --oneline main..HEAD`, and that `main` will then be pushed to `origin`. Wait for an explicit yes. Then:
   `git checkout main` → `git merge --no-ff <branch> -m "Merge <branch>: <title> (spec <service>/<NN>)"` → on conflict, `git merge --abort`, return to the branch and stop → `git branch -d <branch>` → `git push origin main`.
8. **Report.** The merge commit SHA, the files changed (`git diff --stat HEAD^1 HEAD`), and a table of every Definition of Done line with how it was verified: run by Claude (with result) or confirmed by me.
