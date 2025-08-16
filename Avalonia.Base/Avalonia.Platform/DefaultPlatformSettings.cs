using System;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[PrivateApi]
public class DefaultPlatformSettings : IPlatformSettings
{
	public virtual TimeSpan HoldWaitDuration => TimeSpan.FromMilliseconds(300.0);

	public PlatformHotkeyConfiguration HotkeyConfiguration => AvaloniaLocator.Current.GetRequiredService<PlatformHotkeyConfiguration>();

	public virtual event EventHandler<PlatformColorValues>? ColorValuesChanged;

	public virtual Size GetTapSize(PointerType type)
	{
		if (type == PointerType.Touch)
		{
			return new Size(10.0, 10.0);
		}
		return new Size(4.0, 4.0);
	}

	public virtual Size GetDoubleTapSize(PointerType type)
	{
		if (type == PointerType.Touch)
		{
			return new Size(16.0, 16.0);
		}
		return new Size(4.0, 4.0);
	}

	public virtual TimeSpan GetDoubleTapTime(PointerType type)
	{
		return TimeSpan.FromMilliseconds(500.0);
	}

	public virtual PlatformColorValues GetColorValues()
	{
		return new PlatformColorValues
		{
			ThemeVariant = PlatformThemeVariant.Light
		};
	}

	protected void OnColorValuesChanged(PlatformColorValues colorValues)
	{
		this.ColorValuesChanged?.Invoke(this, colorValues);
	}
}
