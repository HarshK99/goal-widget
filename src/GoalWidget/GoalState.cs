using System.Globalization;
using System.Linq;

namespace GoalWidget;

public sealed record GoalState
{
    public const string DefaultGoal = "Build\nsomething\npeople want.";
    public int SchemaVersion { get; init; } = 1;
    public string GoalText { get; init; } = DefaultGoal;
    public SavedPlacement? Placement { get; init; }
}

/// <summary>The card's top-left corner in physical pixels, and the monitor it was on.</summary>
public sealed record SavedPlacement(int X, int Y, string MonitorId, uint Dpi);

public static class GoalTextRules
{
    public static string Normalize(string value) => value.Replace("\r\n", "\n").Replace('\r', '\n');

    public static string? Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return "Write a goal before saving.";
        if (new StringInfo(value).LengthInTextElements > 100)
            return "Keep your goal to 100 characters or fewer.";
        if (value.Split('\n').Length > 5) return "Use no more than five lines.";
        if (value.Any(c => char.IsControl(c) && c != '\n'))
            return "Remove tabs and other hidden control characters.";
        return null;
    }
}
