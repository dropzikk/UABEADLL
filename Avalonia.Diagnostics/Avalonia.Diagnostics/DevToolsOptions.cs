using Avalonia.Input;
using Avalonia.Styling;

namespace Avalonia.Diagnostics;

public class DevToolsOptions
{
	public KeyGesture Gesture { get; set; } = new KeyGesture(Key.F12);

	public bool ShowAsChildWindow { get; set; } = true;

	public Size Size { get; set; } = new Size(1280.0, 720.0);

	public int? StartupScreenIndex { get; set; }

	public bool ShowImplementedInterfaces { get; set; } = true;

	public IScreenshotHandler ScreenshotHandler { get; set; } = Conventions.DefaultScreenshotHandler;

	public ThemeVariant? ThemeVariant { get; set; }
}
