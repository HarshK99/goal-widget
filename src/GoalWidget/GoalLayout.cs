using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace GoalWidget;

internal static class GoalLayout
{
    // Scale the resting card and its type together; editor controls stay full-sized.
    internal const double CardScale = 0.7;
    internal const double CardSize = 300 * CardScale;
    internal const double Width = 248 * CardScale;
    internal const double Height = 240 * CardScale;
    internal const double MinimumFontSize = 23 * CardScale;

    // Uses the same native text layout, font, wrapping and system text scale as the visible card.
    internal static double? Fit(string text)
    {
        var measure = new TextBlock
        {
            Style = (Style)Application.Current.Resources["GoalTypography"],
            Text = text
        };
        for (int referenceSize = 39; referenceSize >= 23; referenceSize--)
        {
            var size = referenceSize * CardScale;
            measure.FontSize = size;
            measure.LineHeight = size * 1.04;
            measure.Measure(new Size(Width, double.PositiveInfinity));
            if (measure.DesiredSize.Height <= Height && measure.DesiredSize.Width <= Width)
                return size;
        }
        return null;
    }
}
