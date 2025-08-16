using System.Runtime.InteropServices;
using Avalonia.Input;

namespace AvaloniaEdit;

public static class ApplicationCommands
{
	private static readonly KeyModifiers PlatformCommandKey = GetPlatformCommandKey();

	public static RoutedCommand Delete { get; } = new RoutedCommand("Delete", new KeyGesture(Key.Delete));

	public static RoutedCommand Copy { get; } = new RoutedCommand("Copy", new KeyGesture(Key.C, PlatformCommandKey));

	public static RoutedCommand Cut { get; } = new RoutedCommand("Cut", new KeyGesture(Key.X, PlatformCommandKey));

	public static RoutedCommand Paste { get; } = new RoutedCommand("Paste", new KeyGesture(Key.V, PlatformCommandKey));

	public static RoutedCommand SelectAll { get; } = new RoutedCommand("SelectAll", new KeyGesture(Key.A, PlatformCommandKey));

	public static RoutedCommand Undo { get; } = new RoutedCommand("Undo", new KeyGesture(Key.Z, PlatformCommandKey));

	public static RoutedCommand Redo { get; } = new RoutedCommand("Redo", new KeyGesture(Key.Y, PlatformCommandKey));

	public static RoutedCommand Find { get; } = new RoutedCommand("Find", new KeyGesture(Key.F, PlatformCommandKey));

	public static RoutedCommand Replace { get; } = new RoutedCommand("Replace", GetReplaceKeyGesture());

	private static KeyModifiers GetPlatformCommandKey()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return KeyModifiers.Meta;
		}
		return KeyModifiers.Control;
	}

	private static KeyGesture GetReplaceKeyGesture()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return new KeyGesture(Key.F, KeyModifiers.Alt | KeyModifiers.Meta);
		}
		return new KeyGesture(Key.H, PlatformCommandKey);
	}
}
