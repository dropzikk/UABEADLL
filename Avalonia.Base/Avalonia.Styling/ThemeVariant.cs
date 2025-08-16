using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Platform;

namespace Avalonia.Styling;

[TypeConverter(typeof(ThemeVariantTypeConverter))]
public sealed record ThemeVariant
{
	public object Key { get; }

	public ThemeVariant? InheritVariant { get; }

	public static ThemeVariant Default { get; } = new ThemeVariant("Default");

	public static ThemeVariant Light { get; } = new ThemeVariant("Light");

	public static ThemeVariant Dark { get; } = new ThemeVariant("Dark");

	internal static readonly StyledProperty<ThemeVariant> ActualThemeVariantProperty = AvaloniaProperty.Register<StyledElement, ThemeVariant>("ActualThemeVariant", null, inherits: true);

	internal static readonly StyledProperty<ThemeVariant?> RequestedThemeVariantProperty = AvaloniaProperty.Register<StyledElement, ThemeVariant>("RequestedThemeVariant", Default);

	public ThemeVariant(object key, ThemeVariant? inheritVariant)
	{
		Key = key ?? throw new ArgumentNullException("key");
		InheritVariant = inheritVariant;
		if (inheritVariant == Default)
		{
			throw new ArgumentException("Inheriting default theme variant is not supported.", "inheritVariant");
		}
	}

	private ThemeVariant(object key)
	{
		Key = key;
	}

	public override string ToString()
	{
		return Key.ToString() ?? $"ThemeVariant {{ Key = {Key} }}";
	}

	public override int GetHashCode()
	{
		return Key.GetHashCode();
	}

	public bool Equals(ThemeVariant? other)
	{
		return Key == other?.Key;
	}

	public static explicit operator ThemeVariant(PlatformThemeVariant themeVariant)
	{
		return themeVariant switch
		{
			PlatformThemeVariant.Light => Light, 
			PlatformThemeVariant.Dark => Dark, 
			_ => throw new ArgumentOutOfRangeException("themeVariant", themeVariant, null), 
		};
	}

	public static explicit operator PlatformThemeVariant?(ThemeVariant themeVariant)
	{
		if (themeVariant == Light)
		{
			return PlatformThemeVariant.Light;
		}
		if (themeVariant == Dark)
		{
			return PlatformThemeVariant.Dark;
		}
		ThemeVariant inheritVariant = themeVariant.InheritVariant;
		if ((object)inheritVariant != null)
		{
			return (PlatformThemeVariant?)inheritVariant;
		}
		return null;
	}

	[CompilerGenerated]
	private ThemeVariant(ThemeVariant original)
	{
		Key = original.Key;
		InheritVariant = original.InheritVariant;
	}
}
