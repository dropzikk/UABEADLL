using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal class MenuEvents : NativeCallbackBase, IAvnMenuEvents, IUnknown, IDisposable
{
	private IAvnMenu _parent;

	public void Initialise(IAvnMenu parent)
	{
		_parent = parent;
	}

	public void NeedsUpdate()
	{
		_parent?.RaiseNeedsUpdate();
	}

	public void Opening()
	{
		_parent?.RaiseOpening();
	}

	public void Closed()
	{
		_parent?.RaiseClosed();
	}
}
