namespace Avalonia.Controls.Primitives;

public class DataGridFrozenGrid : Grid
{
	public static readonly StyledProperty<bool> IsFrozenProperty = AvaloniaProperty.RegisterAttached<DataGridFrozenGrid, Control, bool>("IsFrozen", defaultValue: false);

	public static bool GetIsFrozen(Control element)
	{
		return element.GetValue(IsFrozenProperty);
	}

	public static void SetIsFrozen(Control element, bool value)
	{
		element.SetValue(IsFrozenProperty, value);
	}
}
