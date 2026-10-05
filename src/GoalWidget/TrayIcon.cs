using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace GoalWidget;

/// <summary>Notification-area icon and the widget's one menu, shared by the icon and the card.</summary>
internal sealed class TrayIcon : IDisposable
{
    internal enum Command { None, Edit, Lift, Quit }
    private const int Callback = 0x8001; // WM_APP + 1: the shell reports icon input with it.

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

    private readonly HwndSource host;
    private readonly nint icon;
    private readonly bool ownsIcon;
    private readonly int taskbarCreated = (int)RegisterWindowMessage("TaskbarCreated");
    private readonly Action clicked;
    private readonly Func<bool> lifted;
    private readonly Action<Command> chosen;
    private int lastKeySelect;
    private bool disposed;

    internal TrayIcon(Action clicked, Func<bool> lifted, Action<Command> chosen)
    {
        this.clicked = clicked;
        this.lifted = lifted;
        this.chosen = chosen;
        // A hidden top-level window: it receives the icon's input and Explorer's broadcast, and owns the menu.
        host = new HwndSource(new HwndSourceParameters("Goal")
        {
            WindowStyle = unchecked((int)0x80000000), // WS_POPUP
            ExtendedWindowStyle = 0x80,               // WS_EX_TOOLWINDOW
            Width = 0,
            Height = 0
        });
        host.AddHook(Message);
        var size = GetSystemMetrics(49); // SM_CXSMICON
        icon = LoadImage(0, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "goal.ico"), 1, size, size, 0x10); // IMAGE_ICON, LR_LOADFROMFILE
        ownsIcon = icon != 0;
        if (!ownsIcon) icon = LoadIcon(0, 32512); // IDI_APPLICATION, shared with the system.
        Add();
    }

    internal void ShowMenu(int x, int y)
    {
        var menu = CreatePopupMenu();
        AppendMenu(menu, 0, (nuint)Command.Edit, "Edit goal");
        AppendMenu(menu, lifted() ? 0x8u : 0u, (nuint)Command.Lift, "Show above other windows"); // MF_CHECKED
        AppendMenu(menu, 0x800, 0, null); // MF_SEPARATOR
        AppendMenu(menu, 0, (nuint)Command.Quit, "Quit");
        // A popup menu only dismisses on an outside click when its owner is the foreground window.
        var previous = Native.GetForegroundWindow();
        Native.SetForegroundWindow(host.Handle);
        var command = (Command)TrackPopupMenuEx(menu, 0x100 | 0x80 | 0x2, x, y, host.Handle, 0); // TPM_RETURNCMD | TPM_NONOTIFY | TPM_RIGHTBUTTON
        DestroyMenu(menu);
        // Hand focus back to the app in use, unless the editor is about to take it.
        if (command != Command.Edit && previous != 0) Native.SetForegroundWindow(previous);
        if (command != Command.None) chosen(command);
    }

    private nint Message(nint hwnd, int message, nint wParam, nint lParam, ref bool handled)
    {
        if (message == Callback)
        {
            // Version 4 icons report the event in the low word and the screen position in wParam.
            var input = (int)(lParam & 0xFFFF);
            int x = (short)(wParam & 0xFFFF), y = (short)((wParam >> 16) & 0xFFFF);
            if (input == 0x0400) Post(clicked); // NIN_SELECT
            else if (input == 0x0401) // NIN_KEYSELECT; Enter reports it twice.
            {
                var now = Environment.TickCount;
                if (unchecked(now - lastKeySelect) > 500) Post(clicked);
                lastKeySelect = now;
            }
            else if (input == 0x007B) Post(() => ShowMenu(x, y)); // WM_CONTEXTMENU
        }
        else if (taskbarCreated != 0 && message == taskbarCreated) Add(); // Explorer restarted, or was not ready at sign-in.
        return 0;
    }

    // Run after the window procedure returns: the menu pumps its own message loop.
    private void Post(Action action) => host.Dispatcher.BeginInvoke(action);

    private void Add()
    {
        if (disposed) return;
        var data = Data();
        data.Flags = 0x1 | 0x2 | 0x4 | 0x80; // NIF_MESSAGE | NIF_ICON | NIF_TIP | NIF_SHOWTIP
        data.CallbackMessage = Callback;
        data.Icon = icon;
        data.Tip = "Goal";
        if (!Shell_NotifyIcon(0, ref data)) return; // NIM_ADD; retried on TaskbarCreated.
        data.Version = 4; // NOTIFYICON_VERSION_4
        Shell_NotifyIcon(4, ref data); // NIM_SETVERSION
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        var data = Data();
        Shell_NotifyIcon(2, ref data); // NIM_DELETE
        if (ownsIcon) DestroyIcon(icon);
        host.Dispose();
    }

    private NotifyIconData Data() => new() { Size = (uint)Marshal.SizeOf<NotifyIconData>(), Window = host.Handle, Id = 1 };

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
