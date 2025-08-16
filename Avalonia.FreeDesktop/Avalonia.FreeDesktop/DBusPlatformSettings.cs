using System;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Platform;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace Avalonia.FreeDesktop;

internal class DBusPlatformSettings : DefaultPlatformSettings
{
	private readonly OrgFreedesktopPortalSettings? _settings;

	private PlatformColorValues? _lastColorValues;

	private PlatformThemeVariant? _themeVariant;

	private Color? _accentColor;

	public DBusPlatformSettings()
	{
		if (DBusHelper.Connection != null)
		{
			_settings = new OrgFreedesktopPortalSettings(DBusHelper.Connection, "org.freedesktop.portal.Desktop", "/org/freedesktop/portal/desktop");
			_settings.WatchSettingChangedAsync(SettingsChangedHandler);
			TryGetInitialValuesAsync();
		}
	}

	public override PlatformColorValues GetColorValues()
	{
		return _lastColorValues ?? base.GetColorValues();
	}

	private async Task TryGetInitialValuesAsync()
	{
		_themeVariant = await TryGetThemeVariantAsync();
		_accentColor = await TryGetAccentColorAsync();
		_lastColorValues = BuildPlatformColorValues();
		if ((object)_lastColorValues != null)
		{
			OnColorValuesChanged(_lastColorValues);
		}
	}

	private async Task<PlatformThemeVariant?> TryGetThemeVariantAsync()
	{
		try
		{
			return ToColorScheme((((await _settings.ReadAsync("org.freedesktop.appearance", "color-scheme")).Value as DBusVariantItem).Value as DBusUInt32Item).Value);
		}
		catch (DBusException)
		{
			return null;
		}
	}

	private async Task<Color?> TryGetAccentColorAsync()
	{
		try
		{
			return ToAccentColor((((await _settings.ReadAsync("org.kde.kdeglobals.General", "AccentColor")).Value as DBusVariantItem).Value as DBusStringItem).Value);
		}
		catch (DBusException)
		{
			return null;
		}
	}

	private async void SettingsChangedHandler(Exception? exception, (string @namespace, string key, DBusVariantItem value) valueTuple)
	{
		if (exception == null && valueTuple.@namespace == "org.freedesktop.appearance" && valueTuple.key == "color-scheme")
		{
			DBusVariantItem item = valueTuple.value;
			if (item != null)
			{
				_themeVariant = ToColorScheme((item.Value as DBusUInt32Item).Value);
				_accentColor = await TryGetAccentColorAsync();
				_lastColorValues = BuildPlatformColorValues();
				OnColorValuesChanged(_lastColorValues);
			}
		}
	}

	private PlatformColorValues? BuildPlatformColorValues()
	{
		PlatformThemeVariant? themeVariant = _themeVariant;
		Color? accentColor;
		if (themeVariant.HasValue)
		{
			PlatformThemeVariant valueOrDefault = themeVariant.GetValueOrDefault();
			accentColor = _accentColor;
			if (accentColor.HasValue)
			{
				Color valueOrDefault2 = accentColor.GetValueOrDefault();
				return new PlatformColorValues
				{
					ThemeVariant = valueOrDefault,
					AccentColor1 = valueOrDefault2
				};
			}
		}
		themeVariant = _themeVariant;
		if (themeVariant.HasValue)
		{
			PlatformThemeVariant valueOrDefault3 = themeVariant.GetValueOrDefault();
			return new PlatformColorValues
			{
				ThemeVariant = valueOrDefault3
			};
		}
		accentColor = _accentColor;
		if (accentColor.HasValue)
		{
			Color valueOrDefault4 = accentColor.GetValueOrDefault();
			return new PlatformColorValues
			{
				AccentColor1 = valueOrDefault4
			};
		}
		return null;
	}

	private static PlatformThemeVariant ToColorScheme(uint value)
	{
		if (value != 1)
		{
			return PlatformThemeVariant.Light;
		}
		return PlatformThemeVariant.Dark;
	}

	private static Color ToAccentColor(string value)
	{
		string[] array = value.Split(',');
		return new Color(byte.MaxValue, byte.Parse(array[0]), byte.Parse(array[1]), byte.Parse(array[2]));
	}
}
