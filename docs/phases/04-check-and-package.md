# Phase 4 — Check and package

## Entry and required inputs
`start phase 4`: read `AGENTS.md`, the brief, `STATUS.md`, this file, `docs/environment.md`, `docs/artwork.md`, `docs/release-checks.md` and the actual build output.

Starting this phase does NOT authorize build, test, review or publish commands. Read current instructions and record exactly what is permitted; earlier authorization does not carry forward.

## Execute only within current authorization
- [ ] Build the release configuration with the command in `environment.md`; record result and output. Install it with `tools/install.ps1`.
- [ ] Walk through every row of `release-checks.md` by hand and record the actual result or the omission. Most rows need the user: virtual desktop switching, Win+D, drag, menus, editing, tray icon, sign-in.
- [ ] If Win+D does not keep the widget visible, read `%LOCALAPPDATA%\GoalWidget\log.txt` for "Show Desktop" lines before changing `DesktopPin.cs`. Win+D is wanted but is not a release criterion.
- [ ] Automated tests only if authorized: state corruption and interrupted saves, validation, placement. Code review only if explicitly requested.
- [ ] Check the card at actual size, especially the centered text over the mountains, and at 100/125/150/200% scaling.
- [ ] Confirm artwork provenance before redistribution.
- [ ] With packaging authorization, produce a ZIP of the app folder with recorded path and hashes, and launch from an extracted copy. No store upload or certificate installation.
- [ ] Finalize `install.md` from observed behavior. Removing the app must not delete the goal data.
- [ ] Update `STATUS.md` once with checks, omissions, artifact location, limits and completion or pending state.

## Deliverables and stop
A distributable ZIP, the install guide, the artwork record and the release-check record. If essential builds or checks are missing, report **release pending** with the exact remaining work. Stop after this phase; timer and Apple work need separate scope.
