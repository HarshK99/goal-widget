using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GoalWidget;

// This is the sole owner of state. Each mutation starts from the latest committed state.
public sealed class GoalStore
{
    private static readonly JsonSerializerOptions Json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
    private readonly SemaphoreSlim writes = new(1, 1);
    private readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GoalWidget", "state.json");
    private string Backup => path + ".bak";
    public GoalState Current { get; private set; } = new();
    public string? Notice { get; private set; }
    public string? WriteBlock { get; private set; }

    private enum FileKind { Missing, Valid, Invalid, Unreadable, Future }
    private sealed record ReadResult(FileKind Kind, GoalState? State = null);

    private static ReadResult Read(string file)
    {
        try
        {
            using var stream = File.OpenRead(file);
            if (stream.Length > 65536) return new(FileKind.Invalid);
            using var document = JsonDocument.Parse(stream);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object ||
                !root.TryGetProperty("schemaVersion", out var version) || !version.TryGetInt32(out var schema))
                return new(FileKind.Invalid);
            if (schema > 1) return new(FileKind.Future);
            if (schema != 1 || !root.TryGetProperty("goalText", out var text) || text.ValueKind != JsonValueKind.String)
                return new(FileKind.Invalid);
            var state = root.Deserialize<GoalState>(Json);
            if (state is null || state.GoalText is null) return new(FileKind.Invalid);
            var normalized = GoalTextRules.Normalize(state.GoalText);
            if (GoalTextRules.Validate(normalized) is not null) return new(FileKind.Invalid);
            // Bad placement must not discard a recoverable goal.
            var placement = state.Placement;
            if (placement is not null && (placement.Dpi is < 48 or > 960 || string.IsNullOrEmpty(placement.MonitorId)))
                placement = null;
            return new(FileKind.Valid, state with { GoalText = normalized, Placement = placement });
        }
        catch (FileNotFoundException) { return new(FileKind.Missing); }
        catch (DirectoryNotFoundException) { return new(FileKind.Missing); }
        catch (JsonException) { return new(FileKind.Invalid); }
        catch (IOException) { return new(FileKind.Unreadable); }
        catch (UnauthorizedAccessException) { return new(FileKind.Unreadable); }
    }

    public void Load()
    {
        var primary = Read(path);
        var backup = Read(Backup);
        Current = primary.State ?? backup.State ?? new GoalState();
        if (primary.Kind == FileKind.Future || backup.Kind == FileKind.Future)
            WriteBlock = "A newer app saved this file. Saving is disabled to protect it. Open it with the newer app.";
        else if (primary.Kind == FileKind.Unreadable || backup.Kind == FileKind.Unreadable)
            WriteBlock = "The saved file could not be read. Saving is disabled to protect it. Check folder access, then reopen the app.";
        if (WriteBlock is not null) Notice = WriteBlock;
        else if (primary.Kind != FileKind.Valid && backup.State is not null)
            Notice = "Your goal was recovered from its backup. The original file will be kept.";
        else if (primary.Kind == FileKind.Invalid || backup.Kind == FileKind.Invalid)
            Notice = primary.State is null
                ? "The saved goal could not be read. Showing the starting goal; the original files will be kept."
                : "The backup could not be read. Your saved goal is still available.";
    }

    public Task SaveGoalAsync(string text) => ChangeAsync(state => state with { GoalText = text });
    public Task SavePlacementAsync(SavedPlacement placement) => ChangeAsync(state => state with { Placement = placement });

    private async Task ChangeAsync(Func<GoalState, GoalState> change)
    {
        await writes.WaitAsync();
        string? temporary = null;
        try
        {
            if (WriteBlock is not null) throw new IOException(WriteBlock);
            var next = change(Current);
            var error = GoalTextRules.Validate(next.GoalText);
            if (error is not null) throw new IOException(error);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            var primary = Read(path);
            var backup = Read(Backup);
            if (primary.Kind is FileKind.Future or FileKind.Unreadable || backup.Kind is FileKind.Future or FileKind.Unreadable)
                throw new IOException("The saved files changed or cannot be read. Reopen the app before saving again.");

            temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
            {
                await JsonSerializer.SerializeAsync(stream, next, Json);
                await stream.FlushAsync();
                stream.Flush(flushToDisk: true);
            }
            // Preserve damaged evidence before replacing either file. Never rotate corruption into the backup.
            if (primary.Kind == FileKind.Invalid) Preserve(path);
            if (backup.Kind == FileKind.Invalid) Preserve(Backup);
            if (primary.Kind == FileKind.Valid)
                File.Replace(temporary, path, Backup);
            else
            {
                if (backup.Kind != FileKind.Valid) File.Copy(temporary, Backup, overwrite: true);
                if (primary.Kind == FileKind.Missing) File.Move(temporary, path);
                else File.Replace(temporary, path, null);
            }
            temporary = null;
            Current = next; // Only publish after the durable replacement succeeds.
        }
        finally
        {
            if (temporary is not null)
            {
                try { File.Delete(temporary); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
            writes.Release();
        }
    }

    private static void Preserve(string file) => File.Copy(file, file + ".recovered-" + Guid.NewGuid().ToString("N"));
}
