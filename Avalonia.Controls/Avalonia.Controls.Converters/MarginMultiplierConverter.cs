using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Avalonia.Controls.Converters;

public class MarginMultiplierConverter : IValueConverter
{
	public double Indent { get; set; }

	public bool Left { get; set; }

	public bool Top { get; set; }

	public bool Right { get; set; }

	public bool Bottom { get; set; }

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is int num)
		{
			return new Thickness(Left ? (Indent * (double)num) : 0.0, Top ? (Indent * (double)num) : 0.0, Right ? (Indent * (double)num) : 0.0, Bottom ? (Indent * (double)num) : 0.0);
		}
		if (value is Thickness thickness)
		{
			return new Thickness(Left ? (Indent * thickness.Left) : 0.0, Top ? (Indent * thickness.Top) : 0.0, Right ? (Indent * thickness.Right) : 0.0, Bottom ? (Indent * thickness.Bottom) : 0.0);
		}
		return new Thickness(0.0);
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
