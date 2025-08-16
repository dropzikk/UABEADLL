using Avalonia.Controls;
using Avalonia.Data;

namespace Avalonia.Diagnostics.Behaviors;

internal static class ColumnDefinition
{
	private static readonly GridLength ZeroWidth = new GridLength(0.0, GridUnitType.Pixel);

	private static readonly AttachedProperty<GridLength?> LastWidthProperty = AvaloniaProperty.RegisterAttached<Avalonia.Controls.ColumnDefinition, GridLength?>("LastWidth", typeof(ColumnDefinition), null);

	public static readonly AttachedProperty<bool> IsVisibleProperty = AvaloniaProperty.RegisterAttached<Avalonia.Controls.ColumnDefinition, bool>("IsVisible", typeof(ColumnDefinition), defaultValue: true, inherits: false, BindingMode.OneWay, null, delegate(AvaloniaObject element, bool visibility)
	{
		GridLength? value = element.GetValue(LastWidthProperty);
		if (visibility && value.HasValue)
		{
			element.SetValue(Avalonia.Controls.ColumnDefinition.WidthProperty, value);
		}
		else if (!visibility)
		{
			element.SetValue(LastWidthProperty, element.GetValue(Avalonia.Controls.ColumnDefinition.WidthProperty));
			element.SetValue(Avalonia.Controls.ColumnDefinition.WidthProperty, ZeroWidth);
		}
		return visibility;
	});

	public static bool GetIsVisible(Avalonia.Controls.ColumnDefinition columnDefinition)
	{
		return columnDefinition.GetValue(IsVisibleProperty);
	}

	public static void SetIsVisible(Avalonia.Controls.ColumnDefinition columnDefinition, bool visibility)
	{
		columnDefinition.SetValue(IsVisibleProperty, visibility);
	}
}
