using System;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Win32.Interop;
using Avalonia.Win32.WinRT;

namespace Avalonia.Win32;

internal class Win32PlatformSettings : DefaultPlatformSettings
{
	private PlatformColorValues? _lastColorValues;

	public override Size GetTapSize(PointerType type)
	{
		if (type == PointerType.Touch)
		{
			return new Size(10.0, 10.0);
		}
		return new Size(UnmanagedMethods.GetSystemMetrics(UnmanagedMethods.SystemMetric.SM_CXDRAG), UnmanagedMethods.GetSystemMetrics(UnmanagedMethods.SystemMetric.SM_CYDRAG));
	}

	public override Size GetDoubleTapSize(PointerType type)
	{
		if (type == PointerType.Touch)
		{
			return new Size(16.0, 16.0);
		}
		return new Size(UnmanagedMethods.GetSystemMetrics(UnmanagedMethods.SystemMetric.SM_CXDOUBLECLK), UnmanagedMethods.GetSystemMetrics(UnmanagedMethods.SystemMetric.SM_CYDOUBLECLK));
	}

	public override TimeSpan GetDoubleTapTime(PointerType type)
	{
		return TimeSpan.FromMilliseconds(UnmanagedMethods.GetDoubleClickTime());
	}

	public override PlatformColorValues GetColorValues()
	{
		if (Win32Platform.WindowsVersion.Major < 10)
		{
			return base.GetColorValues();
		}
		IUISettings3 iUISettings = NativeWinRTMethods.CreateInstance<IUISettings3>("Windows.UI.ViewManagement.UISettings");
		Color accentColor = iUISettings.GetColorValue(UIColorType.Accent).ToAvalonia();
		IAccessibilitySettings accessibilitySettings = NativeWinRTMethods.CreateInstance<IAccessibilitySettings>("Windows.UI.ViewManagement.AccessibilitySettings");
		if (accessibilitySettings.HighContrast == 1)
		{
			using (HStringInterop hStringInterop = new HStringInterop(accessibilitySettings.HighContrastScheme))
			{
				PlatformColorValues platformColorValues = new PlatformColorValues();
				string? value = hStringInterop.Value;
				platformColorValues.ThemeVariant = ((value == null || !value.Contains("White")) ? PlatformThemeVariant.Dark : PlatformThemeVariant.Light);
				platformColorValues.ContrastPreference = ColorContrastPreference.High;
				platformColorValues.AccentColor1 = accentColor;
				PlatformColorValues result = platformColorValues;
				_lastColorValues = platformColorValues;
				return result;
			}
		}
		Color color = iUISettings.GetColorValue(UIColorType.Background).ToAvalonia();
		return _lastColorValues = new PlatformColorValues
		{
			ThemeVariant = ((color.R + color.G + color.B < 765 - color.R - color.G - color.B) ? PlatformThemeVariant.Dark : PlatformThemeVariant.Light),
			ContrastPreference = ColorContrastPreference.NoPreference,
			AccentColor1 = accentColor
		};
	}

	internal void OnColorValuesChanged()
	{
		PlatformColorValues? lastColorValues = _lastColorValues;
		PlatformColorValues colorValues = GetColorValues();
		if (lastColorValues != colorValues)
		{
			OnColorValuesChanged(colorValues);
		}
	}
}
