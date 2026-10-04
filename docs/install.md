# Goal widget — installation draft

> Virtual desktop update: user means every Windows virtual desktop. Rebuilt and reopened with right-click **Show in Task View** (temporary visibility switch), zero warnings/errors; `artifacts/desktop-pin-setup-build.log`. To configure Windows pinning: enable that option, press Win+Tab, right-click Goal, choose **Show windows from this app on all desktops**, return to the widget and disable Show in Task View. Pinning has not yet been applied or verified, including after hiding Task View or signing in again.

**No release is available yet. These are proposed instructions, not a demonstrated installation.** See `release-checks.md` for pending work.

The current local app is `artifacts/compact-build/GoalWidget.exe`, successfully built and launched on this machine. Keep its entire 174.9 MiB folder together. The card is 210 × 210 logical units, 30% smaller in each dimension. No published ZIP or clean-machine installation has been verified; editing/save/cancel checks remain pending.

The user deleted the superseded 231.4 MiB build folder; its absence was confirmed. Keep `artifacts/compact-build`, which is the current app folder.

## Intended distribution

A local ZIP containing the complete app folder, including `GoalWidget.exe`, dependencies, generated artwork and its notice. Extract the whole folder; moving only the executable is not supported by the selected configuration. No store upload or installer is planned. Automatic startup and a tray icon were subsequently requested; startup is configured locally, and the tray icon is in the current local build.

Project settings target Windows 11 build 22000 or newer and x64 processors; no ARM64 build is configured. The project targets .NET 10 and uses selected components from Windows App SDK 2.5.1; exact pins are recorded in `environment.md`. It requests both runtimes be included with the app (`SelfContained` and `WindowsAppSDKSelfContained`), rather than requiring a separate runtime installation. That intention is not proof that a published folder will launch on a clean machine; confirm remaining native dependencies from actual release output and launch evidence.

Build-machine prerequisites are separately recorded in `environment.md`. The .NET SDK and Windows development tools are not intended requirements for someone using the eventual release.

## Current local startup

A Goal Widget.lnk shortcut exists in the current user’s Windows Startup folder, pointing to the compact executable above. Checked 2026-10-04: the shortcut is present, its target exists and Windows has not disabled it. The Desktop shortcut has been removed by the user. Startup at sign-in is configured; a sign-in has not yet been observed directly. To disable startup, remove only that shortcut from the Startup folder (open shell:startup using Win+R). Moving the app folder requires updating the shortcut.

The widget adds an icon to the notification area (the small icons beside the clock). Left-click brings the widget to the front; right-click offers **Edit goal**, **Show in Task View** and **Quit**. Windows 11 places new icons in the `^` overflow popup; drag it onto the taskbar once to keep it visible. The icon exists only while the widget runs. Changes to hide the taskbar entry and disable minimization were rebuilt and reopened after authorization; visual taskbar/Alt+Tab inspection remains pending. Use the virtual desktop pinning steps above; their behavior after hiding Task View and signing in again remains unverified.

## Proposed install and use

1. Extract the release ZIP to a permanent folder you control. Keep all extracted files together.
2. Open `GoalWidget.exe`. Optionally create a desktop shortcut pointing to that file.
3. Right-click the card and choose **Edit goal**. With keyboard focus on the card, use **Shift+F10** for the menu or **Enter** to edit.
4. Enter one short goal, using Enter for deliberate line breaks. Choose **Save**, or **Cancel** / Escape to discard the draft. Limit: 100 text elements and five explicit lines; text must also fit the card.
5. Drag the card to move it, or focus it and use **Alt+arrow keys**. The intended behavior is to remember the goal and position.
6. Choose **Quit** from the card's menu or the tray icon's menu to close it. Reopen using the executable or a shortcut to it.

Other apps can cover the widget. Win+D may minimize it; click the tray icon to recover it. It is not attached to the wallpaper. Windows supplies its corners/shadow, which may differ from the HTML preview.

## Goal data and recovery

The configured data folder is `%LOCALAPPDATA%\GoalWidget`, with `state.json` and a last-valid backup, `state.json.bak`. Recovery may preserve damaged files with `.recovered-*` names. Keep these files when troubleshooting. The intended behavior is to retain an unsaved draft on save failure and explain the failure; this has not yet been exercised.

Before manually changing data, quit the app and copy the entire data folder somewhere safe. Do not edit or delete personal data merely to try the release checks. If a future-version file blocks saving, preserve it and use the compatible app version rather than forcing an overwrite.

## Proposed removal

Quit the app, then delete only its extracted app folder and any shortcut you created. This should leave the separate local goal data intact. If you also want to erase the saved goal, back it up if needed and separately delete `%LOCALAPPDATA%\GoalWidget`. No removal command has been executed.

## Release details still required

Before this draft becomes final, record the actual ZIP name/location, build/publish commands and versions, included runtime files, remaining prerequisites, extracted-folder launch results, and any observed installation differences. App use is intended to require no network; build-time package downloads are separate.

