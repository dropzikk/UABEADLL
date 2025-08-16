using System;

namespace Avalonia.Controls;

public sealed class CalendarDateRange
{
	public DateTime Start { get; private set; }

	public DateTime End { get; private set; }

	public CalendarDateRange(DateTime day)
	{
		Start = day;
		End = day;
	}

	public CalendarDateRange(DateTime start, DateTime end)
	{
		if (DateTime.Compare(end, start) >= 0)
		{
			Start = start;
			End = end;
		}
		else
		{
			Start = start;
			End = start;
		}
	}

	internal bool ContainsAny(CalendarDateRange range)
	{
		if (range == null)
		{
			throw new ArgumentNullException("range");
		}
		int num = DateTime.Compare(Start, range.Start);
		if (num > 0 || DateTime.Compare(End, range.Start) < 0)
		{
			if (num >= 0)
			{
				return DateTime.Compare(Start, range.End) <= 0;
			}
			return false;
		}
		return true;
	}
}
