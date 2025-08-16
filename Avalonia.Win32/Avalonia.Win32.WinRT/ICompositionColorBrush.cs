using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionColorBrush : IInspectable, IUnknown, IDisposable
{
	WinRTColor Color { get; }

	void SetColor(WinRTColor value);
}
