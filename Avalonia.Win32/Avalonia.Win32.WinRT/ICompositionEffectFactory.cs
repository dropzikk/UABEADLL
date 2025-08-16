using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionEffectFactory : IInspectable, IUnknown, IDisposable
{
	int ExtendedError { get; }

	CompositionEffectFactoryLoadStatus LoadStatus { get; }

	ICompositionEffectBrush CreateBrush();
}
