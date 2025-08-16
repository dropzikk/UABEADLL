using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnTrayIcon : IUnknown, IDisposable
{
	unsafe void SetIcon(void* data, IntPtr length);

	void SetMenu(IAvnMenu menu);

	void SetIsVisible(int isVisible);
}
