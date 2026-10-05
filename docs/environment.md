# Build environment

The app is C# and WPF on .NET Framework 4.8. Recorded 2026-10-05.

| Item | Value |
| --- | --- |
| Project | `src/GoalWidget/GoalWidget.csproj`, target `net48`, `UseWPF` |
| SDK used to build | .NET SDK 10.0.401, pinned by `global.json` |
| Runtime needed to run | .NET Framework 4.8 or later, part of Windows 11. This machine reports 4.8.1 (release 533509) |
| Packages | `System.Text.Json` 8.0.5 and its dependencies; .NET Framework reference assemblies, fetched by the SDK |
| Output | `artifacts/widget-build`, 3.01 MiB in 14 files |

## Build tools on this machine

There is no machine-wide .NET SDK or Visual Studio. The SDK is a private copy under `D:/FounderMode/Tools/goal-widget/dotnet`, extracted from Microsoft's official ZIP after checking its SHA-512. Package caches and temporary files are kept beside it on D:. The settings below apply only to the PowerShell session that runs them; nothing is added to PATH.

## Build

From the project root in PowerShell, only when currently authorized:

```powershell
$env:DOTNET_ROOT = 'D:/FounderMode/Tools/goal-widget/dotnet'
$env:DOTNET_CLI_HOME = 'D:/FounderMode/Tools/goal-widget/cli'
$env:NUGET_PACKAGES = 'D:/FounderMode/Tools/goal-widget/nuget'
$env:NUGET_HTTP_CACHE_PATH = 'D:/FounderMode/Tools/goal-widget/http-cache'
$env:TEMP = 'D:/FounderMode/Tools/goal-widget/temp'
$env:TMP = $env:TEMP
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_NOLOGO = '1'
$env:DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP2SUPPORT = 'false'
& "$env:DOTNET_ROOT/dotnet.exe" build src/GoalWidget/GoalWidget.csproj -c Release --source https://api.nuget.org/v3/index.json -v:minimal -o artifacts/widget-build
```

- First build on 2026-10-05: restore 16.43 seconds, total 33.72 seconds, zero warnings and errors. Log: `artifacts/wpf-build.log`.
- Later builds can replace `--source …` with `--no-restore`.
- HTTP/2 is turned off because an earlier package restore stalled with it on; the cause was never established.
- There is no test project.

## Install after building

Run `tools/install.ps1`. It copies the build to `%LOCALAPPDATA%\Programs\GoalWidget`, points the Startup shortcut there and starts the widget. The widget in use never runs from the build folder.

## Notes for the next change

- The window model lives in `DesktopPin.cs`: unowned tool window, never-activate, held at the bottom of the stack. Only that class changes the stacking order. Windows sends no event when Show Desktop (Win+D) starts, so the class polls the desktop's position every 250 ms and raises the widget while the desktop is on top.
- The widget is never the active window. So the card is dragged by hand in `WidgetWindow.xaml.cs`, and menus are owned by the tray icon's hidden window in `TrayIcon.cs`.
- `IsExternalInit.cs` exists because .NET Framework lacks the marker type that records need.
- Implicit usings are off; each file lists its own.
- WPF has no letter-spacing on `TextBlock`.
- Goal data: `%LOCALAPPDATA%\GoalWidget\state.json`, backup `state.json.bak`, log `log.txt`. Saves write a temporary file, flush it, then replace the original; damaged files are copied to `.recovered-*` names first.
