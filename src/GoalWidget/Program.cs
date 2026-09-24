using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System.Runtime.InteropServices;

namespace GoalWidget;

internal static class Program
{
    private static readonly object Sync = new();
    private static DispatcherQueue? queue;
    private static Action? activate;
    private static bool pending;

    [STAThread]
    private static void Main()
    {
        WinRT.ComWrappersSupport.InitializeComWrappers();
        var instance = AppInstance.FindOrRegisterForKey("GoalWidget.Desktop.v1");
        if (!instance.IsCurrent)
        {
            NativeWindow.AllowSetForegroundWindow(instance.ProcessId);
            var arguments = AppInstance.GetCurrent().GetActivatedEventArgs();
            using var finished = new EventWaitHandle(false, EventResetMode.ManualReset);
            Exception? failure = null;
            _ = Task.Run(async () =>
            {
                try { await instance.RedirectActivationToAsync(arguments); }
                catch (Exception ex) { failure = ex; }
                finally { finished.Set(); }
            });
            // Pump COM calls while the STA thread waits for activation redirection.
            CoWaitForMultipleObjects(0, uint.MaxValue, 1, [finished.SafeWaitHandle.DangerousGetHandle()], out _);
            if (failure is not null)
                NativeWindow.MessageBox(0, "The existing widget could not be reached. If it is visible, right-click it and choose Quit, then reopen the shortcut. Otherwise, close GoalWidget in Task Manager and try again.", "Goal", 0x10);
            return;
        }
        instance.Activated += (_, _) => RequestActivation();
        Application.Start(parameters =>
        {
            SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
            _ = new App();
        });
        GC.KeepAlive(instance);
    }

    internal static void SetActivationHandler(DispatcherQueue dispatcher, Action handler)
    {
        lock (Sync)
        {
            queue = dispatcher;
            activate = handler;
            if (pending) queue.TryEnqueue(() => handler());
            pending = false;
        }
    }

    private static void RequestActivation()
    {
        lock (Sync)
        {
            if (queue is null || activate is null) { pending = true; return; }
            queue.TryEnqueue(() => activate());
        }
    }

    [DllImport("ole32.dll")]
    private static extern int CoWaitForMultipleObjects(uint flags, uint timeout, uint count, nint[] handles, out uint index);
}
