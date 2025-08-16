using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IContainerVisual : IInspectable, IUnknown, IDisposable
{
	IVisualCollection Children { get; }
}
