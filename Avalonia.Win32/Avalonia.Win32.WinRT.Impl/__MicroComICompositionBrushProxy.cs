using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionBrushProxy : __MicroComIInspectableProxy, ICompositionBrush, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize;

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionBrush), new Guid("AB0D7608-30C0-40E9-B568-B60A6BD1FB46"), (IntPtr p, bool owns) => new __MicroComICompositionBrushProxy(p, owns));
	}

	protected __MicroComICompositionBrushProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
