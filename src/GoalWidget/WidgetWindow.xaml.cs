using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Composition.SystemBackdrops;
using Windows.Foundation;
using Windows.Graphics;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace GoalWidget;

public sealed partial class WidgetWindow : Window
{
    private readonly GoalStore store;
    private readonly nint handle;
    private readonly WindowPlacement placement;
    private readonly NativeWindow.SubclassProc subclass;
    private readonly AccessibilitySettings accessibility = new();
    private readonly UISettings uiSettings = new();
    private readonly MenuFlyout menu = new();
    private EditGoalDialog? editor;
    private Point? pointerStart;
    private Task positionSave = Task.CompletedTask;
    private bool closing;
    private bool closed;

    public WidgetWindow(GoalStore store)
    {
        this.store = store;
        InitializeComponent();
        Goal.Width = GoalLayout.Width;
        Card.Padding = new Thickness(25 * GoalLayout.CardScale, 29 * GoalLayout.CardScale,
                                     25 * GoalLayout.CardScale, 29 * GoalLayout.CardScale);
        handle = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var presenter = (OverlappedPresenter)AppWindow.Presenter;
        presenter.SetBorderAndTitleBar(true, false);
        presenter.IsResizable = false;
        presenter.IsMaximizable = false;
        presenter.IsMinimizable = false;
        presenter.IsAlwaysOnTop = false;
        AppWindow.IsShownInSwitchers = false;
        NativeWindow.RequestRoundedCorners(handle);
        if (DesktopAcrylicController.IsSupported()) SystemBackdrop = new DesktopAcrylicBackdrop();
        placement = new WindowPlacement(AppWindow, handle);
        placement.Restore(store.Current.Placement);
        subclass = WindowMessage;
        NativeWindow.SetWindowSubclass(handle, subclass, 1, 0);

        menu.MenuFlyoutPresenterStyle = new Style(typeof(MenuFlyoutPresenter));
        menu.MenuFlyoutPresenterStyle.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, 180d));
        var edit = new MenuFlyoutItem { Text = "Edit goal", MinHeight = 44 };
        edit.Click += (_, _) => OpenEditor();
        var quit = new MenuFlyoutItem { Text = "Quit", MinHeight = 44 };
        quit.Click += (_, _) => Close();
        menu.Items.Add(edit);
        var taskView = new ToggleMenuFlyoutItem { Text = "Show in Task View", MinHeight = 44 };
        taskView.Click += (_, _) => AppWindow.IsShownInSwitchers = taskView.IsChecked;
        menu.Items.Add(taskView);
        menu.Items.Add(new MenuFlyoutSeparator());
        menu.Items.Add(quit);
        Card.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(Card_PointerPressed), true);
        Card.AddHandler(UIElement.PointerMovedEvent, new PointerEventHandler(Card_PointerMoved), true);
        Card.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler((_, _) => pointerStart = null), true);
        Card.PointerCanceled += (_, _) => pointerStart = null;
        Card.PointerCaptureLost += (_, _) => pointerStart = null;
        Root.Loaded += (_, _) =>
        {
            ApplyGoal();
            Card.Focus(FocusState.Programmatic);
            if (store.Notice is not null) ShowNotice(store.Notice);
        };
        uiSettings.TextScaleFactorChanged += TextScaleChanged;
        ApplyContrast();
        AppWindow.Closing += OnClosing;
        Closed += (_, _) =>
        {
            closed = true;
            uiSettings.TextScaleFactorChanged -= TextScaleChanged;
            NativeWindow.RemoveWindowSubclass(handle, subclass, 1);
            Application.Current.Exit();
        };
    }

    public void Restore()
    {
        if (closed) return;
        NativeWindow.ShowWindow(handle, 9);
        placement.ResizeAndClamp();
        if (editor is not null) editor.Restore();
        else { Activate(); NativeWindow.SetForegroundWindow(handle); }
    }

    private void ApplyGoal()
    {
        var size = GoalLayout.Fit(store.Current.GoalText);
        Goal.Text = store.Current.GoalText;
        Goal.FontSize = size ?? GoalLayout.MinimumFontSize;
        Goal.LineHeight = Goal.FontSize * 1.04;
        // A saved goal may cease to fit after system text scaling changes. Keep it readable in a scroll area.
        if (size is null)
        {
            if (Card.Content is not ScrollViewer)
            {
                Card.Content = null;
                Card.Content = new ScrollViewer { Content = Goal, MaxHeight = GoalLayout.Height, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            }
            ShowNotice("Your saved goal needs more room at this text size. Edit it to shorten it.");
        }
        else if (Card.Content is ScrollViewer scroll)
        {
            scroll.Content = null;
            Card.Content = Goal;
        }
        Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(Card, "Your goal: " + store.Current.GoalText);
    }

    private void OpenEditor()
    {
        if (editor is not null) { editor.Restore(); return; }
        pointerStart = null;
        editor = new EditGoalDialog(this, store);
        editor.Closed += (_, _) =>
        {
            editor = null;
            NativeWindow.EnableWindow(handle, true);
            ApplyGoal();
            Activate();
            Card.Focus(FocusState.Keyboard);
        };
        NativeWindow.EnableWindow(handle, false);
        editor.Activate();
    }

    private void Card_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        pointerStart = null;
        menu.ShowAt(Card, new Microsoft.UI.Xaml.Controls.Primitives.FlyoutShowOptions { Position = e.GetPosition(Card) });
        e.Handled = true;
    }

    private void Card_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        static bool Down(VirtualKey key) => (InputKeyboardSource.GetKeyStateForCurrentThread(key) & CoreVirtualKeyStates.Down) != 0;
        if ((e.Key == VirtualKey.F10 && Down(VirtualKey.Shift)) || e.Key == VirtualKey.Application)
        {
            menu.ShowAt(Card);
            e.Handled = true;
        }
        else if (Down(VirtualKey.Menu) && e.Key is VirtualKey.Left or VirtualKey.Right or VirtualKey.Up or VirtualKey.Down)
        {
            var position = AppWindow.Position;
            var step = (int)Math.Round(10 * NativeWindow.GetDpiForWindow(handle) / 96d);
            var dx = e.Key == VirtualKey.Left ? -step : e.Key == VirtualKey.Right ? step : 0;
            var dy = e.Key == VirtualKey.Up ? -step : e.Key == VirtualKey.Down ? step : 0;
            AppWindow.Move(new PointInt32(position.X + dx, position.Y + dy));
            placement.ResizeAndClamp();
            positionSave = SavePositionAsync();
            e.Handled = true;
        }
        else if (e.Key is VirtualKey.Enter or VirtualKey.Space)
        {
            OpenEditor();
            e.Handled = true;
        }
    }

    private void Card_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint(Card);
        if (point.Properties.IsLeftButtonPressed && Card.Content is not ScrollViewer)
            pointerStart = point.Position;
    }

    private void Card_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (pointerStart is not Point start || editor is not null) return;
        var point = e.GetCurrentPoint(Card);
        if (!point.Properties.IsLeftButtonPressed) { pointerStart = null; return; }
        if (Math.Abs(point.Position.X - start.X) + Math.Abs(point.Position.Y - start.Y) < 6) return;
        pointerStart = null;
        Card.ReleasePointerCaptures();
        NativeWindow.ReleaseCapture();
        // The system move loop provides normal monitor crossing and WM_EXITSIZEMOVE.
        NativeWindow.SendMessage(handle, 0x0112, 0xF012, 0); // WM_SYSCOMMAND, SC_MOVE | HTCAPTION
        e.Handled = true;
    }

    private nint WindowMessage(nint hwnd, uint message, nuint wParam, nint lParam, nuint id, nuint data)
    {
        // AccessibilitySettings.HighContrastChanged requires a UWP window and throws
        // in this desktop app. Use the standard desktop settings/theme broadcasts.
        if (message is 0x001A or 0x031A) // WM_SETTINGCHANGE, WM_THEMECHANGED.
            DispatcherQueue.TryEnqueue(() =>
            {
                if (!closed) ApplyContrast();
            });
        if (message is 0x0232 or 0x02E0 or 0x007E) // End move, DPI change, display change.
            DispatcherQueue.TryEnqueue(() =>
            {
                if (closed || NativeWindow.IsIconic(handle)) return;
                placement.ResizeAndClamp();
                if (message != 0x02E0) positionSave = SavePositionAsync();
            });
        return NativeWindow.DefSubclassProc(hwnd, message, wParam, lParam);
    }

    private async Task SavePositionAsync()
    {
        try { await store.SavePlacementAsync(placement.Capture()); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            ShowNotice("Your position could not be saved. Move the card again to retry. " + ex.Message);
        }
    }

    private async void OnClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (closing) return;
        args.Cancel = true;
        if (editor is not null) { editor.Restore(); return; }
        await positionSave;
        closing = true;
        DispatcherQueue.TryEnqueue(Close);
    }

    private void ShowNotice(string message) { Notice.Message = message; Notice.IsOpen = true; }
    private void TextScaleChanged(UISettings sender, object args) => DispatcherQueue.TryEnqueue(ApplyGoal);
    private void ApplyContrast()
    {
        Artwork.Visibility = accessibility.HighContrast ? Visibility.Collapsed : Visibility.Visible;
        Root.Background = accessibility.HighContrast
            ? new SolidColorBrush(uiSettings.GetColorValue(UIColorType.Background))
            : SystemBackdrop is null ? (Brush)Application.Current.Resources["GoalFrost"]
            : new SolidColorBrush(Microsoft.UI.Colors.Transparent);
        Goal.Foreground = accessibility.HighContrast
            ? new SolidColorBrush(uiSettings.GetColorValue(UIColorType.Foreground))
            : (Brush)Application.Current.Resources["GoalInk"];
    }
}
