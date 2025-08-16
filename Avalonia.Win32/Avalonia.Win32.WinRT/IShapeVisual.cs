using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IShapeVisual : IInspectable, IUnknown, IDisposable
{
	IUnknown Shapes { get; }
}
