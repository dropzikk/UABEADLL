using System;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;

namespace Avalonia.Themes.Fluent.Accents;

internal class SystemAccentColors : IResourceProvider, IResourceNode
{
	public const string AccentKey = "SystemAccentColor";

	public const string AccentDark1Key = "SystemAccentColorDark1";

	public const string AccentDark2Key = "SystemAccentColorDark2";

	public const string AccentDark3Key = "SystemAccentColorDark3";

	public const string AccentLight1Key = "SystemAccentColorLight1";

	public const string AccentLight2Key = "SystemAccentColorLight2";

	public const string AccentLight3Key = "SystemAccentColorLight3";

	private static readonly Color s_defaultSystemAccentColor = Color.FromRgb(0, 120, 215);

	private bool _invalidateColors = true;

	private Color _systemAccentColor;

	private Color _systemAccentColorDark1;

	private Color _systemAccentColorDark2;

	private Color _systemAccentColorDark3;

	private Color _systemAccentColorLight1;

	private Color _systemAccentColorLight2;

	private Color _systemAccentColorLight3;

	public bool HasResources => true;

	public IResourceHost? Owner { get; private set; }

	public event EventHandler? OwnerChanged;

	public bool TryGetResource(object key, ThemeVariant? theme, out object? value)
	{
		if (key is string text)
		{
			if (text.Equals("SystemAccentColor", StringComparison.InvariantCulture))
			{
				EnsureColors();
				value = _systemAccentColor;
				return true;
			}
			if (text.Equals("SystemAccentColorDark1", StringComparison.InvariantCulture))
			{
				EnsureColors();
				value = _systemAccentColorDark1;
				return true;
			}
			if (text.Equals("SystemAccentColorDark2", StringComparison.InvariantCulture))
			{
				EnsureColors();
				value = _systemAccentColorDark2;
				return true;
			}
			if (text.Equals("SystemAccentColorDark3", StringComparison.InvariantCulture))
			{
				EnsureColors();
				value = _systemAccentColorDark3;
				return true;
			}
			if (text.Equals("SystemAccentColorLight1", StringComparison.InvariantCulture))
			{
				EnsureColors();
				value = _systemAccentColorLight1;
				return true;
			}
			if (text.Equals("SystemAccentColorLight2", StringComparison.InvariantCulture))
			{
				EnsureColors();
				value = _systemAccentColorLight2;
				return true;
			}
			if (text.Equals("SystemAccentColorLight3", StringComparison.InvariantCulture))
			{
				EnsureColors();
				value = _systemAccentColorLight3;
				return true;
			}
		}
		value = null;
		return false;
	}

	public void AddOwner(IResourceHost owner)
	{
		if (Owner != owner)
		{
			Owner = owner;
			this.OwnerChanged?.Invoke(this, EventArgs.Empty);
			IPlatformSettings fromOwner = GetFromOwner(owner);
			if (fromOwner != null)
			{
				fromOwner.ColorValuesChanged += PlatformSettingsOnColorValuesChanged;
			}
			_invalidateColors = true;
		}
	}

	public void RemoveOwner(IResourceHost owner)
	{
		if (Owner == owner)
		{
			Owner = null;
			this.OwnerChanged?.Invoke(this, EventArgs.Empty);
			IPlatformSettings fromOwner = GetFromOwner(owner);
			if (fromOwner != null)
			{
				fromOwner.ColorValuesChanged -= PlatformSettingsOnColorValuesChanged;
			}
			_invalidateColors = true;
		}
	}

	private void EnsureColors()
	{
		if (_invalidateColors)
		{
			_invalidateColors = false;
			_systemAccentColor = GetFromOwner(Owner)?.GetColorValues().AccentColor1 ?? s_defaultSystemAccentColor;
			(_systemAccentColorDark1, _systemAccentColorDark2, _systemAccentColorDark3, _systemAccentColorLight1, _systemAccentColorLight2, _systemAccentColorLight3) = CalculateAccentShades(_systemAccentColor);
		}
	}

	private static IPlatformSettings? GetFromOwner(IResourceHost? owner)
	{
		if (!(owner is Application application))
		{
			if (owner is Visual visual)
			{
				return TopLevel.GetTopLevel(visual)?.PlatformSettings;
			}
			return null;
		}
		return application.PlatformSettings;
	}

	public static (Color d1, Color d2, Color d3, Color l1, Color l2, Color l3) CalculateAccentShades(Color accentColor)
	{
		HslColor hslColor = accentColor.ToHsl();
		return (d1: new HslColor(hslColor.A, hslColor.H, hslColor.S, hslColor.L - 19.0 / 170.0).ToRgb(), d2: new HslColor(hslColor.A, hslColor.H, hslColor.S, hslColor.L - 49.0 / 255.0).ToRgb(), d3: new HslColor(hslColor.A, hslColor.H, hslColor.S, hslColor.L - 149.0 / 510.0).ToRgb(), l1: new HslColor(hslColor.A, hslColor.H, hslColor.S, hslColor.L + 13.0 / 85.0).ToRgb(), l2: new HslColor(hslColor.A, hslColor.H, hslColor.S, hslColor.L + 14.0 / 51.0).ToRgb(), l3: new HslColor(hslColor.A, hslColor.H, hslColor.S, hslColor.L + 103.0 / 255.0).ToRgb());
	}

	private void PlatformSettingsOnColorValuesChanged(object? sender, PlatformColorValues e)
	{
		_invalidateColors = true;
		Owner?.NotifyHostedResourcesChanged(ResourcesChangedEventArgs.Empty);
	}
}
