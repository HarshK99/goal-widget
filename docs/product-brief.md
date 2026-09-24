# Goal widget — product brief

> Virtual desktop update: user means every Windows virtual desktop. Rebuilt and reopened with right-click **Show in Task View** (temporary visibility switch), zero warnings/errors; `artifacts/desktop-pin-setup-build.log`. To configure Windows pinning: enable that option, press Win+Tab, right-click Goal, choose **Show windows from this app on all desktops**, return to the widget and disable Show in Task View. Pinning has not yet been applied or verified, including after hiding Task View or signing in again.

## Job and scope
A beautiful Windows 11 desktop object displaying one personal goal, edited roughly monthly. Exceptional visual quality matters more than features.

The user approved a compact square with a frosted-glass landscape, then requested text centered horizontally and vertically with centered lines. They accepted the adjustment and authorized finalizing the plan. The temporary preview is the visual reference, not proof of native Windows feasibility.

MVP: one goal, editing, local persistence, dragging, saved position. Other apps cover it. Win+D support is optional and must be skipped if it requires fragile desktop integration.

Excluded: timer, accounts, sync, multiple goals, statistics, streaks, theme/landscape galleries, resizing controls and tray menu. iPhone/iPad widgets are desired later, not current scope.

## Approved design
- Current adjustment (2026-09-24): user accepts the current visual treatment for now and requests a 30% reduction in card width/height: **210 × 210 logical units**. Scale resting text and spacing by 0.7 (font range 27.3–16.1); keep editor/menu controls full-sized. This supersedes the original starting dimensions and font sizes below. User also prioritizes reducing app disk use; additional glass refinement is deferred.
- Reference: `preview/index.html`. Original root PNG is inspiration, not a strict specification. The old `preview/preview.png` predates centering.
- Start at 300 × 300 Windows logical units, which scale with display settings.
- Georgia serif, regular, centered; starting font size 39, line height approximately 1.04. Match visual appearance rather than blindly copying browser measurements.
- Default goal with deliberate line breaks: “Build” / “something” / “people want.”
- Navy text `#192B46`, pale frost `#EDF0F4`, muted mountain blues, quiet warm glow. Maintain contrast where the centered goal overlaps the mountains.
- Target 30-unit corners, fine white rim, restrained shadow, faint internal highlights. Native corner constraints must be addressed early.
- One fixed landscape. Current preview photo is provisional; clear its provenance/license or replace before distribution.
- No persistent buttons/labels inside the resting widget. Prototype header, caption, wallpaper/zoom controls are preview aids only.
- No idle animation, cursor-following glare, parallax, or continuous redraw.

## Flow
1. Launch from shortcut; restore goal/position. First launch uses default text near the primary display's upper-right usable area.
2. Drag unused card space; save position when movement ends. Restore into a visible connected display if screen configuration changed.
3. Right-click → Edit goal. Keyboard: focus window, Shift+F10. Context menu also includes Quit because there is no title bar.
4. Small edit dialog: labeled text field, Save, Cancel. Preserve explicit line breaks; Escape cancels, Enter inserts a line break.
5. Allow 1–100 text elements and up to five explicit lines; reject blank text. Fit wrapping within the card down to size 23. If it still cannot fit, ask for a shorter goal rather than clipping it.
6. Save locally before reporting success. On failure, retain draft/editor with a plain explanation and retry. Cancel changes nothing.
7. Quit/relaunch preserves goal and position. App use needs no network.

## Windows behavior and quality
Use an ordinary non-topmost desktop window without visible title bar; other apps cover it. No repeated foreground activation. Normal activation while editing is acceptable. User clarified on 2026-09-24: start automatically at Windows sign-in, remain behind other apps, and remove the taskbar entry. Use a desktop shortcut for reopening and the context menu for Quit. Current-user Startup and Desktop shortcuts are configured; changes to hide taskbar/Alt+Tab and disable minimization have been rebuilt and reopened; visual verification remains pending. The user wants one widget across all Windows virtual desktops using the pinning setup above; the result remains unverified.

Win+D behavior remains unverified; restore via shortcut if needed. Do not promise wallpaper-level attachment or use undocumented Explorer tricks. Win+D survival is not a release criterion.

Keyboard focus must be visible. Check 100/125/150/200% display scaling when authorized. Standard text targets 4.5:1 contrast; large goal text at least 3:1, preferably 4.5:1. Respect high contrast and reduced motion. Keep an attractive tinted fallback when transparency is unavailable. The goal stays centered for all accepted text.

## Future Apple direction
Keep goal content/artwork separate from Windows behavior. Apple widgets use WidgetKit/SwiftUI, Apple's widget/interface tools; expect a separate Apple interface. Do not add shared frameworks or sync services now or promise identical glass rendering. [Apple documentation](https://developer.apple.com/documentation/widgetkit)

