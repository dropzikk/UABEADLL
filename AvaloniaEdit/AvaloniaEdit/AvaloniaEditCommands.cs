using Avalonia.Input;

namespace AvaloniaEdit;

public static class AvaloniaEditCommands
{
	public static RoutedCommand ToggleOverstrike { get; } = new RoutedCommand("ToggleOverstrike", new KeyGesture(Key.Insert));

	public static RoutedCommand DeleteLine { get; } = new RoutedCommand("DeleteLine", new KeyGesture(Key.D, KeyModifiers.Control));

	public static RoutedCommand RemoveLeadingWhitespace { get; } = new RoutedCommand("RemoveLeadingWhitespace");

	public static RoutedCommand RemoveTrailingWhitespace { get; } = new RoutedCommand("RemoveTrailingWhitespace");

	public static RoutedCommand ConvertToUppercase { get; } = new RoutedCommand("ConvertToUppercase");

	public static RoutedCommand ConvertToLowercase { get; } = new RoutedCommand("ConvertToLowercase");

	public static RoutedCommand ConvertToTitleCase { get; } = new RoutedCommand("ConvertToTitleCase");

	public static RoutedCommand InvertCase { get; } = new RoutedCommand("InvertCase");

	public static RoutedCommand ConvertTabsToSpaces { get; } = new RoutedCommand("ConvertTabsToSpaces");

	public static RoutedCommand ConvertSpacesToTabs { get; } = new RoutedCommand("ConvertSpacesToTabs");

	public static RoutedCommand ConvertLeadingTabsToSpaces { get; } = new RoutedCommand("ConvertLeadingTabsToSpaces");

	public static RoutedCommand ConvertLeadingSpacesToTabs { get; } = new RoutedCommand("ConvertLeadingSpacesToTabs");

	public static RoutedCommand IndentSelection { get; } = new RoutedCommand("IndentSelection", new KeyGesture(Key.I, KeyModifiers.Control));
}
