using System;

namespace Avalonia.Controls;

public class DatePickerSelectedValueChangedEventArgs
{
	public DateTimeOffset? NewDate { get; }

	public DateTimeOffset? OldDate { get; }

	public DatePickerSelectedValueChangedEventArgs(DateTimeOffset? oldDate, DateTimeOffset? newDate)
	{
		NewDate = newDate;
		OldDate = oldDate;
	}
}
