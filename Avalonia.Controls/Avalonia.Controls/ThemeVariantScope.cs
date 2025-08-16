using Avalonia.Styling;

namespace Avalonia.Controls;

public class ThemeVariantScope : Decorator
{
	public static readonly StyledProperty<ThemeVariant> ActualThemeVariantProperty = ThemeVariant.ActualThemeVariantProperty.AddOwner<ThemeVariantScope>();

	public static readonly StyledProperty<ThemeVariant?> RequestedThemeVariantProperty = ThemeVariant.RequestedThemeVariantProperty.AddOwner<ThemeVariantScope>();

	public ThemeVariant? RequestedThemeVariant
	{
		get
		{
			return GetValue(RequestedThemeVariantProperty);
		}
		set
		{
			SetValue(RequestedThemeVariantProperty, value);
		}
	}
}
