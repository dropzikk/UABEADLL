using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionEffectBrush : IInspectable, IUnknown, IDisposable
{
	ICompositionBrush GetSourceParameter(IntPtr name);

	void SetSourceParameter(IntPtr name, ICompositionBrush source);
}
