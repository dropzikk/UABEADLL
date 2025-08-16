using System;
using Avalonia.Interactivity;

namespace Avalonia.Controls;

public class CalendarDateChangedEventArgs : RoutedEventArgs
{
	public DateTime? RemovedDate { get; private set; }

	public DateTime? AddedDate { get; private set; }

	internal CalendarDateChangedEventArgs(DateTime? removedDate, DateTime? addedDate)
	{
		RemovedDate = removedDate;
		AddedDate = addedDate;
	}
}
