using System;
using System.Globalization;
using Avalonia.Utilities;

namespace Avalonia.Data.Converters;

public class FuncValueConverter<TIn, TOut> : IValueConverter
{
	private readonly Func<TIn?, TOut> _convert;

	public FuncValueConverter(Func<TIn?, TOut> convert)
	{
		_convert = convert;
	}

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (TypeUtilities.CanCast<TIn>(value))
		{
			return _convert((TIn)value);
		}
		return AvaloniaProperty.UnsetValue;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
