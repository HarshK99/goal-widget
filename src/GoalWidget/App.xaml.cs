using System;
using System.Threading;
using System.Windows;

namespace GoalWidget;

public partial class App : Application
{
    private Mutex? single;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        // The widget is always on the desktop, so a second launch has nothing to do.
        single = new Mutex(true, "GoalWidget.Desktop.v2", out var first);
        if (!first) { Shutdown(); return; }

        DispatcherUnhandledException += (_, args) => Log.Write("Crash: " + args.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, args) => Log.Write("Crash: " + args.ExceptionObject);
        Log.Write("Started.");

        var store = new GoalStore();
        store.Load();
        new WidgetWindow(store).Show();
        if (store.Notice is not null)
        {
            Log.Write(store.Notice);
            MessageBox.Show(store.Notice, "Goal", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        single?.Dispose();
        base.OnExit(e);
    }
}
