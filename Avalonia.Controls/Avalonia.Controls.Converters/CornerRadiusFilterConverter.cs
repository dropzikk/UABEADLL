using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Avalonia.Controls.Converters;

public class CornerRadiusFilterConverter : IValueConverter
{
	public Corners Filter { get; set; }

	public double Scale { get; set; } = 1.0;

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (!(value is CornerRadius cornerRadius))
		{
			return value;
		}
		return new CornerRadius(Filter.HasAllFlags(Corners.TopLeft) ? (cornerRadius.TopLeft * Scale) : 0.0, Filter.HasAllFlags(Corners.TopRight) ? (cornerRadius.TopRight * Scale) : 0.0, Filter.HasAllFlags(Corners.BottomRight) ? (cornerRadius.BottomRight * Scale) : 0.0, Filter.HasAllFlags(Corners.BottomLeft) ? (cornerRadius.BottomLeft * Scale) : 0.0);
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
