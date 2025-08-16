using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;

namespace Avalonia.Controls.Primitives;

public sealed class CalendarBlackoutDatesCollection : ObservableCollection<CalendarDateRange>
{
	private readonly Calendar _owner;

	public CalendarBlackoutDatesCollection(Calendar owner)
	{
		_owner = owner ?? throw new ArgumentNullException("owner");
	}

	public void AddDatesInPast()
	{
		Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1.0)));
	}

	public bool Contains(DateTime date)
	{
		int count = base.Count;
		for (int i = 0; i < count; i++)
		{
			if (DateTimeHelper.InRange(date, base[i]))
			{
				return true;
			}
		}
		return false;
	}

	public bool Contains(DateTime start, DateTime end)
	{
		DateTime t;
		DateTime t2;
		if (DateTime.Compare(end, start) > -1)
		{
			t = DateTimeHelper.DiscardTime(start);
			t2 = DateTimeHelper.DiscardTime(end);
		}
		else
		{
			t = DateTimeHelper.DiscardTime(end);
			t2 = DateTimeHelper.DiscardTime(start);
		}
		int count = base.Count;
		for (int i = 0; i < count; i++)
		{
			CalendarDateRange calendarDateRange = base[i];
			if (DateTime.Compare(calendarDateRange.Start, t) == 0 && DateTime.Compare(calendarDateRange.End, t2) == 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool ContainsAny(CalendarDateRange range)
	{
		return this.Any((CalendarDateRange r) => r.ContainsAny(range));
	}

	protected override void ClearItems()
	{
		EnsureValidThread();
		base.ClearItems();
		_owner.UpdateMonths();
	}

	protected override void InsertItem(int index, CalendarDateRange item)
	{
		EnsureValidThread();
		if (!IsValid(item))
		{
			throw new ArgumentOutOfRangeException("item", "Value is not valid.");
		}
		base.InsertItem(index, item);
		_owner.UpdateMonths();
	}

	protected override void RemoveItem(int index)
	{
		EnsureValidThread();
		base.RemoveItem(index);
		_owner.UpdateMonths();
	}

	protected override void SetItem(int index, CalendarDateRange item)
	{
		EnsureValidThread();
		if (!IsValid(item))
		{
			throw new ArgumentOutOfRangeException("item", "Value is not valid.");
		}
		base.SetItem(index, item);
		_owner.UpdateMonths();
	}

	private bool IsValid(CalendarDateRange item)
	{
		foreach (DateTime selectedDate in _owner.SelectedDates)
		{
			if (DateTimeHelper.InRange(selectedDate, item))
			{
				return false;
			}
		}
		return true;
	}

	private static void EnsureValidThread()
	{
		Dispatcher.UIThread.VerifyAccess();
	}
}
