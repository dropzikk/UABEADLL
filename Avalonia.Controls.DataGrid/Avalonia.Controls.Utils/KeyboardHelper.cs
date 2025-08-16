using Avalonia.Input;

namespace Avalonia.Controls.Utils;

internal static class KeyboardHelper
{
	public static void GetMetaKeyState(Control target, KeyModifiers modifiers, out bool ctrlOrCmd, out bool shift)
	{
		ctrlOrCmd = modifiers.HasFlag(GetPlatformCtrlOrCmdKeyModifier(target));
		shift = modifiers.HasFlag(KeyModifiers.Shift);
	}

	public static void GetMetaKeyState(Control target, KeyModifiers modifiers, out bool ctrlOrCmd, out bool shift, out bool alt)
	{
		ctrlOrCmd = modifiers.HasFlag(GetPlatformCtrlOrCmdKeyModifier(target));
		shift = modifiers.HasFlag(KeyModifiers.Shift);
		alt = modifiers.HasFlag(KeyModifiers.Alt);
	}

	public static KeyModifiers GetPlatformCtrlOrCmdKeyModifier(Control target)
	{
		return TopLevel.GetTopLevel(target).PlatformSettings.HotkeyConfiguration.CommandModifiers;
	}
}
