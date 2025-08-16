using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IDesktopWindowTarget : IInspectable, IUnknown, IDisposable
{
	int IsTopmost { get; }
}
