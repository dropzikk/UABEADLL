using System;
using Avalonia.Input.TextInput;

namespace Avalonia.FreeDesktop.DBusIme;

internal class DBusInputMethodFactory<T> : IX11InputMethodFactory where T : ITextInputMethodImpl, IX11InputMethodControl
{
	private readonly Func<IntPtr, T> _factory;

	public DBusInputMethodFactory(Func<IntPtr, T> factory)
	{
		_factory = factory;
	}

	public (ITextInputMethodImpl method, IX11InputMethodControl control) CreateClient(IntPtr xid)
	{
		T val = _factory(xid);
		return (method: val, control: val);
	}
}
