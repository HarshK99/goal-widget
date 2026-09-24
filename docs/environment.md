# Phase 2 environment and native implementation

> Virtual desktop update: user means every Windows virtual desktop. Rebuilt and reopened with right-click **Show in Task View** (temporary visibility switch), zero warnings/errors; `artifacts/desktop-pin-setup-build.log`. To configure Windows pinning: enable that option, press Win+Tab, right-click Goal, choose **Show windows from this app on all desktops**, return to the widget and disable Show in Task View. Pinning has not yet been applied or verified, including after hiding Task View or signing in again.

> Latest update: the user manually deleted the old build; its absence was confirmed. Earlier deletion-block notes below are historical. Current-user Startup and Desktop shortcuts now point to `artifacts/compact-build/GoalWidget.exe`. Taskbar hiding and disabled minimization were rebuilt and reopened after explicit authorization: 18.75 seconds, zero warnings/errors (`artifacts/desktop-build.log`). Sign-in startup, Win+D and visual taskbar/Alt+Tab inspection remain unverified. No automated tests or review ran. The user wants one widget across Windows virtual desktops; pinning remains unverified.

Originally recorded in Phase 2 on 2026-09-24. The Phase 4 update below supersedes earlier build/tool availability statements; historical notes follow it.

## Compact build — current result

User authorized rebuilding and reopening with `yes`. Card is now 210 × 210 logical units, 30% smaller in each dimension, with proportional text/padding and full-sized editor controls. The compact Release build succeeded in 52.09 seconds with zero warnings/errors; see `artifacts/compact-build.log`.

The broad WindowsAppSDK 2.5.1 reference was replaced with its WinUI 2.3.9, Foundation 2.3.12, InteractiveExperiences 2.1.9 and DWrite 2.1.0 components. Runtime bundling remains enabled. Fresh output `artifacts/compact-build` measured 183,383,604 bytes (174.9 MiB), versus 242,644,978 bytes (231.4 MiB) previously: 24.4% smaller. The command below was used with `-o artifacts/compact-build -v:minimal`; restoration was allowed for changed dependencies.

Launched `artifacts/compact-build/GoalWidget.exe` and measured a 315 × 315 physical-pixel client at 150% scaling. Screenshot `artifacts/widget-compact.png` shows centered saved text and mountain artwork. Editing/save/cancel and broader release checks remain pending. No automated tests, code review, publish or ZIP ran.

The old build was closed and the compact app left running. User explicitly requested deleting the old `src/GoalWidget/bin/x64/Release/net10.0-windows10.0.26100.0/win-x64` folder, but automatic approval review blocked deletion, including the retry after that request. The old 231.4 MiB folder remains; no disk space has been reclaimed by cleanup. SDK/tool caches and personal goal data were not removed.

The remaining sections record earlier checkpoints; current size, output path and dependency choices above supersede those historical values.

## Phase 4 build-and-launch checkpoint — 2026-09-24

User authorized a minimal setup and native build/launch attempt capped at about an hour (`cool go ahead`). Started 20:49 IST; reached a working, square widget by approximately 21:02, and recorded the checkpoint at 21:05. No full Visual Studio installation, Developer Mode change, automated tests, code review, publish or ZIP creation.

- Downloaded Microsoft's .NET 10.0.401 Windows x64 ZIP and verified SHA-512 against the official release metadata before extracting to `D:/FounderMode/Tools/goal-widget/dotnet`.
- Observed SDK 10.0.401, MSBuild 18.9.11, runtime 10.0.12. Existing project/package pins unchanged. The Windows SDK build tools arrived through the project's NuGet dependency; no full Windows SDK or Visual Studio installer was needed.
- First build failed NU1100 because no usable package source was configured. A command-scoped explicit nuget.org source resolved this. A quiet restore was interrupted while investigating an apparent stall; the next attempt used normal verbosity and disabled HTTP/2 for that process. That attempt succeeded, but the stall's exact cause was not established.
- Build attempt 3 succeeded, but first launch failed subscribing to `AccessibilitySettings.HighContrastChanged` (COM error `0x80070490`). Changed to the desktop `WM_SETTINGCHANGE` / `WM_THEMECHANGED` broadcasts. Added opt-in startup diagnostics using `GOAL_WIDGET_STARTUP_LOG`.
- Observed `ResizeClient` making the titleless card too tall. Corrected outer size using the actual Win32 client rectangle. Latest client size measured **450 × 450 physical pixels at DPI 144 (150%)**, equivalent to 300 × 300 logical units. Screenshot viewed at `artifacts/widget-square.png`.
- Final build attempt 6: **success, zero warnings/errors**, 20.33 seconds with dependencies cached. Earlier successful first build took 2 minutes 26.78 seconds excluding prior download/restore work. Final app stayed open with visible centered default text and mountain art. This is not full release verification.

### Reproduce the local build (only when currently authorized)

From the project root, in PowerShell:

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
& "$env:DOTNET_ROOT/dotnet.exe" build src/GoalWidget/GoalWidget.csproj -c Release -p:Platform=x64 --source https://api.nuget.org/v3/index.json
```

Executed first successful build with `-v:normal`; later successful builds used `--no-restore -v:minimal` instead of `--source`. Logs are in `artifacts/build-attempt-*.log`. Environment changes are process-local, not machine-wide. No permanent PATH change.

Executable: `src/GoalWidget/bin/x64/Release/net10.0-windows10.0.26100.0/win-x64/GoalWidget.exe`. This is a local build folder, not a published/tested distribution. Keep its dependencies alongside it.

### Measured space

At the checkpoint: local tools folder **2.57 GiB** including SDK (0.75), dependency cache (1.43), downloaded SDK ZIP (0.28), redirected HTTP cache (0.10) and temporary files. Local app output **231.4 MiB**, including dependencies; not the size of a release ZIP. The initial restore also populated approximately **392.1 MiB** in the default C: NuGet HTTP cache before it was redirected to D:. Existing shared cache files were not deleted. Free space observed afterward: C: 25.2 GiB, D: 165.1 GiB; those changes may include other machine activity.

Sources: [Microsoft .NET 10 release metadata](https://builds.dotnet.microsoft.com/dotnet/release-metadata/10.0/releases.json), [WinUI command-line setup](https://learn.microsoft.com/en-us/windows/apps/get-started/start-here?tabs=command-line), [desktop accessibility broadcasts](https://learn.microsoft.com/en-us/windows/win32/winauto/accessibility-parameters), [theme broadcasts](https://learn.microsoft.com/en-us/windows/win32/winmsg/wm-themechanged).

## This machine: observed, read-only

- Windows 11 Home Single Language, 64-bit, 25H2, build 26200.9457. The older registry product-name field says Windows 10; the operating-system query identifies Windows 11.
- `dotnet` and `msbuild` were not found on PATH. `dotnet --list-sdks` could not run because the command was missing. No standard `C:\Program Files\dotnet` installation was found.
- Visual Studio Installer's `vswhere` reports Build Tools 2017 15.9.33, and incomplete/non-launchable Build Tools 2019 16.11.9. Neither is the selected modern WinUI toolchain.
- Installed Windows SDK include directory found: `10.0.17763.0`; the selected Windows 11 SDK was not found there.
- No tools, workloads, runtimes or packages were installed or restored. No executable was produced.

## Selected stable configuration

| Component | Selection |
| --- | --- |
| .NET SDK | 10.0.401, `global.json`, later patch in that feature band allowed; previews disabled |
| Target | .NET 10, Windows API surface 10.0.26100.0; minimum Windows 11 22000 |
| Microsoft.WindowsAppSDK | 2.5.1, pinned |
| Microsoft.Windows.SDK.BuildTools | 10.0.26100.9169, pinned |
| Architecture | x64, matching this machine |
| Deployment configuration | Unpackaged, .NET and Windows App SDK self-contained; folder distribution later |

Selections are based on published stable versions, not successful local compilation. Windows App SDK 1.8's published servicing end date has passed; it was not selected. No preview dependencies, third-party UI framework, installer, signing certificate, or packaging project was added.

Sources: [Microsoft release channels and lifecycle](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/release-channels), [Windows App SDK 2.5.1 package](https://www.nuget.org/packages/Microsoft.WindowsAppSDK/2.5.1), [.NET 10 downloads](https://dotnet.microsoft.com/en-us/download/dotnet/10.0), [Windows SDK BuildTools package](https://www.nuget.org/packages/Microsoft.Windows.SDK.BuildTools/10.0.26100.9169), [unpackaged deployment](https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/unpackage-winui-app).

## Prerequisites for a later authorized build

Phase 4 decision: keep C#/WinUI and VS Code, first attempting the smaller .NET command-line setup. Microsoft's [current quickstart](https://learn.microsoft.com/en-us/windows/apps/get-started/start-here?tabs=command-line) documents a .NET 10 route without the full Visual Studio editor. This supersedes the earlier blanket recommendation to install Visual Studio and C++ tools. Our existing unpackaged project differs from that guide's generated template, so compatibility is still unproven; do not add package-identity tooling unless the app actually needs it.

After explicit build/setup authorization, obtain the pinned stable .NET SDK (confirm its availability), prefer a task-local installation and package cache on D:, then attempt the existing project's build. Add only dependencies required by concrete build failures. Do not silently escalate to a full Visual Studio installation, enable Developer Mode, or switch to an HTML wrapper. Record actual downloaded/installed size and tool versions; the current several-GB allowance is an estimate, not a measured requirement. No setup has been performed.

After the user explicitly authorizes a build for the current task, from the project root:

```powershell
dotnet build .\src\GoalWidget\GoalWidget.csproj -c Debug -p:Platform=x64
```

**Documented only, not executed.** This command restores packages and builds the app. It must not be used as a prerequisite to an otherwise unauthorized walkthrough. There is no test project or test command. Publishing and packaging belong to Phase 4 with its own authorization.

## Native choices and limits

- Ordinary `OverlappedPresenter` window, not always on top, with taskbar recovery. A system border is retained without a title bar. The client area is resized to 300 logical units using each monitor's DPI (display scaling).
- Windows' supported corner preference requests rounded outer corners. It does not offer a custom 30-unit radius. System border/shadow and actual rounding need observation; this is an implementation compromise, not newly recorded user acceptance. No shaped window region or Explorer attachment is used. [Microsoft corner guidance](https://learn.microsoft.com/en-us/windows/apps/desktop/modernize/ui/apply-rounded-corners)
- Desktop acrylic is requested when supported. A local photo, tinted base and frost overlay remain independently of acrylic. The OS manages inactive/transparency-disabled acrylic fallback; unsupported acrylic uses solid frost. Actual active/inactive appearance and contrast have not been seen. [System backdrop guidance](https://learn.microsoft.com/en-us/windows/apps/develop/ui/system-backdrops)
- Win+D may minimize the window. Taskbar or repeated launch restores it. There is no promise of wallpaper-level attachment. Single instance uses Windows App SDK activation redirection before creating the writer/window. [App lifecycle guidance](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/applifecycle/applifecycle-single-instance)
- The native editor uses an owned, resizable window hosting `ContentDialog`, because a 300-unit card is too small for accessible editing. Owner is disabled while editing. Enter inserts a newline; Escape/Cancel discard draft; window close is blocked during a save.
- Goal acceptance uses native text measurement with matching typography/layout bounds, trying sizes 39 through 23. Saved text that no longer fits after a system text-size increase remains scrollable with a shortening notice. New oversized drafts are rejected. High contrast uses system colors and hides artwork. Keyboard focus and all these behaviors remain unverified.
- Left-drag starts after a small movement threshold. Alt+arrow keys move in ten-unit steps as a keyboard alternative. Context menu opens by right-click, Shift+F10 or Menu key; Enter/Space opens editing.
- State: `%LOCALAPPDATA%\GoalWidget\state.json`, backup `state.json.bak`. Saves are serialized and replace the file after flushing a same-directory temporary file. Damaged files are copied to uniquely named `.recovered-*` files before replacement. Unreadable or future-version primary/backup files block writes. Save errors retain the draft or show a nonmodal position message. Abrupt shutdown may leave an unused `.tmp` file; it is not read as state.
- Physical screen position, monitor device name and DPI are stored. Restore clamps the card to a current usable display; absent monitor falls back to primary. Display rearrangement is clamped rather than promising identical relative spacing. Native display, scaling and removal behavior require execution.

## Remaining evidence

No builds, tests, code review, application launch, native screenshots, scaling walkthroughs, data-failure exercises or packaging were run or authorized in Phase 2. The skill search was implementation guidance, not a review. Native feasibility here means documented supported APIs and source choices, not a demonstrated runnable result.

The accepted centered HTML was read. The provisional photo was copied unchanged from `preview/landscape.jpg`; its provenance remains unresolved and distribution is deferred. Phase 3 owns visual refinement and artwork clearance/replacement. Build/runtime limitations must remain visible in its handoff.

