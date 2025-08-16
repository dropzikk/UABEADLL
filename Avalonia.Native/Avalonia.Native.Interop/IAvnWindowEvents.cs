using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnWindowEvents : IAvnWindowBaseEvents, IUnknown, IDisposable
{
	int Closing();

	void WindowStateChanged(AvnWindowState state);

	void GotInputWhenDisabled();
}
