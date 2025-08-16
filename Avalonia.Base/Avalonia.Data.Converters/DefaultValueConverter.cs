using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Input;
using Avalonia.Utilities;

namespace Avalonia.Data.Converters;

[RequiresUnreferencedCode("Conversion methods are required for type conversion, including op_Implicit, op_Explicit, Parse and TypeConverter.")]
public class DefaultValueConverter : IValueConverter
{
	public static readonly DefaultValueConverter Instance = new DefaultValueConverter();

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		if (typeof(ICommand).IsAssignableFrom(targetType) && value is Delegate @delegate && @delegate.Method.GetParameters().Length <= 1)
		{
			if (!@delegate.Method.IsPrivate)
			{
				return new MethodToCommandConverter(@delegate);
			}
			return new BindingNotification(new InvalidCastException("You can't bind to private methods!"), BindingErrorType.Error);
		}
		if (TypeUtilities.TryConvert(targetType, value, culture, out object result))
		{
			return result;
		}
		string message = ((!TypeUtilities.IsNumeric(targetType)) ? $"Could not convert '{value}' to '{targetType.Name}'." : $"'{value}' is not a valid number.");
		return new BindingNotification(new InvalidCastException(message), BindingErrorType.Error);
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return Convert(value, targetType, parameter, culture);
	}
}
