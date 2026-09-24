# Goal widget — handoff

## Status
Phase 4 release pending. Working compact C#/WinUI app: `artifacts/compact-build/GoalWidget.exe`. User accepts the current visual treatment for now. Source repository destination: https://github.com/HarshK99/goal-widget (public, explicitly requested).

## Implemented
- Card is 210 × 210 logical units, 30% smaller in each dimension, with proportional text/padding. Editor/menu remain full-sized.
- Selected Windows App SDK components replace the broad dependency; runtime bundling retained. App folder measured 174.9 MiB versus 231.4 MiB previously, a 24.4% reduction. User manually deleted the old output; its absence was confirmed.
- Hidden from taskbar/Alt+Tab by default; normal minimization disabled; other apps can cover the widget. Right-click editing/Quit and shortcut reopening remain available.
- Startup and Desktop shortcuts configured for the current Windows user. They point to the compact output; cloning the repository does not install these shortcuts.
- Right-click **Show in Task View** temporarily exposes the widget for Windows pinning. User clarified that “all screens” means every virtual desktop, not physical monitors.

## Evidence and limits
- Latest authorized rebuild passed in 16.14 seconds with zero warnings/errors; `artifacts/desktop-pin-setup-build.log`. Reopened the resulting executable.
- Earlier compact screenshot showed centered saved text and mountains, 315 × 315 physical pixels at 150% scaling. App window observation confirmed no forced app-window flag, no minimize button and no always-on-top flag. Visual taskbar/Alt+Tab inspection remains pending.
- Virtual desktop setup: enable Show in Task View, press Win+Tab, right-click Goal → Show windows from this app on all desktops, then disable Show in Task View. Pinning and its persistence after hiding Task View or signing in again are NOT verified. User's “cool” is not recorded as proof of those checks.
- Actual sign-in startup, Win+D, editing/save/cancel, position retention and other release cases remain pending. No automated tests, code review, publish, ZIP or clean-machine checks ran. Personal goal data was not changed.

## Current documentation / GitHub request
User requested docs update and GitHub push, then explicitly requested a new public repository. Prepared README, corrected outdated startup/taskbar/virtual-desktop notes, updated artwork details and excluded build output, captures, personal state and provisional reference images from Git. Local generated app artwork and its notice are included. No build/test/review was requested or run for this documentation/push task.

Files updated: README.md, .gitignore, STATUS.md, docs/install.md, docs/product-brief.md, docs/environment.md, docs/release-checks.md, docs/artwork.md. Initial source commit also includes the existing app, phase plans and text-only preview reference. Local build evidence remains under ignored artifacts/.

## Next starting point
Observe virtual-desktop pinning after restoring hidden Task View status. Verify sign-in startup and remaining cases in docs/release-checks.md, obtaining explicit authorization for new build/test/review activity. Then package a release ZIP when authorized. Do not mark Phase 4 complete before the required artifact and evidence exist.
