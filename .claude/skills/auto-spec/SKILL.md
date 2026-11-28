---
name: auto-spec
description: Run one spec end to end by sequencing /new-spec, /start-spec and /finish-spec, with two human gates (spec buildable, plan approved). Adds no steps of its own.
argument-hint: <service> <NN>
disable-model-invocation: true
---

# /auto-spec

Arguments: `$ARGUMENTS` → `<service> <NN>`.

## How to run a sub-skill

The three skills are user-invoked only, so run each one by reading `.claude/skills/<name>/SKILL.md` and following it exactly, with `<service> <NN>` as its arguments. Where it injects repository state (`` !`…` ``), run those commands yourself first. Never paraphrase, skip or reorder their steps — they are the single source of truth.

## Steps

1. Run **new-spec** `<service> <NN>`.
2. **Gate one.** Show me the completed spec and ask: *is this buildable?* Anything but yes → stop.
3. Run **start-spec** `<service> <NN>`.
4. **Gate two.** Its plan is the gate: wait for my approval. Anything but yes → stop.
5. Run **finish-spec** `<service> <NN>`.
6. **Refusals.** If any sub-skill refuses or stops, stop immediately. Report which skill, which check, and its suggested next step. Never work around a refusal.
7. Print the close-out summary from finish-spec.

## When not to use this

Run the three skills by hand, one session each, when:

- the spec introduces a pattern this repository has not built before;
- the spec touches more than one service;
- the spec's Definition of Done contains a check nobody can actually run.
