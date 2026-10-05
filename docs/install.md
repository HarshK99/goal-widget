# Goal widget — installation

**No packaged release exists yet.** These steps describe the local install on the development machine, performed on 2026-10-05. See `release-checks.md` for what is still unchecked.

## Requirements

Windows 11. The app uses .NET Framework 4.8, which is part of Windows 11, so nothing else needs installing. The app folder is about 3 MiB.

## Install from a build

1. Build the app (see `environment.md`). Output goes to `artifacts/widget-build`.
2. Run `tools/install.ps1` in PowerShell. It:
   - closes a running widget,
   - copies the build to `%LOCALAPPDATA%\Programs\GoalWidget`,
   - creates **Goal Widget.lnk** in your Startup folder pointing there,
   - starts the widget.

The installed copy is separate from the build output on purpose: a failed or half-finished build cannot break the widget you use.

## Use

- The widget lies on the desktop beneath your apps on every virtual desktop. It has no taskbar button and is not in Alt+Tab.
- Double-click the card, or right-click → **Edit goal**. Enter makes a new line; Escape cancels. Limit: 100 characters and five lines, and the text must fit the card.
- Drag the card to move it. The position is saved when you let go.
- Tray icon beside the clock: left-click shows the widget above other windows until you switch to another app; right-click gives the same menu as the card. New tray icons start in the `^` overflow popup; drag it onto the taskbar once to keep it visible. Win+B reaches it from the keyboard.
- **Quit** is in both menus. To start it again, open `GoalWidget.exe` in the install folder, or sign in again.

## Startup

The Startup shortcut starts the widget at sign-in. To stop that, delete **Goal Widget.lnk** from the Startup folder (Win+R, then `shell:startup`). Checked 2026-10-05: the shortcut points to the installed exe and the target exists. An actual sign-in has not been observed.

## Goal data, log and recovery

`%LOCALAPPDATA%\GoalWidget` holds `state.json`, its last-valid backup `state.json.bak`, and `log.txt`. Recovery may keep damaged files with `.recovered-*` names. The log records starts, quits, crashes and Show Desktop changes; read it first if the widget fails to appear.

Before changing data by hand, quit the widget and copy the whole folder somewhere safe.

## Remove

Quit the widget, delete `%LOCALAPPDATA%\Programs\GoalWidget` and the Startup shortcut. This leaves your goal. To erase the goal too, delete `%LOCALAPPDATA%\GoalWidget`.

## Still required before a release

A ZIP with recorded name, location and hashes; a launch from an extracted copy on a clean machine; the unchecked rows in `release-checks.md`.
