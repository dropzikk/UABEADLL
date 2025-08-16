using System;
using Avalonia.Media;
using Avalonia.Native.Interop;
using Avalonia.Platform;
using MicroCom.Runtime;

namespace Avalonia.Native;

internal class NativePlatformSettings : DefaultPlatformSettings
{
	private class ColorsChangeCallback : NativeCallbackBase, IAvnActionCallback, IUnknown, IDisposable
	{
		private readonly NativePlatformSettings _settings;

		public ColorsChangeCallback(NativePlatformSettings settings)
		{
			_settings = settings;
		}

		public void Run()
		{
			_settings.OnColorValuesChanged();
		}
	}

	private readonly IAvnPlatformSettings _platformSettings;

	private PlatformColorValues _lastColorValues;

	public NativePlatformSettings(IAvnPlatformSettings platformSettings)
	{
		_platformSettings = platformSettings;
		platformSettings.RegisterColorsChange(new ColorsChangeCallback(this));
	}

	public unsafe override PlatformColorValues GetColorValues()
	{
		object obj = _platformSettings.PlatformTheme switch
		{
			AvnPlatformThemeVariant.Dark => (PlatformThemeVariant.Dark, ColorContrastPreference.NoPreference), 
			AvnPlatformThemeVariant.Light => (PlatformThemeVariant.Light, ColorContrastPreference.NoPreference), 
			AvnPlatformThemeVariant.HighContrastDark => (PlatformThemeVariant.Dark, ColorContrastPreference.High), 
			AvnPlatformThemeVariant.HighContrastLight => (PlatformThemeVariant.Light, ColorContrastPreference.High), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		PlatformThemeVariant item = ((ValueTuple<PlatformThemeVariant, ColorContrastPreference>*)(&obj))->Item1;
		ColorContrastPreference item2 = ((ValueTuple<PlatformThemeVariant, ColorContrastPreference>*)(&obj))->Item2;
		uint accentColor = _platformSettings.AccentColor;
		if (accentColor != 0)
		{
			_lastColorValues = new PlatformColorValues
			{
				ThemeVariant = item,
				ContrastPreference = item2,
				AccentColor1 = Color.FromUInt32(accentColor)
			};
		}
		else
		{
			_lastColorValues = new PlatformColorValues
			{
				ThemeVariant = item,
				ContrastPreference = item2
			};
		}
		return _lastColorValues;
	}

	public void OnColorValuesChanged()
	{
		PlatformColorValues lastColorValues = _lastColorValues;
		PlatformColorValues colorValues = GetColorValues();
		if (lastColorValues != colorValues)
		{
			OnColorValuesChanged(colorValues);
		}
	}
}
