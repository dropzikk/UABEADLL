using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionDrawingSurfaceInteropProxy : MicroComProxyBase, ICompositionDrawingSurfaceInterop, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 6;

	public unsafe UnmanagedMethods.POINT BeginDraw(UnmanagedMethods.RECT* updateRect, Guid* iid, void** updateObject)
	{
		UnmanagedMethods.POINT result = default(UnmanagedMethods.POINT);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, updateRect, iid, updateObject, &result);
		if (num != 0)
		{
			throw new COMException("BeginDraw failed", num);
		}
		return result;
	}

	public unsafe void EndDraw()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV);
		if (num != 0)
		{
			throw new COMException("EndDraw failed", num);
		}
	}

	public unsafe void Resize(UnmanagedMethods.POINT sizePixels)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, UnmanagedMethods.POINT, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, sizePixels);
		if (num != 0)
		{
			throw new COMException("Resize failed", num);
		}
	}

	public unsafe void Scroll(UnmanagedMethods.RECT* scrollRect, UnmanagedMethods.RECT* clipRect, int offsetX, int offsetY)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int, int, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, scrollRect, clipRect, offsetX, offsetY);
		if (num != 0)
		{
			throw new COMException("Scroll failed", num);
		}
	}

	public unsafe void ResumeDraw()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV);
		if (num != 0)
		{
			throw new COMException("ResumeDraw failed", num);
		}
	}

	public unsafe void SuspendDraw()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV);
		if (num != 0)
		{
			throw new COMException("SuspendDraw failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionDrawingSurfaceInterop), new Guid("FD04E6E3-FE0C-4C3C-AB19-A07601A576EE"), (IntPtr p, bool owns) => new __MicroComICompositionDrawingSurfaceInteropProxy(p, owns));
	}

	protected __MicroComICompositionDrawingSurfaceInteropProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
