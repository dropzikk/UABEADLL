namespace Avalonia;

public static class AvaloniaPropertyChangedExtensions
{
	public static T GetOldValue<T>(this AvaloniaPropertyChangedEventArgs e)
	{
		return ((AvaloniaPropertyChangedEventArgs<T>)e).OldValue.GetValueOrDefault();
	}

	public static T GetNewValue<T>(this AvaloniaPropertyChangedEventArgs e)
	{
		return ((AvaloniaPropertyChangedEventArgs<T>)e).NewValue.GetValueOrDefault();
	}

	public static (T oldValue, T newValue) GetOldAndNewValue<T>(this AvaloniaPropertyChangedEventArgs e)
	{
		AvaloniaPropertyChangedEventArgs<T> avaloniaPropertyChangedEventArgs = (AvaloniaPropertyChangedEventArgs<T>)e;
		return (oldValue: avaloniaPropertyChangedEventArgs.OldValue.GetValueOrDefault(), newValue: avaloniaPropertyChangedEventArgs.NewValue.GetValueOrDefault());
	}
}
