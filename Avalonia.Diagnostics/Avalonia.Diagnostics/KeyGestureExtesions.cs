using Avalonia.Input;
using Avalonia.Input.Raw;

namespace Avalonia.Diagnostics;

internal static class KeyGestureExtesions
{
	public static bool Matches(this KeyGesture gesture, RawKeyEventArgs keyEvent)
	{
		if ((keyEvent.Modifiers & RawInputModifiers.KeyboardMask) == (RawInputModifiers)gesture.KeyModifiers)
		{
			return ResolveNumPadOperationKey(keyEvent.Key) == ResolveNumPadOperationKey(gesture.Key);
		}
		return false;
	}

	private static Key ResolveNumPadOperationKey(Key key)
	{
		return key switch
		{
			Key.Add => Key.OemPlus, 
			Key.Subtract => Key.OemMinus, 
			Key.Decimal => Key.OemPeriod, 
			_ => key, 
		};
	}
}
