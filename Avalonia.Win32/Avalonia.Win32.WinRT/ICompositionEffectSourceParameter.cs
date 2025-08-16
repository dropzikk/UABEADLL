using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionEffectSourceParameter : IInspectable, IUnknown, IDisposable
{
	IntPtr Name { get; }
}
