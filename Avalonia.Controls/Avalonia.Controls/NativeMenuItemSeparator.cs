namespace Avalonia.Controls;

public class NativeMenuItemSeparator : NativeMenuItem
{
	public NativeMenuItemSeparator()
	{
		SetCurrentValue(NativeMenuItem.HeaderProperty, "-");
	}
}
