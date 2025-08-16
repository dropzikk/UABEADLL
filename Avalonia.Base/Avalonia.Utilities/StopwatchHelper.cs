using System;
using System.Diagnostics;

namespace Avalonia.Utilities;

internal static class StopwatchHelper
{
	private static readonly double s_timestampToTicks = 10000000.0 / (double)Stopwatch.Frequency;

	public static TimeSpan GetElapsedTime(long startingTimestamp)
	{
		return GetElapsedTime(startingTimestamp, Stopwatch.GetTimestamp());
	}

	public static TimeSpan GetElapsedTime(long startingTimestamp, long endingTimestamp)
	{
		return new TimeSpan((long)((double)(endingTimestamp - startingTimestamp) * s_timestampToTicks));
	}
}
