# Goal Widget

A small Windows 11 desktop widget for one personal goal. It lies on the desktop beneath your apps, never takes focus, shows on every virtual desktop and starts at sign-in. Built with C# and WPF.

## Current status

Rebuilt on 2026-10-05 as a desktop-level window; the earlier WinUI version behaved like an ordinary app window. The app folder is about 3 MiB and needs no runtime install, because it uses the .NET Framework that ships with Windows 11. There is no packaged release yet; remaining checks are in [release checks](docs/release-checks.md).

## Use

- **Edit:** double-click the card, or right-click → **Edit goal**.
- **Move:** drag the card. The position is remembered.
- **Tray icon** (beside the clock): left-click shows the widget above your windows until you move to another app; right-click gives **Edit goal**, **Show above other windows** and **Quit**. Windows 11 keeps new tray icons in the `^` overflow popup until you drag them out.
- No taskbar button and no Alt+Tab entry, by design.

## Install from source

Build (see [build setup](docs/environment.md)), then run `tools/install.ps1`. It copies the build to `%LOCALAPPDATA%\Programs\GoalWidget`, adds a Startup shortcut and starts the widget. Your goal is stored separately in `%LOCALAPPDATA%\GoalWidget`. Details: [installation](docs/install.md).

## Development and handoff

- [Current status and next steps](STATUS.md)
- [Product brief](docs/product-brief.md)
- [Local build setup and commands](docs/environment.md)
- [Artwork source and notice](docs/artwork.md)
- [Project instructions](AGENTS.md)

Build and test commands require explicit authorization under the project instructions. No automated tests or code review are claimed.

`preview/` contains the earlier HTML design reference, not the app. Its provisional photograph and reference screenshots stay local and are excluded from Git; the app includes its own generated artwork and notice.
