using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace GoalWidget;

/// <summary>An ordinary window: unlike the widget, the editor comes to the front and takes the keyboard.</summary>
public partial class EditGoalWindow : Window
{
    private readonly GoalStore store;
    internal bool Saving { get; private set; }

    public EditGoalWindow(GoalStore store)
    {
        this.store = store;
        InitializeComponent();
        Draft.Text = store.Current.GoalText;
        if (store.WriteBlock is not null) ShowError(store.WriteBlock);
        Loaded += (_, _) =>
        {
            Draft.Focus();
            Draft.SelectAll();
        };
    }

    private void Draft_Changed(object sender, TextChangedEventArgs e)
    {
        var text = GoalTextRules.Normalize(Draft.Text);
        Count.Text = $"{new StringInfo(text).LengthInTextElements}/100 characters · {text.Split('\n').Length}/5 lines";
        // Typing clears the last complaint, but not the reason saving is disabled.
        if (store.WriteBlock is null) Error.Visibility = Visibility.Collapsed;
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        var text = GoalTextRules.Normalize(Draft.Text);
        var error = GoalTextRules.Validate(text);
        if (error is null && GoalLayout.Fit(text) is null)
            error = "This goal will not fit on the card. Shorten it or use fewer lines.";
        if (error is not null) { ShowError(error); return; }

        SetSaving(true);
        try
        {
            await store.SaveGoalAsync(text); // Saved to disk before the editor reports success by closing.
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            SetSaving(false);
            ShowError("Your goal was not saved. Your draft is still here; try Save again. " + ex.Message);
            return;
        }
        SetSaving(false);
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

    private void SetSaving(bool saving)
    {
        Saving = saving;
        Save.Content = saving ? "Saving…" : "Save";
        Draft.IsEnabled = Save.IsEnabled = Cancel.IsEnabled = !saving;
    }

    private void ShowError(string message)
    {
        Error.Text = message;
        Error.Visibility = Visibility.Visible;
        Draft.Focus();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel = Saving;
        base.OnClosing(e);
    }
}
