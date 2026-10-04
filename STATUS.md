# Goal widget — handoff

## Status
Phase 4 release pending. Working compact C#/WinUI app: `artifacts/compact-build/GoalWidget.exe`. User accepts the current visual treatment for now. Source repository destination: https://github.com/HarshK99/goal-widget (public, explicitly requested).

## Implemented
- Card is 210 × 210 logical units, 30% smaller in each dimension, with proportional text/padding. Editor/menu remain full-sized.
- Selected Windows App SDK components replace the broad dependency; runtime bundling retained. App folder measured 174.9 MiB versus 231.4 MiB previously, a 24.4% reduction. User manually deleted the old output; its absence was confirmed.
- Hidden from taskbar/Alt+Tab by default; normal minimization disabled; other apps can cover the widget. Right-click editing/Quit and shortcut reopening remain available.
- Startup shortcut configured for the current Windows user, pointing to the compact output; cloning the repository does not install it. The Desktop shortcut was later removed by the user.
- Right-click **Show in Task View** temporarily exposes the widget for Windows pinning. User clarified that “all screens” means every virtual desktop, not physical monitors.

## Evidence and limits
- Latest authorized rebuild passed in 16.14 seconds with zero warnings/errors; `artifacts/desktop-pin-setup-build.log`. Reopened the resulting executable.
- Earlier compact screenshot showed centered saved text and mountains, 315 × 315 physical pixels at 150% scaling. App window observation confirmed no forced app-window flag, no minimize button and no always-on-top flag. Visual taskbar/Alt+Tab inspection remains pending.
- Virtual desktop setup: enable Show in Task View, press Win+Tab, right-click Goal → Show windows from this app on all desktops, then disable Show in Task View. Pinning and its persistence after hiding Task View or signing in again are NOT verified. User's “cool” is not recorded as proof of those checks.
- Actual sign-in startup, Win+D, editing/save/cancel, position retention and other release cases remain pending. No automated tests, code review, publish, ZIP or clean-machine checks ran. Personal goal data was not changed.

## Current documentation / GitHub request
User requested docs update and GitHub push, then explicitly requested a new public repository. Prepared README, corrected outdated startup/taskbar/virtual-desktop notes, updated artwork details and excluded build output, captures, personal state and provisional reference images from Git. Local generated app artwork and its notice are included. No build/test/review was requested or run for this documentation/push task.

Files updated: README.md, .gitignore, STATUS.md, docs/install.md, docs/product-brief.md, docs/environment.md, docs/release-checks.md, docs/artwork.md. Initial source commit also includes the existing app, phase plans and text-only preview reference. Local build evidence remains under ignored artifacts/.

## Tray icon (2026-10-04) — built and running, clicks NOT yet observed
User removed the Desktop shortcut, then asked for a notification-area icon and for the widget to always start at sign-in. This reverses the brief's earlier "no tray menu" exclusion; brief updated.

- Added `src/GoalWidget/TrayIcon.cs` and `Assets/goal.ico`; changed `WidgetWindow.xaml.cs` and `GoalWidget.csproj`. Left-click restores the widget; right-click shows a plain Windows menu with Edit goal, Show in Task View and Quit. The icon is re-added when Explorer restarts and removed on Quit. No new package.
- Checked: the icon file loads through the same Windows call the app uses, at 16/20/24/32 pixels. Startup shortcut is present in the Startup folder, its target exists and it is not disabled; no Desktop shortcut exists.
- Authorization: user replied `yes go ahead` to quitting the widget, rebuilding into `artifacts/compact-build` and reopening it. Scope is this tray-icon task only; no tests, code review, publish or ZIP.
- Build: passed in 40.72 seconds, zero warnings/errors; `artifacts/tray-icon-build.log`. `Assets/goal.ico` is in the output. Widget reopened and responding. Windows recorded a tray icon for the executable with tooltip "Goal" (`HKCU:\Control Panel\NotifyIconSettings`), not promoted, so it sits in the `^` overflow popup.
- Omitted: left-click, the right-click menu, icon removal on Quit, re-adding after an Explorer restart and the icon's look at tray size were not observed. Sign-in startup has still not been observed directly. No automated tests or code review.
- Docs updated: README.md, docs/product-brief.md, docs/install.md, docs/release-checks.md, docs/artwork.md. Not committed.

## Next starting point
User to try the tray icon by hand (Tray icon row in docs/release-checks.md) and confirm it appears after the next sign-in. Then observe virtual-desktop pinning after restoring hidden Task View status. Verify sign-in startup and remaining cases in docs/release-checks.md, obtaining explicit authorization for new build/test/review activity. Then package a release ZIP when authorized. Do not mark Phase 4 complete before the required artifact and evidence exist.
