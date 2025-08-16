using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionSurfaceProxy : __MicroComIInspectableProxy, ICompositionSurface, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize;

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionSurface), new Guid("1527540D-42C7-47A6-A408-668F79A90DFB"), (IntPtr p, bool owns) => new __MicroComICompositionSurfaceProxy(p, owns));
	}

	protected __MicroComICompositionSurfaceProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
