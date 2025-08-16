using Avalonia.Input;

namespace AvaloniaEdit.Search;

public static class SearchCommands
{
	public static readonly RoutedCommand FindNext = new RoutedCommand("FindNext", new KeyGesture(Key.F3));

	public static readonly RoutedCommand FindPrevious = new RoutedCommand("FindPrevious", new KeyGesture(Key.F3, KeyModifiers.Shift));

	public static readonly RoutedCommand CloseSearchPanel = new RoutedCommand("CloseSearchPanel", new KeyGesture(Key.Escape));

	public static readonly RoutedCommand ReplaceNext = new RoutedCommand("ReplaceNext", new KeyGesture(Key.R, KeyModifiers.Alt));

	public static readonly RoutedCommand ReplaceAll = new RoutedCommand("ReplaceAll", new KeyGesture(Key.A, KeyModifiers.Alt));
}
