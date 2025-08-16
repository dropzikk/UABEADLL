using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnGlDisplayProxy : MicroComProxyBase, IAvnGlDisplay, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 4;

	public unsafe IAvnGlContext CreateContext(IAvnGlContext share)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, MicroComRuntime.GetNativePointer(share), &pObject);
		if (num != 0)
		{
			throw new COMException("CreateContext failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnGlContext>(pObject, ownsHandle: true);
	}

	public unsafe void LegacyClearCurrentContext()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 1])(base.PPV);
	}

	public unsafe IAvnGlContext WrapContext(IntPtr native)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, native, &pObject);
		if (num != 0)
		{
			throw new COMException("WrapContext failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnGlContext>(pObject, ownsHandle: true);
	}

	public unsafe IntPtr GetProcAddress(string proc)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(proc) + 1];
		Encoding.UTF8.GetBytes(proc, 0, proc.Length, array, 0);
		IntPtr result;
		fixed (byte* ptr = array)
		{
			result = ((delegate* unmanaged[Stdcall]<void*, void*, IntPtr>)(*base.PPV)[base.VTableSize + 3])(base.PPV, ptr);
		}
		return result;
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnGlDisplay), new Guid("60452465-8616-40af-bc00-042e69828ce7"), (IntPtr p, bool owns) => new __MicroComIAvnGlDisplayProxy(p, owns));
	}

	protected __MicroComIAvnGlDisplayProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
