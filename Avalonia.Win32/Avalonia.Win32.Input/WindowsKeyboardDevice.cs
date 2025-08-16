using System.Text;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Utilities;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32.Input;

internal class WindowsKeyboardDevice : KeyboardDevice
{
	private readonly byte[] _keyStates = new byte[256];

	public new static WindowsKeyboardDevice Instance { get; } = new WindowsKeyboardDevice();

	public RawInputModifiers Modifiers
	{
		get
		{
			UpdateKeyStates();
			RawInputModifiers rawInputModifiers = RawInputModifiers.None;
			if (IsDown(Key.LeftAlt) || IsDown(Key.RightAlt))
			{
				rawInputModifiers |= RawInputModifiers.Alt;
			}
			if (IsDown(Key.LeftCtrl) || IsDown(Key.RightCtrl))
			{
				rawInputModifiers |= RawInputModifiers.Control;
			}
			if (IsDown(Key.LeftShift) || IsDown(Key.RightShift))
			{
				rawInputModifiers |= RawInputModifiers.Shift;
			}
			if (IsDown(Key.LWin) || IsDown(Key.RWin))
			{
				rawInputModifiers |= RawInputModifiers.Meta;
			}
			return rawInputModifiers;
		}
	}

	public void WindowActivated(Window window)
	{
		SetFocusedElement(window, NavigationMethod.Unspecified, KeyModifiers.None);
	}

	public string StringFromVirtualKey(uint virtualKey)
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire(256);
		UnmanagedMethods.ToUnicode(virtualKey, 0u, _keyStates, stringBuilder, 256, 0u);
		return StringBuilderCache.GetStringAndRelease(stringBuilder);
	}

	private void UpdateKeyStates()
	{
		UnmanagedMethods.GetKeyboardState(_keyStates);
	}

	private bool IsDown(Key key)
	{
		return (GetKeyStates(key) & KeyStates.Down) != 0;
	}

	private KeyStates GetKeyStates(Key key)
	{
		int num = KeyInterop.VirtualKeyFromKey(key);
		byte num2 = _keyStates[num];
		KeyStates keyStates = KeyStates.None;
		if ((num2 & 0x80) != 0)
		{
			keyStates |= KeyStates.Down;
		}
		if ((num2 & 1) != 0)
		{
			keyStates |= KeyStates.Toggled;
		}
		return keyStates;
	}
}
