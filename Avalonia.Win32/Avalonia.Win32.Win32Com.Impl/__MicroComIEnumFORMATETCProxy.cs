using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIEnumFORMATETCProxy : MicroComProxyBase, IEnumFORMATETC, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 4;

	public unsafe uint Next(uint celt, FORMATETC* rgelt, uint* pceltFetched)
	{
		return ((delegate* unmanaged[Stdcall]<void*, uint, void*, void*, uint>)(*base.PPV)[base.VTableSize])(base.PPV, celt, rgelt, pceltFetched);
	}

	public unsafe uint Skip(uint celt)
	{
		return ((delegate* unmanaged[Stdcall]<void*, uint, uint>)(*base.PPV)[base.VTableSize + 1])(base.PPV, celt);
	}

	public unsafe void Reset()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV);
		if (num != 0)
		{
			throw new COMException("Reset failed", num);
		}
	}

	public unsafe IEnumFORMATETC Clone()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("Clone failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IEnumFORMATETC>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IEnumFORMATETC), new Guid("00000103-0000-0000-C000-000000000046"), (IntPtr p, bool owns) => new __MicroComIEnumFORMATETCProxy(p, owns));
	}

	protected __MicroComIEnumFORMATETCProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
