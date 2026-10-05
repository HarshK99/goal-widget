using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace GoalWidget;

/// <summary>
/// Places the widget in physical pixels. Positions refer to the card; the window is
/// larger on every side by the room left for the card's shadow.
/// </summary>
internal static class WindowPlacement
{
    private sealed record Display(Native.MonitorInfo Info, uint Dpi);

    private static Display? Describe(nint monitor)
    {
        var info = new Native.MonitorInfo { Size = (uint)Marshal.SizeOf<Native.MonitorInfo>(), Device = "" };
        if (!Native.GetMonitorInfo(monitor, ref info)) return null;
        var result = Native.GetDpiForMonitor(monitor, 0, out var dpi, out _);
        return new(info, result == 0 && dpi > 0 ? dpi : 96);
    }

    private static List<Display> Displays()
    {
        var displays = new List<Display>();
        Native.EnumDisplayMonitors(0, 0, (monitor, _, _, _) =>
        {
            if (Describe(monitor) is { } display) displays.Add(display);
            return true;
        }, 0);
        return displays;
    }

    internal static void Restore(nint window, SavedPlacement? saved, double shadowRoom)
    {
        var displays = Displays();
        if (displays.Count == 0) return;
        var primary = displays.FirstOrDefault(d => (d.Info.Flags & 1) != 0) ?? displays[0];
        var chosen = saved is null ? primary : displays.FirstOrDefault(d => d.Info.Device == saved.MonitorId) ?? primary;
        var scale = chosen.Dpi / 96d;
        var size = (int)Math.Round(GoalLayout.CardSize * scale);
        var margin = (int)Math.Round(24 * scale);
        var work = chosen.Info.Work;
        // First launch: upper-right of the primary display. Clamp even when a monitor's name survives a rearrangement.
        var x = saved?.X ?? work.Right - size - margin;
        var y = saved?.Y ?? work.Top + margin;
        Move(window, Clamp(x, work.Left, work.Right - size), Clamp(y, work.Top, work.Bottom - size), shadowRoom * scale);
    }

    /// <summary>Keeps the card inside the nearest display's usable area and reports where it ended up.</summary>
    internal static SavedPlacement? Settle(nint window, double shadowRoom)
    {
        if (Describe(Native.MonitorFromWindow(window, 2)) is not { } display || !Native.GetWindowRect(window, out var rect))
            return null;
        var scale = display.Dpi / 96d;
        var room = (int)Math.Round(shadowRoom * scale);
        var size = (int)Math.Round(GoalLayout.CardSize * scale);
        var work = display.Info.Work;
        var x = Clamp(rect.Left + room, work.Left, work.Right - size);
        var y = Clamp(rect.Top + room, work.Top, work.Bottom - size);
        Move(window, x, y, shadowRoom * scale);
        return new(x, y, display.Info.Device, display.Dpi);
    }

    private static void Move(nint window, int cardX, int cardY, double room) =>
        Native.SetWindowPos(window, 0, cardX - (int)Math.Round(room), cardY - (int)Math.Round(room), 0, 0,
            Native.NoSize | Native.NoZOrder | Native.NoActivate);

    private static int Clamp(int value, int minimum, int maximum) => Math.Max(minimum, Math.Min(value, Math.Max(minimum, maximum)));
}
