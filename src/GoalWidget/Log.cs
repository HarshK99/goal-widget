using System;
using System.IO;

namespace GoalWidget;

/// <summary>A small always-on log, so a failure at sign-in leaves something to read.</summary>
internal static class Log
{
    private static readonly string Path = System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GoalWidget", "log.txt");

    internal static void Write(string message)
    {
        try
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path)!);
            if (new FileInfo(Path) is { Exists: true, Length: > 262144 }) File.Delete(Path);
            File.AppendAllText(Path, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}{Environment.NewLine}");
        }
        catch (IOException) { /* Logging must never take the widget down. */ }
        catch (UnauthorizedAccessException) { }
    }
}
