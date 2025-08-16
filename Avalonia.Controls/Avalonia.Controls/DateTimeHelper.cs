using System;
using System.Globalization;

namespace Avalonia.Controls;

internal static class DateTimeHelper
{
	public static DateTime? AddDays(DateTime time, int days)
	{
		System.Globalization.Calendar calendar = new GregorianCalendar();
		try
		{
			return calendar.AddDays(time, days);
		}
		catch (ArgumentException)
		{
			return null;
		}
	}

	public static DateTime? AddMonths(DateTime time, int months)
	{
		System.Globalization.Calendar calendar = new GregorianCalendar();
		try
		{
			return calendar.AddMonths(time, months);
		}
		catch (ArgumentException)
		{
			return null;
		}
	}

	public static DateTime? AddYears(DateTime time, int years)
	{
		System.Globalization.Calendar calendar = new GregorianCalendar();
		try
		{
			return calendar.AddYears(time, years);
		}
		catch (ArgumentException)
		{
			return null;
		}
	}

	public static int CompareDays(DateTime dt1, DateTime dt2)
	{
		return DateTime.Compare(DiscardTime(dt1), DiscardTime(dt2));
	}

	public static int CompareYearMonth(DateTime dt1, DateTime dt2)
	{
		return (dt1.Year - dt2.Year) * 12 + (dt1.Month - dt2.Month);
	}

	public static int DecadeOfDate(DateTime date)
	{
		return date.Year - date.Year % 10;
	}

	public static DateTime DiscardDayTime(DateTime d)
	{
		return new DateTime(d.Year, d.Month, 1, 0, 0, 0);
	}

	public static DateTime DiscardTime(DateTime d)
	{
		return d.Date;
	}

	public static int EndOfDecade(DateTime date)
	{
		return DecadeOfDate(date) + 9;
	}

	public static DateTimeFormatInfo GetCurrentDateFormat()
	{
		if (CultureInfo.CurrentCulture.Calendar is GregorianCalendar)
		{
			return CultureInfo.CurrentCulture.DateTimeFormat;
		}
		System.Globalization.Calendar[] optionalCalendars = CultureInfo.CurrentCulture.OptionalCalendars;
		foreach (System.Globalization.Calendar calendar in optionalCalendars)
		{
			if (calendar is GregorianCalendar)
			{
				DateTimeFormatInfo dateTimeFormat = new CultureInfo(CultureInfo.CurrentCulture.Name).DateTimeFormat;
				dateTimeFormat.Calendar = calendar;
				return dateTimeFormat;
			}
		}
		DateTimeFormatInfo dateTimeFormat2 = new CultureInfo(CultureInfo.InvariantCulture.Name).DateTimeFormat;
		dateTimeFormat2.Calendar = new GregorianCalendar();
		return dateTimeFormat2;
	}

	public static bool InRange(DateTime date, CalendarDateRange range)
	{
		if (CompareDays(date, range.Start) > -1 && CompareDays(date, range.End) < 1)
		{
			return true;
		}
		return false;
	}

	public static string ToYearMonthPatternString(DateTime date)
	{
		DateTimeFormatInfo currentDateFormat = GetCurrentDateFormat();
		return date.ToString(currentDateFormat.YearMonthPattern, currentDateFormat);
	}

	public static string ToYearString(DateTime date)
	{
		DateTimeFormatInfo currentDateFormat = GetCurrentDateFormat();
		return date.Year.ToString(currentDateFormat);
	}
}
