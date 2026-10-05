using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;

namespace GoalWidget;

/// <summary>
/// Makes a window lie on the desktop. It never takes focus and stays beneath every app.
/// As an unowned tool window it is also left out of the taskbar, Alt+Tab and
/// per-desktop tracking, so no pinning is needed for virtual desktops.
/// </summary>
internal sealed class DesktopPin : IDisposable
{
    private readonly nint window;
    private readonly HwndSource probe;
    private readonly Native.WinEventProc foregroundChanged;
    private readonly nint hook;
    private readonly DispatcherTimer desktopWatch;
    private nint desktopHost, liftedOver;
    private bool ordering, overDesktop;

    /// <summary>The user asked to see the widget above other windows for a moment.</summary>
    internal bool Lifted { get; private set; }

    internal DesktopPin(HwndSource source)
    {
        window = source.Handle;
        var style = (long)Native.GetWindowLongPtr(window, -20);  // GWL_EXSTYLE
        style = (style | 0x80 | 0x08000000) & ~0x40000L;         // + TOOLWINDOW, NOACTIVATE; - APPWINDOW
        Native.SetWindowLongPtr(window, -20, (nint)style);
        source.AddHook(FreezeOrder);

        // A hidden window that is never raised. Show Desktop lifts the desktop above it.
        probe = new HwndSource(new HwndSourceParameters("GoalWidget.Probe")
        {
            WindowStyle = unchecked((int)0x88000000), // WS_POPUP | WS_DISABLED
            ExtendedWindowStyle = 0x80,               // WS_EX_TOOLWINDOW
            Width = 0,
            Height = 0
        });
        probe.AddHook(FreezeOrder);

        desktopWatch = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
        desktopWatch.Tick += (_, _) => Evaluate();
        foregroundChanged = (_, _, _, _, _, _, _) => Evaluate();
        hook = Native.SetWinEventHook(3, 3, 0, foregroundChanged, 0, 0, 0); // EVENT_SYSTEM_FOREGROUND, delivered on this thread
        Place();
    }

    internal void ToggleLift()
    {
        Lifted = !Lifted;
        liftedOver = Native.GetForegroundWindow();
        Place();
    }

    /// <summary>Re-reads the desktop's state and puts the widget where it belongs.</summary>
    internal void Evaluate()
    {
        // A lift lasts until the user moves on to another app.
        var foreground = Native.GetForegroundWindow();
        if (Lifted && foreground != 0 && foreground != liftedOver && !IsOurs(foreground)) Lifted = false;

        var desktop = ShowDesktopActive();
        if (desktop != overDesktop)
        {
            overDesktop = desktop;
            desktopWatch.IsEnabled = desktop; // Leaving Show Desktop does not always change the foreground window.
            Log.Write(desktop ? "Show Desktop on: widget raised." : "Show Desktop off: widget back on the desktop.");
        }
        Place();
    }

    private void Place()
    {
        const uint flags = Native.NoSize | Native.NoMove | Native.NoActivate;
        ordering = true;
        try
        {
            Native.SetWindowPos(probe.Handle, Native.Bottom, 0, 0, 0, 0, flags);
            if (Lifted || overDesktop) Native.SetWindowPos(window, Native.Topmost, 0, 0, 0, 0, flags);
            else
            {
                Native.SetWindowPos(window, Native.NotTopmost, 0, 0, 0, 0, flags);
                Native.SetWindowPos(window, Native.Bottom, 0, 0, 0, 0, flags);
            }
        }
        finally { ordering = false; }
    }

    // Only Place may change the stacking order; clicks and other apps cannot raise the widget.
    private nint FreezeOrder(nint hwnd, int message, nint wParam, nint lParam, ref bool handled)
    {
        if (message == 0x0046 && !ordering) // WM_WINDOWPOSCHANGING
        {
            var position = Marshal.PtrToStructure<Native.WindowPos>(lParam);
            position.Flags |= Native.NoZOrder;
            Marshal.StructureToPtr(position, lParam, false);
        }
        return 0;
    }

    private bool ShowDesktopActive()
    {
        var host = DesktopHost();
        if (host == 0 || !Native.IsWindowVisible(host)) return false;
        for (var below = Native.GetWindow(host, 2); below != 0; below = Native.GetWindow(below, 2)) // GW_HWNDNEXT
            if (below == probe.Handle) return true;
        return false;
    }

    // The top-level window that holds the desktop icons.
    private nint DesktopHost()
    {
        if (desktopHost != 0 && Native.FindWindowEx(desktopHost, 0, "SHELLDLL_DefView", null) != 0) return desktopHost;
        nint found = 0;
        Native.EnumWindows((candidate, _) =>
        {
            if (Native.FindWindowEx(candidate, 0, "SHELLDLL_DefView", null) == 0) return true;
            found = candidate;
            return false;
        }, 0);
        return desktopHost = found;
    }

    private static bool IsOurs(nint other)
    {
        Native.GetWindowThreadProcessId(other, out var process);
        return process == Native.GetCurrentProcessId();
    }

    public void Dispose()
    {
        desktopWatch.Stop();
        Native.UnhookWinEvent(hook);
        probe.Dispose();
    }
}
