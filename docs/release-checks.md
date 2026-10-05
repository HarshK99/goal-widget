# Phase 4 release checks

Recorded 2026-10-05 against the WPF rebuild. **Release pending.** Only results written below were observed; expected outcomes are requirements, not observations. Further build or test work needs explicit authorization. Use a disposable state folder or a separate Windows account for data-failure exercises, so the real goal is never at risk.

| Case | Expected outcome | Result |
| --- | --- | --- |
| Build | Record command, exit status and output | Passed: 33.72 seconds, zero warnings/errors, `artifacts/widget-build`, 3.01 MiB. `artifacts/wpf-build.log` |
| Window model | Tool window, never-activate, no owner, not topmost, not foreground | Read from the running widget: all as expected |
| Stays on the desktop | Beneath every app; directly above the desktop | Stacking order read: widget 18th, desktop 19th of 20 visible windows. Not recorded as watched by a person |
| Virtual desktops | Visible on every desktop with no pinning; never takes focus or comes forward on a switch | Windows returns no desktop ID for the widget (0x8002802B), unlike an app window. No hand result recorded |
| Taskbar and Alt+Tab | No entry in either | Flags say so; no hand result recorded |
| Win+D | Widget stays visible; returns beneath apps afterwards | Not performed. Detection unproven on this Windows build; check `log.txt` |
| Rendering | Rounded card, artwork, centered goal, shadow | Window capture at 150% shows card, artwork and centered saved goal. Shadow on the real desktop not recorded |
| Existing data | Saved goal and position read by the new app | Goal text and position matched `state.json` |
| Drag | Card moves; position saved at the end; stays inside the display | Not performed |
| Menus | Right-click on card and tray: Edit goal, Show above other windows, Quit | Not performed |
| Editing | Double-click or menu opens the editor; Save persists; Cancel and Escape change nothing; Enter adds a line; invalid goals explained | Not performed |
| Tray icon | Appears at launch, at sign-in and after Explorer restarts; left-click lifts the widget; gone after Quit | Windows registered an icon for the installed path. Clicks, sign-in and Explorer restart not performed |
| Quit and restart | Graceful quit; goal and position kept | Log shows Started, Quit, Started; position kept |
| Install | `tools/install.ps1` copies the build, sets the Startup shortcut and starts the widget | Performed; shortcut target exists; widget running from the install folder |
| Start at sign-in | Widget and tray icon present after signing in | Not performed |
| Display scaling | 100%, 125%, 150%, 200% | Only 150% seen |
| Monitor changes | Card stays reachable when a display is removed or rearranged | Not performed |
| High contrast | Card uses system colors; artwork hidden | Not performed. The editor is not restyled |
| Contrast | Goal text at least 3:1, preferably 4.5:1 | Not performed |
| Save failure and state recovery | Draft kept on failure; damaged files preserved; backup used | Not performed |
| Artwork in output | Generated artwork and notice included; provisional preview photograph excluded | Notice and icon are in the output folder; the landscape is inside the exe. Preview photograph is not referenced by the project |
| ZIP and clean-machine launch | Recorded path and hashes; launch from an extracted copy | Not performed |

After the hand-check list was given, the user replied "perfect" and asked for cleanup and a push. That reply did not say which cases were tried, so no row above is marked as user-confirmed.

No automated tests exist and no formal code review has been run. If tests are authorized, start with state recovery, interrupted saves, goal validation and placement.
