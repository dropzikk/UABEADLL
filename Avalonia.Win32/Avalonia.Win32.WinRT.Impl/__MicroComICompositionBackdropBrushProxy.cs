using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionBackdropBrushProxy : __MicroComIInspectableProxy, ICompositionBackdropBrush, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize;

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionBackdropBrush), new Guid("C5ACAE58-3898-499E-8D7F-224E91286A5D"), (IntPtr p, bool owns) => new __MicroComICompositionBackdropBrushProxy(p, owns));
	}

	protected __MicroComICompositionBackdropBrushProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
