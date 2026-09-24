# Phase 4 release checks

> Virtual desktop update: user means every Windows virtual desktop. Rebuilt and reopened with right-click **Show in Task View** (temporary visibility switch), zero warnings/errors; `artifacts/desktop-pin-setup-build.log`. To configure Windows pinning: enable that option, press Win+Tab, right-click Goal, choose **Show windows from this app on all desktops**, return to the widget and disable Show in Task View. Pinning has not yet been applied or verified, including after hiding Task View or signing in again.

> Latest update: the user manually deleted the old build; its absence was confirmed. Earlier deletion-block notes below are historical. Current-user Startup and Desktop shortcuts now point to `artifacts/compact-build/GoalWidget.exe`. Taskbar hiding and disabled minimization were rebuilt and reopened after explicit authorization: 18.75 seconds, zero warnings/errors (`artifacts/desktop-build.log`). Sign-in startup, Win+D and visual taskbar/Alt+Tab inspection remain unverified. No automated tests or review ran. The user wants one widget across Windows virtual desktops; pinning remains unverified.

Recorded 2026-09-24. **Release pending. Local executable built and opened; no published distributable.**

The compact build succeeded and opened: 210-unit card, 315 × 315 physical pixels at 150% scaling, centered saved text and mountain artwork. Folder size decreased from 231.4 to 174.9 MiB (24.4%). Evidence: `artifacts/compact-build.log` and `artifacts/widget-compact.png`. User accepts the current visual treatment for now; acceptance of the resized result has not been recorded. The taskbar complaint remains unresolved.

User's later `yes` authorized this compact rebuild/reopen and measurement. The latest request authorized old-build deletion and documentation updates only; automatic approval review blocked deletion and the old folder remains. No new build/test/review ran during cleanup. Automated tests, review and publishing/ZIP remain outside the recorded authorization.

## Authorization and preparation

User initially requested `start phase 4`, then replied `cool go ahead` to a build-and-launch attempt capped at about one hour. This authorized the minimal local tool setup, builds and concrete fixes, launch and visual assessment. It did not authorize automated tests, code review or publishing/packaging. The checkpoint succeeded in about 15 minutes; setup/build details and exact commands are in `environment.md`.

Read the required project/phase documents, artwork record, `global.json` and project settings. Inventory shows production source and artwork, but no listed release artifact or test project. Read-only command discovery found `winget`, but not `dotnet` or `msbuild`; the standard `C:/Program Files/dotnet` folder was not found. Phase 2's older Visual Studio/Windows SDK observations were not rechecked. These observations are preparation, not release verification.

## Pending evidence

Only results explicitly recorded below were observed. All other cases remain **not performed**. Expected outcomes describe requirements, not observed behavior. Further build/test work must stay within explicit authorization; use a disposable state location or separate Windows account for destructive data-failure exercises, preserving any real goal data.

| Case | Expected outcome | Result |
| --- | --- | --- |
| Prerequisites and Release build | Record actual installed tool versions, exact command, exit status and output path; resolve concrete failures | Passed local build: .NET 10.0.401, MSBuild 18.9.11, compact build succeeded in 52.09 seconds, zero warnings/errors. Output artifacts/compact-build; see environment.md and artifacts/compact-build.log |
| First launch, missing state | Default three-line goal appears near primary display's upper-right usable area | Observed after fixing startup accessibility subscription failure; no pre-existing goal data was found before launch |
| Editing and fitting | Short, 100-text-element, five-line, wrapped, Unicode and explicit-line-break goals remain centered; shrink only within 27.3–16.1 | Not performed |
| Invalid goals | Blank, over-limit and non-fitting text rejected with readable explanation; draft retained | Not performed |
| Save, Cancel, Escape, Enter | Save persists before success; Cancel/Escape leave goal unchanged; Enter adds a line | Not performed |
| Relaunch and duplicate launch | Goal/position retained; repeated launch restores one instance | Compact launch displayed existing saved text. Position retention and duplicate launch not verified |
| Save failure and retry | Draft remains available; no false success; retry works after failure clears | Not performed |
| State recovery | Malformed/unreadable/interrupted state preserves evidence; valid backup recovered; future schema blocks writes | Not performed |
| Drag and keyboard movement | Position saved at end of drag; Alt+arrows move; editing/menu interaction does not drag | Not performed |
| Monitor changes | Removal, negative coordinates and display rearrangement leave window reachable | Not performed |
| Display scaling | 100%, 125%, 150%, 200%; card size, typography, editor and hit areas usable | Not performed |
| Ordinary window behavior | Other apps cover widget; no repeated foreground activation; taskbar/shortcut recovery and Quit work | Not performed |
| Win+D observation | Record minimizing/restoring behavior; survival is optional | Not performed |
| Native appearance | Actual-size centered composition over mountains; light/dark surroundings and inactive state; record corner compromise and user acceptance separately | Compact card viewed at 150%: centered saved text and artwork rendered, client rectangle 315 × 315 physical pixels. Screenshot artifacts/widget-compact.png. Other settings and resized-result acceptance pending |
| Contrast | Measure composed goal contrast: at least 3:1 for large text, preferably 4.5:1; ordinary text at least 4.5:1 | Not performed |
| Accessibility | Visible keyboard focus, menu/editor navigation, Narrator labels, increased system text size | Not performed |
| System appearance settings | Disabled transparency, high contrast and reduced motion remain usable; restore any changed settings afterward | Not performed |
| Artwork in output | Generated PNG and notice included; provisional preview photograph excluded | Not performed |
| Local release folder and ZIP | All dependencies included or precise prerequisites documented; record paths and hashes | Not performed |
| Extracted-folder launch | Launch extracted ZIP, edit/save/quit/relaunch successfully; document actual runtime requirements | Not performed |
| Removal | Removing binaries/shortcut leaves goal data; separate optional data removal explained | Not performed |

## Known blockers and limits

- Minimal local build tools are now available. Recorded permission covered the initial and compact build/launch checkpoints; publishing/packaging and automated tests remain unauthorized.
- Fixed two observed issues: startup crash subscribing to high-contrast changes, and excess card height from ResizeClient. Current build/launch succeeds; no code review was authorized or performed.
- Generated-art provenance is recorded in `artwork.md`; packaging inclusion and native visual acceptance remain pending.
- No automated tests exist yet. If authorized, prioritize state recovery/save interruption, validation and monitor placement rather than decorative constants.
- No release readiness or user acceptance is claimed. Attempted editor opening through guarded keyboard input was inconclusive; no editor/save/cancel success is claimed. Some capture attempts showed a covering browser; only the final visible-card screenshot is native appearance evidence. System display/accessibility settings were not changed.

Resume here after authorization. Replace each result with actual evidence or a specific omission; record build/publish command, tool versions, artifact location and launch observations before declaring Phase 4 complete.

