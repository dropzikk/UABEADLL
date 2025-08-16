using System;
using System.Collections.Generic;

namespace SixLabors.ImageSharp;

internal static class EnumerableExtensions
{
	public static IEnumerable<int> SteppedRange(int fromInclusive, Func<int, bool> toDelegate, int step)
	{
		return RangeIterator(fromInclusive, toDelegate, step);
	}

	private static IEnumerable<int> RangeIterator(int fromInclusive, Func<int, bool> toDelegate, int step)
	{
		for (int i = fromInclusive; toDelegate(i); i += step)
		{
			yield return i;
		}
	}
}
