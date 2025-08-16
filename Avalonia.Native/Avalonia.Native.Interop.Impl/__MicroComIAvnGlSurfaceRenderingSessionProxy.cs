using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnGlSurfaceRenderingSessionProxy : MicroComProxyBase, IAvnGlSurfaceRenderingSession, IUnknown, IDisposable
{
	public unsafe AvnPixelSize PixelSize
	{
		get
		{
			AvnPixelSize result = default(AvnPixelSize);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetPixelSize failed", num);
			}
			return result;
		}
	}

	public unsafe double Scaling
	{
		get
		{
			double result = 0.0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetScaling failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 2;

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnGlSurfaceRenderingSession), new Guid("e625b406-f04c-484e-946a-4abd2c6015ad"), (IntPtr p, bool owns) => new __MicroComIAvnGlSurfaceRenderingSessionProxy(p, owns));
	}

	protected __MicroComIAvnGlSurfaceRenderingSessionProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
