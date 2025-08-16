using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IUISettings3 : IInspectable, IUnknown, IDisposable
{
	WinRTColor GetColorValue(UIColorType desiredColor);
}
