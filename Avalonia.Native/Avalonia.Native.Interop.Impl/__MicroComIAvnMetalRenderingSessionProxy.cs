using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnMetalRenderingSessionProxy : MicroComProxyBase, IAvnMetalRenderingSession, IUnknown, IDisposable
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

	public unsafe double Scaling => ((delegate* unmanaged[Stdcall]<void*, double>)(*base.PPV)[base.VTableSize + 1])(base.PPV);

	public unsafe IntPtr Texture => ((delegate* unmanaged[Stdcall]<void*, IntPtr>)(*base.PPV)[base.VTableSize + 2])(base.PPV);

	protected override int VTableSize => base.VTableSize + 3;

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnMetalRenderingSession), new Guid("e625b406-f04c-484e-946a-4abd2c6015ad"), (IntPtr p, bool owns) => new __MicroComIAvnMetalRenderingSessionProxy(p, owns));
	}

	protected __MicroComIAvnMetalRenderingSessionProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
