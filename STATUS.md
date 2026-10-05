# Goal widget — handoff

## Status
Rebuilt from scratch on 2026-10-05 as a desktop-level WPF widget. Installed and running from `%LOCALAPPDATA%\Programs\GoalWidget\GoalWidget.exe`. Phase 4 release is still pending. Source: https://github.com/HarshK99/goal-widget (public).

## Why it was rebuilt
The user reported the widget coming to the front when switching virtual desktops. The cause was the window model chosen in the first brief: an ordinary app window, hidden from the taskbar afterwards and pinned to all desktops by hand. Windows gives focus to pinned app windows on a desktop switch. The user asked to redo it from first principles and said the brief was wrong and could be updated. `docs/product-brief.md` is revised.

## What exists now
- `src/GoalWidget` is a WPF app on .NET Framework 4.8 (built into Windows 11). App folder 3.01 MiB in 14 files; the WinUI build was 174.9 MiB.
- `DesktopPin.cs` holds the window model: unowned tool window, never-activate, kept at the bottom of the stack, raised only by the app itself (tray "show above", or Show Desktop).
- `WidgetWindow` draws the card itself: 21-unit corners, rim, shadow, the same artwork and colors.
- `TrayIcon.cs`: tray icon plus the one menu shared by the icon and the card (Edit goal, Show above other windows, Quit).
- `EditGoalWindow`: ordinary editor window. Double-click the card also opens it.
- `GoalStore.cs` and `GoalState.cs` carried over with the same behavior and file format; the user's goal was read back correctly.
- `Log.cs`: always-on log at `%LOCALAPPDATA%\GoalWidget\log.txt` (start, quit, crashes, Show Desktop changes, unsaved positions).
- `tools/install.ps1` copies a build to `%LOCALAPPDATA%\Programs\GoalWidget`, points the Startup shortcut there and starts it.

## Authorization recorded (2026-10-05)
- User asked to check the pop-up and "what other things can be fixed", which requested the code read-through reported that day.
- User replied "great - fix from scratch - you can update the brief - it was wrong - yes go ahead with option 2" to the rebuild proposal, which included building. Scope: the rebuild, its build, installing outside the build folder and the crash log. No automated tests, formal code review, publish or ZIP.
- User replied "perfect - update docs - remove redundant old irrelevant files and code - and then push to github". Scope: docs, cleanup, commit and push. No build ran for this.

## Checks performed
- Build: passed first time, 33.72 seconds, zero warnings/errors; `artifacts/wpf-build.log`.
- Window flags read from the running widget: tool window yes, never-activate yes, app-window no, topmost no, no owner, not the foreground window.
- Stacking order: widget 18th and the desktop 19th of 20 visible windows, so it sits directly above the desktop.
- Virtual desktops: Windows returns no desktop ID for the widget (error 0x8002802B) while an ordinary app window has one.
- Rendering: a capture of the window shows the rounded card, artwork and centered saved goal at 150% scaling.
- Saved goal and position were read from the existing `state.json`; the card is at the saved spot.
- Graceful quit and restart worked. Tray icon registered with Windows for the installed path.
- Startup shortcut points to the installed exe and the target exists.

## User-reported results (2026-10-05)
"drag is working, desktop switching is perfect now, editing as well." The saved file confirms an edited goal and a new position. The original pop-up problem is resolved.

Win+D: the user first wrote "win+d working", then clarified that Win+D hides the widget and pressing it again shows it. That is a failure against the brief, which wants the widget to stay visible. An earlier note here claiming Windows kept it visible was wrong.

Diagnosis (scripted Show Desktop toggle sampled every 20 ms): Windows raises the desktop window (`Progman`) above every app and above the widget, and sends no foreground-change event when it does. `DesktopPin` relied on that event, so it only noticed when Show Desktop was ending, raised the widget for half a second and dropped it. A first fix that assumed a wrong reading did not help and was replaced.

Fix, built and installed (user authorized the rebuild with "okay"; two builds, zero warnings/errors, `artifacts/wpf-build.log`): `DesktopPin` now polls the desktop's position every 250 ms, reading a few window handles and changing nothing unless the state changed. The widget counts as "over the desktop" only while the focus is on the shell or on the widget's own process, so it drops back the moment an app returns. Measured: raised about 200 ms after the toggle, held for the 3 seconds measured, back at the bottom within 35 ms of toggling off, never above the returning windows. The user has not yet confirmed it with the real Win+D key.

## Not checked
Right-click menus, tray clicks, start at the next sign-in, and the remaining rows in `docs/release-checks.md`. No automated tests exist; no formal code review was run on the new code.

## Cleanup done (2026-10-05)
- Removed from the repository: the WinUI source files, `docs/phases/01`–`03`, the old implementation plan, and the historical WinUI sections of `environment.md` and `release-checks.md`. `AGENTS.md` now lists only Phase 4; `docs/phases/04-check-and-package.md` was rewritten for the WPF app.
- Removed from the local `artifacts/` folder (never in Git): the old 174.9 MiB WinUI app, its build logs, capture script and screenshots. `artifacts/widget-build` and `artifacts/wpf-build.log` remain.
- Kept: `preview/` (design reference), the build tools under `D:/FounderMode/Tools/goal-widget`, and the user's goal data.

## Open issues
- Known gaps are listed in the brief under Quality: keyboard access to the card, the Windows text-size setting, the editor in high contrast, letter-spacing.
- The Win+D fix adds a permanent 250 ms timer. It is cheap, but it is the one piece of the app that runs continuously.

## Next starting point
Get the user's confirmation of Win+D with the real key. Collect results for menus, tray clicks and sign-in start. Then finish the release checks and package a ZIP when authorized.