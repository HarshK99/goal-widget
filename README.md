# Goal Widget

A compact Windows desktop widget for one personal goal. Built with C# and WinUI, with a mountain background, centered text, local saving and a draggable 210 × 210 card.

## Current status

The Windows x64 app has been built and launched locally. Its complete app folder measured 174.9 MiB. There is no packaged release yet; remaining checks are recorded in [release checks](docs/release-checks.md).

## Use on the development machine

Open `artifacts/compact-build/GoalWidget.exe`, keeping the entire folder together. Build output and machine-local shortcuts are not included in this repository.

- Right-click → **Edit goal** to change the text, or **Quit** to close the widget.
- Drag the card to move it. Other apps can cover it.
- The widget starts hidden from the taskbar and Alt+Tab.
- A tray icon beside the clock brings the widget to the front on left-click and offers the same menu on right-click. Windows 11 puts it in the `^` overflow popup until you drag it out.
- A Startup shortcut is configured on the original development machine. Cloning this repository does not install that shortcut.
- For virtual desktops: right-click → **Show in Task View**, press **Win+Tab**, right-click **Goal** → **Show windows from this app on all desktops**, then turn **Show in Task View** off again. Pinning after hiding Task View or signing in again remains unverified.

See [installation and startup](docs/install.md) for details. The widget is not attached to the wallpaper; Win+D behavior remains unverified.

## Development and handoff

- [Current status and next steps](STATUS.md)
- [Product brief](docs/product-brief.md)
- [Local build setup and commands](docs/environment.md)
- [Artwork source and notice](docs/artwork.md)
- [Project instructions](AGENTS.md)

The project uses .NET 10 and selected Windows App SDK components. Build and test commands require explicit authorization under the project instructions. No automated tests or code review are claimed.

`preview/` contains the earlier HTML design reference, not the app. Its provisional photograph and reference screenshots stay local and are excluded from Git; the native app includes its generated artwork and notice.
