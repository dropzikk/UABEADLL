using System;
using Avalonia.Input.TextInput;

namespace Avalonia.FreeDesktop;

internal interface IX11InputMethodFactory
{
	(ITextInputMethodImpl method, IX11InputMethodControl control) CreateClient(IntPtr xid);
}
