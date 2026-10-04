using System.Runtime.InteropServices;

namespace GoalWidget;

/// <summary>Notification-area icon: the widget has no taskbar button, so this is its handle.</summary>
internal sealed class TrayIcon : IDisposable
{
    internal const uint Message = 0x8001; // WM_APP + 1: the shell reports icon input with it.
    internal enum Command { None, Edit, TaskView, Quit }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NotifyIconData
    {
        public uint Size;
        public nint Window;
        public uint Id, Flags, CallbackMessage;
        public nint Icon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string Tip;
        public uint State, StateMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string Info;
        public uint Version;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string InfoTitle;
        public uint InfoFlags;
        public Guid Item;
        public nint BalloonIcon;
    }

    private readonly nint window;
    private readonly nint icon;
    private readonly bool ownsIcon;
    private readonly uint taskbarCreated = RegisterWindowMessage("TaskbarCreated");
    private bool disposed;

    internal TrayIcon(nint window)
    {
        this.window = window;
        var size = GetSystemMetrics(49); // SM_CXSMICON
        icon = LoadImage(0, Path.Combine(AppContext.BaseDirectory, "Assets", "goal.ico"), 1, size, size, 0x10); // IMAGE_ICON, LR_LOADFROMFILE
        ownsIcon = icon != 0;
        if (!ownsIcon) icon = LoadIcon(0, 32512); // IDI_APPLICATION, shared with the system.
        Add();
    }

    // Explorer drops every icon when it restarts, and may not be ready yet at sign-in.
    internal bool IsTaskbarCreated(uint message) => taskbarCreated != 0 && message == taskbarCreated;

    internal void Add()
    {
        if (disposed) return;
        var data = Data();
        data.Flags = 0x1 | 0x2 | 0x4 | 0x80; // NIF_MESSAGE | NIF_ICON | NIF_TIP | NIF_SHOWTIP
        data.CallbackMessage = Message;
        data.Icon = icon;
        data.Tip = "Goal";
        if (!Shell_NotifyIcon(0, ref data)) return; // NIM_ADD; retried on TaskbarCreated.
        data.Version = 4; // NOTIFYICON_VERSION_4
        Shell_NotifyIcon(4, ref data); // NIM_SETVERSION
    }

    internal Command ShowMenu(int x, int y, bool taskViewChecked)
    {
        var menu = CreatePopupMenu();
        AppendMenu(menu, 0, (nuint)Command.Edit, "Edit goal");
        AppendMenu(menu, taskViewChecked ? 0x8u : 0u, (nuint)Command.TaskView, "Show in Task View"); // MF_CHECKED
        AppendMenu(menu, 0x800, 0, null); // MF_SEPARATOR
        AppendMenu(menu, 0, (nuint)Command.Quit, "Quit");
        // A popup menu only dismisses on an outside click when its owner is the foreground window.
        NativeWindow.SetForegroundWindow(window);
        var command = TrackPopupMenuEx(menu, 0x100 | 0x80 | 0x2, x, y, window, 0); // TPM_RETURNCMD | TPM_NONOTIFY | TPM_RIGHTBUTTON
        DestroyMenu(menu);
        return (Command)command;
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        var data = Data();
        Shell_NotifyIcon(2, ref data); // NIM_DELETE
        if (ownsIcon) DestroyIcon(icon);
    }

    private NotifyIconData Data() => new() { Size = (uint)Marshal.SizeOf<NotifyIconData>(), Window = window, Id = 1 };

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)] private static extern bool Shell_NotifyIcon(uint message, ref NotifyIconData data);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern uint RegisterWindowMessage(string name);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern nint LoadImage(nint instance, string name, uint type, int width, int height, uint flags);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern nint LoadIcon(nint instance, nint name);
    [DllImport("user32.dll")] private static extern bool DestroyIcon(nint icon);
    [DllImport("user32.dll")] private static extern int GetSystemMetrics(int index);
    [DllImport("user32.dll")] private static extern nint CreatePopupMenu();
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern bool AppendMenu(nint menu, uint flags, nuint id, string? text);
    [DllImport("user32.dll")] private static extern int TrackPopupMenuEx(nint menu, uint flags, int x, int y, nint hwnd, nint parameters);
    [DllImport("user32.dll")] private static extern bool DestroyMenu(nint menu);
}
