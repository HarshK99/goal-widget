# Goal Widget Implementation Plan

**Goal:** Deliver one elegant, editable Windows 11 goal widget using the accepted centered landscape-glass design.

**Architecture:** One desktop process owns the card and editing dialog. One local file stores goal/position; artwork and Windows behavior remain separate from content.

**Tech stack:** C#, WinUI 3 (Microsoft's Windows interface toolkit), Windows App SDK (window/material tools), XAML (interface layout), JSON (local text storage). Select and pin supported stable versions in Phase 2 after checking installed tools; no preview dependencies.

**Spec:** `docs/product-brief.md`.

**Execution:** Follow `AGENTS.md`, one requested phase per chat. User restrictions override skill templates: no automatic builds, tests, reviews, delegation, or next-phase execution. This planning deliverable intentionally contains no production code.

## Stack decision
A Windows-only MVP with a small interface favors native window and material support. WinUI provides system backdrops and AppWindow window management. The HTML prototype establishes appearance without committing the production app to a browser engine. Tauri is an alternative only if a concrete native limitation warrants revisiting the choice; do not switch silently.

Acrylic blur is enhancement, not a requirement for a usable card. Landscape and tinted layers must preserve the composition when OS effects change. The native outer corner shape may differ from the 30-unit preview radius; establish the compromise in Phase 2 before refinement.

Sources: [WinUI overview](https://learn.microsoft.com/en-us/windows/apps/get-started/winui-get-started-overview), [window management](https://learn.microsoft.com/en-us/windows/apps/develop/ui/windowing-overview), [backdrops](https://learn.microsoft.com/en-us/windows/apps/develop/ui/system-backdrops), [material fallbacks](https://learn.microsoft.com/en-us/windows/apps/develop/ui/materials).

## Global constraints
Windows 11 only; no timer/Apple implementation. Centered goal in compact landscape square. Ordinary non-topmost window; Win+D optional, no Explorer attachment. Local data only; no server, accounts, analytics, or database engine. Builds, tests, packaging builds, and code review require explicit authorization for that activity in the current phase. Update handoff and stop at each phase boundary.

## Planned file responsibilities
These production paths do not exist yet. Add template-required bootstrap/manifest files and record any actual path differences in the handoff.

| Path | Responsibility |
|---|---|
| `src/GoalWidget/GoalWidget.csproj` | Pinned stable dependencies and Windows configuration |
| `src/GoalWidget/App.xaml` and `.cs` | Startup, resources, single instance, saved-state restore |
| `src/GoalWidget/WidgetWindow.xaml` and `.cs` | Card, context menu, editing coordination |
| `src/GoalWidget/EditGoalDialog.xaml` and `.cs` | Draft, validation, Save/Cancel, save errors |
| `src/GoalWidget/GoalState.cs` | Versioned content/placement model |
| `src/GoalWidget/GoalStore.cs` | Read/write, recovery, safe replacement |
| `src/GoalWidget/WindowPlacement.cs` | Scaling, display bounds, move completion |
| `src/GoalWidget/Styles/WidgetResources.xaml` | Typography, palette, glass layers, fallback |
| `src/GoalWidget/Assets/landscape.jpg` | Final cleared artwork |
| `docs/environment.md` | Exact prerequisites/versions/commands; distinguish documented from executed |
| `docs/artwork.md` | Artwork source and distribution permission |
| `docs/release-checks.md` | Actual observations and omitted checks |
| `docs/install.md` | Distribution, launch, quit, restore, removal |

Use small focused classes and standard controls, not a general service framework or mobile project.

## Data model and boundaries
Save `%LOCALAPPDATA%/GoalWidget/state.json` with `schemaVersion` (integer 1), `goalText` (plain text, normalized newlines), and optional `placement` (physical x/y screen coordinates, monitor identifier, DPI at save time). Fixed size/artwork are app resources, not settings. No speculative history/timer/sync fields.

`GoalStore` consumes/produces `GoalState`, independent of views. `WindowPlacement` combines saved placement with current display information to produce visible bounds. Editor submits a validated draft; update visible text only after successful save. Serialize edits/position writes through one owner to prevent lost updates.

Write a temporary file in the same directory, atomically replace state, and retain a last-valid backup. Missing state uses defaults. Corrupt/unreadable state is preserved; try backup and explain recovery if defaults are needed. Never overwrite an unsupported future schema version. A failed position save receives a nonmodal message instead of a false success claim.

## Milestones
| Phase | Inputs | Deliverable / stop |
|---|---|---|
| 1 — Design and plan | User scope, PNG, feedback | Approved direction, brief, plan, phase instructions; stop before production code |
| 2 — Working desktop widget | Phase 1 documents, preview | Window, editing/storage, position, quit/recovery, native feasibility; stop before refinement |
| 3 — Visual finish | Phase 2 actual outputs/status | Landscape/glass/typography, cleared art, interaction/accessibility polish; stop before release |
| 4 — Check and package | Phase 3 outputs, current authorization | Recorded checks/fixes, authorized distributable, install guide; stop before new features |

Detailed phase instructions: `docs/phases/01-design.md` through `04-check-and-package.md`.

## Verification policy
Acceptance criteria are not permission to run commands. Without build authorization report implementation as unbuilt/unverified. Phase 4 cannot be marked complete while required artifact creation or release verification is pending.

When tests are requested, prioritize malformed/missing/future-version state, interrupted save, invalid/overflowing goals, and disconnected displays/negative coordinates/scaling changes. Avoid tests duplicating decorative constants. No tests are authored/run in this planning phase.

## Parked future concepts
Timer exploration is deferred along with the timer. Candidates to revisit: notched rotary knob, circular duration arc, vertical minute wheel, or quiet duration presets. None belongs in current deliverables. Apple widgets and sync need separate scope discussions.
