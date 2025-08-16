using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ISpriteVisual : IInspectable, IUnknown, IDisposable
{
	ICompositionBrush Brush { get; }

	void SetBrush(ICompositionBrush value);
}
