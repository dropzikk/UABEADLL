using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IDesktopWindowContentBridgeInterop : IUnknown, IDisposable
{
	IntPtr HWnd { get; }

	float AppliedScaleFactor { get; }

	void Initialize(ICompositor compositor, IntPtr parentHwnd);
}
