using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionTarget : IInspectable, IUnknown, IDisposable
{
	IVisual Root { get; }

	void SetRoot(IVisual value);
}
