using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace GoalWidget;

public partial class WidgetWindow : Window
{
    private const double ShadowRoom = 26; // The margin around the card in the XAML.
    private readonly GoalStore store;
    private nint handle;
    private DesktopPin? pin;
    private TrayIcon? tray;
    private EditGoalWindow? editor;
    private Native.Point dragCursor;
    private Native.Rect dragWindow;
    private bool dragging, moved;
    private Task positionSave = Task.CompletedTask;

    public WidgetWindow(GoalStore store)
    {
        this.store = store;
        InitializeComponent();
        ApplyGoal();
        ApplyContrast();
        SystemParameters.StaticPropertyChanged += ContrastChanged;
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        var source = (HwndSource)PresentationSource.FromVisual(this);
        handle = source.Handle;
        var desktop = new DesktopPin(source);
        pin = desktop;
        WindowPlacement.Restore(handle, store.Current.Placement, ShadowRoom);
        source.AddHook(Message);
        tray = new TrayIcon(desktop.ToggleLift, () => desktop.Lifted, Run);
        Loaded += (_, _) => desktop.Evaluate();
    }

    private void Run(TrayIcon.Command command)
    {
        switch (command)
        {
            case TrayIcon.Command.Edit: OpenEditor(); break;
            case TrayIcon.Command.Lift: pin?.ToggleLift(); break;
            case TrayIcon.Command.Quit: Close(); break;
        }
    }

    private void ApplyGoal()
    {
        var text = store.Current.GoalText;
        var size = GoalLayout.Fit(text);
        if (size is null) Log.Write("The saved goal does not fit the card; showing it at the smallest size.");
        Goal.Text = text;
        Goal.FontSize = size ?? GoalLayout.MinimumFontSize;
        Goal.LineHeight = Goal.FontSize * 1.04;
        AutomationProperties.SetName(this, "Your goal: " + text);
    }

    private void OpenEditor()
    {
        if (editor is not null) { editor.Activate(); return; }
        editor = new EditGoalWindow(store);
        editor.Closed += (_, _) =>
        {
            editor = null;
            ApplyGoal();
        };
        editor.Show();
        editor.Activate();
    }

    private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2) { OpenEditor(); return; }
        // The widget is never the active window, so it is moved by hand, not by the system's move loop.
        Native.GetCursorPos(out dragCursor);
        Native.GetWindowRect(handle, out dragWindow);
        moved = false;
        dragging = Card.CaptureMouse();
    }

    private void Card_MouseMove(object sender, MouseEventArgs e)
    {
        if (!dragging) return;
        Native.GetCursorPos(out var cursor);
        int dx = cursor.X - dragCursor.X, dy = cursor.Y - dragCursor.Y;
        if (!moved && Math.Abs(dx) + Math.Abs(dy) < 6) return;
        moved = true;
        Native.SetWindowPos(handle, 0, dragWindow.Left + dx, dragWindow.Top + dy, 0, 0,
            Native.NoSize | Native.NoZOrder | Native.NoActivate);
    }

    private void Card_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => Card.ReleaseMouseCapture();

    private void Card_LostMouseCapture(object sender, MouseEventArgs e)
    {
        if (!dragging) return;
        dragging = false;
        if (moved) positionSave = SavePositionAsync();
    }

    private void Card_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        Native.GetCursorPos(out var cursor);
        tray?.ShowMenu(cursor.X, cursor.Y);
    }

    private async Task SavePositionAsync()
    {
        try
        {
            if (WindowPlacement.Settle(handle, ShadowRoom) is { } placement) await store.SavePlacementAsync(placement);
        }
        catch (Exception ex) { Log.Write("Position not saved: " + ex.Message); } // A lost position must never take the widget down.
    }

    private nint Message(nint hwnd, int message, nint wParam, nint lParam, ref bool handled)
    {
        // Keep the card reachable when a display is removed or rearranged, without overwriting the saved spot.
        if (message == 0x007E) Dispatcher.BeginInvoke(new Action(() => WindowPlacement.Settle(handle, ShadowRoom))); // WM_DISPLAYCHANGE
        return 0;
    }

    private void ContrastChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SystemParameters.HighContrast)) ApplyContrast();
    }

    private void ApplyContrast()
    {
        var high = SystemParameters.HighContrast;
        Artwork.Visibility = high ? Visibility.Collapsed : Visibility.Visible;
        Plate.Background = high ? SystemColors.WindowBrush : (Brush)FindResource("GoalFrost");
        Goal.Foreground = high ? SystemColors.WindowTextBrush : (Brush)FindResource("GoalInk");
        Rim.BorderBrush = high ? SystemColors.WindowTextBrush : (Brush)FindResource("GoalRim");
    }

    protected override async void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        if (editor is { Saving: true }) { e.Cancel = true; editor.Activate(); return; }
        if (positionSave.IsCompleted) return;
        // Let the last move reach the disk before quitting.
        e.Cancel = true;
        await positionSave;
        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        SystemParameters.StaticPropertyChanged -= ContrastChanged;
        editor?.Close();
        tray?.Dispose();
        pin?.Dispose();
        Log.Write("Quit.");
        Application.Current.Shutdown();
    }
}
