namespace Avalonia;

public class MacOSPlatformOptions
{
	public bool ShowInDock { get; set; } = true;

	public bool DisableDefaultApplicationMenuItems { get; set; }

	public bool DisableNativeMenus { get; set; }

	public bool DisableSetProcessName { get; set; }

	public bool DisableAvaloniaAppDelegate { get; set; }
}
