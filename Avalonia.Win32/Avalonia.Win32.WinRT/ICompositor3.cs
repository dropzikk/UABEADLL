using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositor3 : IInspectable, IUnknown, IDisposable
{
	ICompositionBackdropBrush CreateHostBackdropBrush();
}
