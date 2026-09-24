# Phase 2 — Working desktop widget

## Entry and required inputs
`start phase 2`: read `AGENTS.md`, brief, status, master plan, this file, Phase 1 document, and `preview/index.html`. The centered HTML, not the older screenshot, is the appearance reference.

Implement native functionality and establish window/material feasibility. Dedicated visual refinement belongs to Phase 3.

## Steps
- [ ] Read installed Windows/.NET/development-tool versions without building/testing/reviewing. Record compatible stable versions and missing prerequisites in `docs/environment.md`. Do not silently install a large IDE/workload or change framework; ask one concrete question if prerequisites block work.
- [ ] Create the C#/WinUI project under `src/GoalWidget/` with the master plan's file boundaries and stable pinned versions. Prefer unpackaged app plus later folder distribution for personal use, avoiding store/certificate requirements.
- [ ] Implement fixed 300-unit card, no visible title bar, ordinary non-topmost behavior, taskbar recovery. Establish native corner/shadow/backdrop limits and inactive/fallback appearance. Do not use unsupported Explorer integration for Win+D.
- [ ] Add centered default goal, provisional landscape, Edit goal/Quit context menu, keyboard access, and editing dialog. Validate nonblank text, 100 text elements, five explicit lines, and actual available space. Preserve line breaks.
- [ ] Implement versioned local state, serialized recoverable writes, and backup behavior from the master plan. Cancel preserves current state; save failure keeps the draft open for retry.
- [ ] Add dragging, move-end persistence, and scaling-aware position restoration into a connected display. First launch uses an upper-right margin. Editing controls/menu must not initiate window dragging.
- [ ] Keep one instance: repeated launch restores/activates the existing widget instead of creating another writer. Quit exits normally. Record taskbar and Win+D behavior/limitations.
- [ ] Perform only currently authorized checks. A visual walkthrough may use an available executable; do not build implicitly to obtain one. Without authorization, runtime behavior remains unverified.
- [ ] Update `STATUS.md` once with decisions, files, prerequisites, authorization, actual/omitted checks, next start.

## Intended acceptance evidence
When execution is authorized: edit/save/relaunch retains goal; Cancel and failed save preserve expected text; dragging restores position; removed monitors do not strand it; another app covers it; duplicate launch does not duplicate widgets; Quit works. Record native material observations or explicitly mark them unverified.

## Deliverables and stop
Core production source, `docs/environment.md`, handoff. Without build authorization report **implemented, not built or runtime-verified**. Stop before Phase 3; no final-art generation, visual redesign, or release packaging.
