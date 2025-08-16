using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionEffectSourceParameterFactory : IInspectable, IUnknown, IDisposable
{
	ICompositionEffectSourceParameter Create(IntPtr name);
}
