using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositorDesktopInterop : IUnknown, IDisposable
{
	IDesktopWindowTarget CreateDesktopWindowTarget(IntPtr hwndTarget, int isTopmost);

	void EnsureOnThread(int threadId);
}
