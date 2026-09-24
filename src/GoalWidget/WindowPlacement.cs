using Microsoft.UI.Windowing;
using Windows.Graphics;
using System.Runtime.InteropServices;

namespace GoalWidget;

internal sealed class WindowPlacement(AppWindow window, nint handle)
{
    private sealed record Monitor(nint Handle, NativeWindow.MonitorInfo Info, uint Dpi);

    private static List<Monitor> Displays()
    {
        var monitors = new List<Monitor>();
        NativeWindow.EnumDisplayMonitors(0, 0, (monitor, _, _, _) =>
        {
            var info = new NativeWindow.MonitorInfo { Size = (uint)Marshal.SizeOf<NativeWindow.MonitorInfo>(), Device = "" };
            if (NativeWindow.GetMonitorInfo(monitor, ref info))
            {
                var result = NativeWindow.GetDpiForMonitor(monitor, 0, out var dpi, out _);
                monitors.Add(new(monitor, info, result == 0 && dpi > 0 ? dpi : 96));
            }
            return true;
        }, 0);
        return monitors;
    }

    internal void Restore(SavedPlacement? saved)
    {
        var displays = Displays();
        if (displays.Count == 0) return;
        var primary = displays.FirstOrDefault(m => (m.Info.Flags & 1) != 0) ?? displays[0];
        var chosen = saved is null ? primary : displays.FirstOrDefault(m => m.Info.Device == saved.MonitorId) ?? primary;
        var work = chosen.Info.Work;
        var size = (int)Math.Round(GoalLayout.CardSize * chosen.Dpi / 96d);
        var margin = (int)Math.Round(24 * chosen.Dpi / 96d);
        // Coordinates are physical pixels. Clamp even when a monitor's name survives a rearrangement.
        var x = saved?.X ?? work.Right - size - margin;
        var y = saved?.Y ?? work.Top + margin;
        x = Math.Clamp(x, work.Left, Math.Max(work.Left, work.Right - size));
        y = Math.Clamp(y, work.Top, Math.Max(work.Top, work.Bottom - size));
        window.Move(new PointInt32(x, y));
        ResizeAndClamp();
    }

    internal void ResizeAndClamp()
    {
        var dpi = NativeWindow.GetDpiForWindow(handle);
        var size = (int)Math.Round(GoalLayout.CardSize * (dpi == 0 ? 96 : dpi) / 96d);
        window.ResizeClient(new SizeInt32(size, size));
        // ResizeClient can include caption height even with the title bar hidden.
        // Correct from the actual client rectangle rather than hard-coding a caption size.
        if (NativeWindow.GetClientRect(handle, out var client))
        {
            var extraWidth = size - (client.Right - client.Left);
            var extraHeight = size - (client.Bottom - client.Top);
            if (extraWidth != 0 || extraHeight != 0)
                window.Resize(new SizeInt32(window.Size.Width + extraWidth, window.Size.Height + extraHeight));
        }
        var monitor = NativeWindow.MonitorFromWindow(handle, 2);
        var info = new NativeWindow.MonitorInfo { Size = (uint)Marshal.SizeOf<NativeWindow.MonitorInfo>(), Device = "" };
        if (!NativeWindow.GetMonitorInfo(monitor, ref info)) return;
        var work = info.Work;
        var position = window.Position;
        // Include any system border in the visibility calculation.
        window.Move(new PointInt32(
            Math.Clamp(position.X, work.Left, Math.Max(work.Left, work.Right - window.Size.Width)),
            Math.Clamp(position.Y, work.Top, Math.Max(work.Top, work.Bottom - window.Size.Height))));
    }

    internal SavedPlacement Capture()
    {
        var monitor = NativeWindow.MonitorFromWindow(handle, 2);
        var info = new NativeWindow.MonitorInfo { Size = (uint)Marshal.SizeOf<NativeWindow.MonitorInfo>(), Device = "" };
        NativeWindow.GetMonitorInfo(monitor, ref info);
        var position = window.Position;
        return new(position.X, position.Y, info.Device, NativeWindow.GetDpiForWindow(handle));
    }
}
