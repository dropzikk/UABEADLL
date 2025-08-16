using System;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia.Controls;

internal static class Extensions
{
	public static bool IsNaN(this double d)
	{
		return double.IsNaN(d);
	}

	public static IEnumerable<TSource> Do<TSource>(this IEnumerable<TSource> source, Action<TSource> predicate)
	{
		IList<TSource> list = (source as IList<TSource>) ?? source.ToList();
		foreach (TSource item in list)
		{
			predicate(item);
		}
		return list;
	}
}
