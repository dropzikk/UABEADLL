using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Avalonia.Controls.Converters;

public class CornerRadiusToDoubleConverter : IValueConverter
{
	public Corners Corner { get; set; }

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (!(value is CornerRadius cornerRadius))
		{
			return AvaloniaProperty.UnsetValue;
		}
		return Corner switch
		{
			Corners.TopLeft => cornerRadius.TopLeft, 
			Corners.TopRight => cornerRadius.TopRight, 
			Corners.BottomRight => cornerRadius.BottomRight, 
			Corners.BottomLeft => cornerRadius.BottomLeft, 
			_ => 0.0, 
		};
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
