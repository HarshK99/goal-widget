using System.Globalization;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Graphics;

namespace GoalWidget;

public sealed partial class EditGoalDialog : Window
{
    private readonly GoalStore store;
    private readonly nint handle;
    private bool saving;
    private bool shown;

    public EditGoalDialog(Window owner, GoalStore store)
    {
        this.store = store;
        InitializeComponent();
        handle = WinRT.Interop.WindowNative.GetWindowHandle(this);
        NativeWindow.SetWindowLongPtr(handle, -8, WinRT.Interop.WindowNative.GetWindowHandle(owner)); // Owned window.
        AppWindow.IsShownInSwitchers = false;
        var presenter = (OverlappedPresenter)AppWindow.Presenter;
        presenter.IsMaximizable = false;
        presenter.IsMinimizable = false;
        // Resizable editor accommodates larger system text; the resting card remains fixed.
        var dpi = NativeWindow.GetDpiForWindow(WinRT.Interop.WindowNative.GetWindowHandle(owner));
        var scale = dpi / 96d;
        var work = DisplayArea.GetFromWindowId(owner.AppWindow.Id, DisplayAreaFallback.Primary).WorkArea;
        var width = Math.Min((int)(460 * scale), work.Width);
        var height = Math.Min((int)(540 * scale), work.Height);
        AppWindow.MoveAndResize(new RectInt32(
            Math.Clamp(owner.AppWindow.Position.X, work.X, work.X + work.Width - width),
            Math.Clamp(owner.AppWindow.Position.Y, work.Y, work.Y + work.Height - height), width, height));
        Draft.Text = store.Current.GoalText;
        AppWindow.Closing += (_, args) => args.Cancel = saving;
    }

    public void Restore()
    {
        NativeWindow.ShowWindow(handle, 9);
        Activate();
        NativeWindow.SetForegroundWindow(handle);
    }

    private async void Host_Loaded(object sender, RoutedEventArgs args)
    {
        if (shown) return;
        shown = true;
        Editor.XamlRoot = Host.XamlRoot;
        Editor.Opened += (_, _) => Draft.Focus(FocusState.Programmatic);
        if (store.WriteBlock is not null) ShowError(store.WriteBlock);
        await Editor.ShowAsync();
        Close();
    }

    private void Draft_Changed(object sender, TextChangedEventArgs args)
    {
        // XAML can raise this before later named controls are constructed.
        if (Count is null || Error is null) return;
        var text = GoalTextRules.Normalize(Draft.Text);
        Count.Text = $"{new StringInfo(text).LengthInTextElements}/100 characters · {text.Split('\n').Length}/5 lines";
        Error.Text = store.WriteBlock ?? string.Empty;
        Error.Visibility = store.WriteBlock is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private async void Editor_Save(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var text = GoalTextRules.Normalize(Draft.Text);
        var error = GoalTextRules.Validate(text);
        if (error is null && GoalLayout.Fit(text) is null)
            error = "This goal will not fit on the card. Shorten it or use fewer lines.";
        if (error is not null) { args.Cancel = true; ShowError(error); return; }
        var deferral = args.GetDeferral();
        saving = true;
        SaveStatus.Visibility = Visibility.Visible;
        Editor.PrimaryButtonText = "Saving…";
        Draft.IsEnabled = false;
        Editor.IsPrimaryButtonEnabled = false;
        Editor.IsSecondaryButtonEnabled = false;
        try
        {
            await store.SaveGoalAsync(text);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            args.Cancel = true;
            ShowError("Your goal was not saved. Your draft is still here; try Save again. " + ex.Message);
        }
        finally
        {
            saving = false;
            SaveStatus.Visibility = Visibility.Collapsed;
            Editor.PrimaryButtonText = "Save";
            Draft.IsEnabled = true;
            Editor.IsPrimaryButtonEnabled = true;
            Editor.IsSecondaryButtonEnabled = true;
            if (args.Cancel) Draft.Focus(FocusState.Programmatic);
            deferral.Complete();
        }
    }

    private void Editor_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        if (saving) args.Cancel = true;
    }

    private void ShowError(string message)
    {
        Error.Text = message;
        Error.Visibility = Visibility.Visible;
        Draft.Focus(FocusState.Programmatic);
    }
}
