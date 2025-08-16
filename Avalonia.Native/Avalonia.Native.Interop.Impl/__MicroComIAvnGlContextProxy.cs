using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnGlContextProxy : MicroComProxyBase, IAvnGlContext, IUnknown, IDisposable
{
	public unsafe int SampleCount => ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV);

	public unsafe int StencilSize => ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV);

	public unsafe IntPtr NativeHandle => ((delegate* unmanaged[Stdcall]<void*, IntPtr>)(*base.PPV)[base.VTableSize + 4])(base.PPV);

	protected override int VTableSize => base.VTableSize + 5;

	public unsafe IUnknown MakeCurrent()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("MakeCurrent failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe void LegacyMakeCurrent()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV);
		if (num != 0)
		{
			throw new COMException("LegacyMakeCurrent failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnGlContext), new Guid("78c5711e-2a98-40d2-bac4-0cc9a49dc4f3"), (IntPtr p, bool owns) => new __MicroComIAvnGlContextProxy(p, owns));
	}

	protected __MicroComIAvnGlContextProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
