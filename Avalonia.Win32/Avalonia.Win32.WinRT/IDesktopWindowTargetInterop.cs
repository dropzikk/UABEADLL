using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IDesktopWindowTargetInterop : IUnknown, IDisposable
{
	IntPtr HWnd { get; }
}
