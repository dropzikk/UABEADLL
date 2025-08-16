namespace Avalonia.Input;

public static class NavigationDirectionExtensions
{
	public static bool IsTab(this NavigationDirection direction)
	{
		if (direction != 0)
		{
			return direction == NavigationDirection.Previous;
		}
		return true;
	}

	public static bool IsDirectional(this NavigationDirection direction)
	{
		if (direction > NavigationDirection.Previous)
		{
			return direction <= NavigationDirection.PageDown;
		}
		return false;
	}

	public static NavigationDirection? ToNavigationDirection(this Key key, KeyModifiers modifiers = KeyModifiers.None)
	{
		return key switch
		{
			Key.Tab => ((modifiers & KeyModifiers.Shift) != 0) ? NavigationDirection.Previous : NavigationDirection.Next, 
			Key.Up => NavigationDirection.Up, 
			Key.Down => NavigationDirection.Down, 
			Key.Left => NavigationDirection.Left, 
			Key.Right => NavigationDirection.Right, 
			Key.Home => NavigationDirection.First, 
			Key.End => NavigationDirection.Last, 
			Key.PageUp => NavigationDirection.PageUp, 
			Key.PageDown => NavigationDirection.PageDown, 
			_ => null, 
		};
	}
}
