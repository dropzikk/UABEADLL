using Avalonia.Interactivity;

namespace Avalonia.Controls;

public class CalendarModeChangedEventArgs : RoutedEventArgs
{
	public CalendarMode OldMode { get; private set; }

	public CalendarMode NewMode { get; private set; }

	public CalendarModeChangedEventArgs(CalendarMode oldMode, CalendarMode newMode)
	{
		OldMode = oldMode;
		NewMode = newMode;
	}
}
