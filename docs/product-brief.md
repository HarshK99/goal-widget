# Goal widget — product brief

> Revised 2026-10-05. The first brief asked for "an ordinary non-topmost desktop window". That was wrong for a widget meant to stay on the desktop: an ordinary app window takes focus, belongs to one virtual desktop and needs patches to hide it. The user asked for a rebuild from first principles. This brief replaces the window model and the toolkit; the visual design and the saving rules are unchanged.

## Job and scope
A beautiful Windows 11 desktop object displaying one personal goal, edited roughly monthly. It lies on the desktop like a note on a desk. Exceptional visual quality matters more than features.

MVP: one goal, editing, local persistence, dragging, saved position, a tray icon, start at sign-in.

Excluded: timer, accounts, sync, multiple goals, statistics, streaks, theme/landscape galleries and resizing controls. iPhone/iPad widgets are desired later, not current scope.

## Desktop behavior (the core requirement)
1. **Stays on the desktop.** Above the wallpaper and icons, beneath every app window. It never comes forward by itself.
2. **Never takes focus.** Not at sign-in, not when clicked, not when switching virtual desktops.
3. **On every virtual desktop**, with no setup or pinning.
4. **No taskbar button and no Alt+Tab entry.**
5. **Starts at every sign-in** without the user doing anything.
6. **A tray icon** (the small icons beside the clock) is its handle: left-click shows the widget above other windows until the user moves to another app; right-click offers Edit goal, Show above other windows and Quit.
7. **Win+D (Show Desktop)** should leave the widget visible. This is wanted but not a release criterion.

How this is achieved: the widget is an unowned tool window flagged never-activate, held at the bottom of the window stack, and only the app itself may change its stacking order. Only documented Windows APIs are used. It is not attached to the wallpaper and no Explorer internals are modified.

## Approved design
- Card: **210 × 210 logical units** (the original 300 scaled by 0.7), with text and spacing scaled the same way; font range 27.3–16.1. Editor controls stay full-sized.
- Reference: `preview/index.html`. Original root PNG is inspiration, not a strict specification.
- Georgia serif, regular, centered; line height approximately 1.04.
- Default goal with deliberate line breaks: “Build” / “something” / “people want.”
- Navy text `#192B46`, pale frost `#EDF0F4`, muted mountain blues, quiet warm glow. Maintain contrast where the centered goal overlaps the mountains.
- 21-unit corners (the design's 30 at card scale), fine white rim, restrained shadow, faint internal highlights. The app draws these itself.
- One fixed landscape, `Assets/landscape.png`; provenance in `artwork.md`.
- No persistent buttons or labels inside the resting widget.
- No idle animation, cursor-following glare, parallax or continuous redraw.
- Live background blur is not used: Windows turns it off for windows that are not in focus, and the widget never is.

## Flow
1. Starts at sign-in; restores goal and position. First launch uses default text near the primary display's upper-right usable area.
2. Drag the card to move it; the position is saved when the drag ends. If the display setup changes, the card is kept inside a visible display.
3. Double-click the card, or right-click → Edit goal, or use the tray icon's menu. The tray icon is the keyboard route (Win+B).
4. Editor window: text field, Save, Cancel. Explicit line breaks are kept; Escape cancels, Enter inserts a line break. The editor is an ordinary window that comes to the front.
5. Allow 1–100 text elements and up to five explicit lines; reject blank text. Fit wrapping within the card down to size 16.1. If it still cannot fit, ask for a shorter goal rather than clipping it.
6. Save locally before reporting success. On failure, keep the draft and the editor open with a plain explanation. Cancel changes nothing.
7. Quit from either menu. Relaunch preserves goal and position. No network is needed.

## Technology
C# and WPF on .NET Framework 4.8, which is part of Windows 11, so no runtime is bundled. The app folder is about 3 MiB (the WinUI version was 175 MiB). The goal is stored in `%LOCALAPPDATA%\GoalWidget\state.json` with a backup; a small log is kept beside it.

## Quality
Check 100/125/150/200% display scaling when authorized. Large goal text at least 3:1 contrast, preferably 4.5:1. High contrast hides the artwork and uses system colors on the card. The goal stays centered for all accepted text.

Known gaps against the earlier brief: the card no longer takes keyboard focus (the tray menu replaces Shift+F10, Enter and Alt+arrow moves); text does not follow the Windows text-size setting; the editor does not restyle itself for high contrast; WPF has no letter-spacing, so the goal text is set slightly wider than in the preview.

## Future Apple direction
Keep goal content and artwork separate from Windows behavior. Apple widgets use WidgetKit/SwiftUI; expect a separate Apple interface. Do not add shared frameworks or sync services now. [Apple documentation](https://developer.apple.com/documentation/widgetkit)
