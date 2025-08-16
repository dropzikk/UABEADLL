using System.Collections.Generic;

namespace Avalonia.Utilities;

internal static class BinarySearchExtension
{
	private static int GetMedian(int low, int hi)
	{
		return low + (hi - low >> 1);
	}

	public static int BinarySearch<T>(this IReadOnlyList<T> list, T value, IComparer<T> comparer)
	{
		return list.BinarySearch(0, list.Count, value, comparer);
	}

	public static int BinarySearch<T>(this IReadOnlyList<T> list, int index, int length, T value, IComparer<T> comparer)
	{
		int num = index;
		int num2 = index + length - 1;
		while (num <= num2)
		{
			int median = GetMedian(num, num2);
			int num3 = comparer.Compare(list[median], value);
			if (num3 == 0)
			{
				return median;
			}
			if (num3 < 0)
			{
				num = median + 1;
			}
			else
			{
				num2 = median - 1;
			}
		}
		return ~num;
	}
}
