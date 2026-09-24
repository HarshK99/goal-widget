using Microsoft.UI.Xaml;

namespace GoalWidget;

public partial class App : Application
{
    private WidgetWindow? window;
    public App()
    {
        UnhandledException += (_, args) => RecordStartupFailure(args.Exception);
        try { InitializeComponent(); }
        catch (Exception ex) { RecordStartupFailure(ex); throw; }
    }

    private static void RecordStartupFailure(Exception exception)
    {
        var log = Environment.GetEnvironmentVariable("GOAL_WIDGET_STARTUP_LOG");
        if (string.IsNullOrWhiteSpace(log)) return;
        try { File.AppendAllText(log, DateTimeOffset.Now + "\n" + exception + "\n"); }
        catch (Exception) { /* Diagnostic logging must not replace the original failure. */ }
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var store = new GoalStore();
        store.Load();
        window = new WidgetWindow(store);
        Program.SetActivationHandler(window.DispatcherQueue, window.Restore);
        window.Activate();
    }
}
