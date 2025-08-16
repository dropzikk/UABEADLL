using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using Avalonia.Utilities;

namespace Avalonia.Controls.Converters;

public class MenuScrollingVisibilityConverter : IMultiValueConverter
{
	public static readonly MenuScrollingVisibilityConverter Instance = new MenuScrollingVisibilityConverter();

	public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
	{
		if (parameter == null || values.Count != 4 || !(values[0] is ScrollBarVisibility scrollBarVisibility) || !(values[1] is double num) || !(values[2] is double num2) || !(values[3] is double num3))
		{
			return AvaloniaProperty.UnsetValue;
		}
		if (scrollBarVisibility == ScrollBarVisibility.Auto)
		{
			if (MathUtilities.AreClose(num2, num3))
			{
				return false;
			}
			double value;
			if (parameter is double num4)
			{
				value = num4;
			}
			else
			{
				if (!(parameter is string s))
				{
					return AvaloniaProperty.UnsetValue;
				}
				value = double.Parse(s, NumberFormatInfo.InvariantInfo);
			}
			if (MathUtilities.AreClose(MathUtilities.Clamp(num * 100.0 / (num2 - num3), 0.0, 100.0), value))
			{
				return false;
			}
			return true;
		}
		return false;
	}
}
