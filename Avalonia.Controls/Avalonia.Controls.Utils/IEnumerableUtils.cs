using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia.Controls.Utils;

internal static class IEnumerableUtils
{
	public static bool Contains(this IEnumerable items, object item)
	{
		return items.IndexOf(item) != -1;
	}

	public static bool TryGetCountFast(this IEnumerable? items, out int count)
	{
		if (items != null)
		{
			if (items is ICollection collection)
			{
				count = collection.Count;
				return true;
			}
			if (items is IReadOnlyCollection<object> readOnlyCollection)
			{
				count = readOnlyCollection.Count;
				return true;
			}
		}
		count = 0;
		return false;
	}

	public static int Count(this IEnumerable? items)
	{
		if (items.TryGetCountFast(out var count))
		{
			return count;
		}
		return Enumerable.Count(items?.Cast<object>()) ?? 0;
	}

	public static int IndexOf(this IEnumerable items, object item)
	{
		if (items == null)
		{
			throw new ArgumentNullException("items");
		}
		if (items is IList list)
		{
			return list.IndexOf(item);
		}
		int num = 0;
		foreach (object item2 in items)
		{
			if (item2 == item)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public static object? ElementAt(this IEnumerable items, int index)
	{
		if (items == null)
		{
			throw new ArgumentNullException("items");
		}
		if (items is IList list)
		{
			return list[index];
		}
		return Enumerable.ElementAt(items.Cast<object>(), index);
	}
}
