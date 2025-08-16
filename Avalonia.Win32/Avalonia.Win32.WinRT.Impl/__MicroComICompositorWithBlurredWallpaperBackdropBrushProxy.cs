using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositorWithBlurredWallpaperBackdropBrushProxy : __MicroComIInspectableProxy, ICompositorWithBlurredWallpaperBackdropBrush, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe ICompositionBackdropBrush TryCreateBlurredWallpaperBackdropBrush()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("TryCreateBlurredWallpaperBackdropBrush failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionBackdropBrush>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositorWithBlurredWallpaperBackdropBrush), new Guid("0D8FB190-F122-5B8D-9FDD-543B0D8EB7F3"), (IntPtr p, bool owns) => new __MicroComICompositorWithBlurredWallpaperBackdropBrushProxy(p, owns));
	}

	protected __MicroComICompositorWithBlurredWallpaperBackdropBrushProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
