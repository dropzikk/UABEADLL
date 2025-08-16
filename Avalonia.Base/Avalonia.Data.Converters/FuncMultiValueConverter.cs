using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Avalonia.Data.Converters;

public class FuncMultiValueConverter<TIn, TOut> : IMultiValueConverter
{
	private readonly Func<IEnumerable<TIn?>, TOut> _convert;

	public FuncMultiValueConverter(Func<IEnumerable<TIn?>, TOut> convert)
	{
		_convert = convert;
	}

	public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
	{
		List<TIn> list2 = OfTypeWithDefaultSupport(values).ToList();
		if (list2.Count == values.Count)
		{
			return _convert(list2);
		}
		return AvaloniaProperty.UnsetValue;
		static IEnumerable<TIn?> OfTypeWithDefaultSupport(IList<object?> list)
		{
			foreach (object item in list)
			{
				if (item is TIn)
				{
					yield return (TIn)item;
				}
				else if (object.Equals(item, default(TIn)))
				{
					yield return default(TIn);
				}
			}
		}
	}
}
