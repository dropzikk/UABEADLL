using System;
using Avalonia.Native.Interop;
using MicroCom.Runtime;

namespace Avalonia.Native;

internal class MenuActionCallback : NativeCallbackBase, IAvnActionCallback, IUnknown, IDisposable
{
	private Action _action;

	public MenuActionCallback(Action action)
	{
		_action = action;
	}

	void IAvnActionCallback.Run()
	{
		_action?.Invoke();
	}
}
