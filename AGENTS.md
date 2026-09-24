# Project instructions

## Communication
Use plain words and short updates. Explain technical terms and a skill/subagent on first use. Say what you will do before doing it. Ask one focused question at a time. Never invent checks, prior progress, or user acceptance.

## Starting a phase
`start phase N` is sufficient instruction. Read, in order:
1. This file.
2. `docs/product-brief.md`.
3. `STATUS.md`.
4. `docs/superpowers/plans/2026-09-24-goal-widget.md`.
5. The matching document in `docs/phases/` and its required prior outputs.

Execute only that phase; resume recorded progress. Phases are numbered 1–4, with no Phase 0. If a requested phase is undefined, ask one focused question rather than remapping it. Report missing prerequisites instead of inventing prior completion.

## Authorization
- Do not run build commands, test commands, or code review without the user's explicit request for that activity in the current phase/task. Starting a phase, including Phase 4, does not authorize these activities.
- Do not bypass this through IDE actions, launch commands that build first, packaging/publish commands, scripts, automation, or subagents.
- Record authorization and scope in `STATUS.md`; it does not carry into another phase.
- Reading code needed for authorized implementation is allowed; unsolicited code review is not.
- Manual visual walkthroughs are separate from build/test commands, subject to current instructions. Do not trigger an unauthorized build to obtain an executable for a walkthrough.
- User restrictions override skill templates, including automatic testing, review, delegation, and cross-phase execution.

## Scope and handoff
The product brief defines scope; references and future ideas do not expand it. At phase end update `STATUS.md` once with decisions, actual progress, changed files, checks performed/omitted, open issues, and next starting point. Include this update in the phase commit if committing; no extra commit to record its own number. Do not initialize Git or invent a commit just because a phase ended. Give a short completion message and stop before the next phase.
