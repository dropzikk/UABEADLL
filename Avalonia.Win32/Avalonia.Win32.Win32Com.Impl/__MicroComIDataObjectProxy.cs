using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIDataObjectProxy : MicroComProxyBase, IDataObject, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 9;

	public unsafe uint GetData(FORMATETC* pformatetcIn, STGMEDIUM* pmedium)
	{
		return ((delegate* unmanaged[Stdcall]<void*, void*, void*, uint>)(*base.PPV)[base.VTableSize])(base.PPV, pformatetcIn, pmedium);
	}

	public unsafe uint GetDataHere(FORMATETC* pformatetc, STGMEDIUM* pmedium)
	{
		return ((delegate* unmanaged[Stdcall]<void*, void*, void*, uint>)(*base.PPV)[base.VTableSize + 1])(base.PPV, pformatetc, pmedium);
	}

	public unsafe uint QueryGetData(FORMATETC* pformatetc)
	{
		return ((delegate* unmanaged[Stdcall]<void*, void*, uint>)(*base.PPV)[base.VTableSize + 2])(base.PPV, pformatetc);
	}

	public unsafe FORMATETC GetCanonicalFormatEtc(FORMATETC* pformatectIn)
	{
		FORMATETC result = default(FORMATETC);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, pformatectIn, &result);
		if (num != 0)
		{
			throw new COMException("GetCanonicalFormatEtc failed", num);
		}
		return result;
	}

	public unsafe uint SetData(FORMATETC* pformatetc, STGMEDIUM* pmedium, int fRelease)
	{
		return ((delegate* unmanaged[Stdcall]<void*, void*, void*, int, uint>)(*base.PPV)[base.VTableSize + 4])(base.PPV, pformatetc, pmedium, fRelease);
	}

	public unsafe IEnumFORMATETC EnumFormatEtc(int dwDirection)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, int, void*, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, dwDirection, &pObject);
		if (num != 0)
		{
			throw new COMException("EnumFormatEtc failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IEnumFORMATETC>(pObject, ownsHandle: true);
	}

	public unsafe int DAdvise(FORMATETC* pformatetc, int advf, void* pAdvSink)
	{
		int result = 0;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int, void*, void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, pformatetc, advf, pAdvSink, &result);
		if (num != 0)
		{
			throw new COMException("DAdvise failed", num);
		}
		return result;
	}

	public unsafe void DUnadvise(int dwConnection)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, dwConnection);
		if (num != 0)
		{
			throw new COMException("DUnadvise failed", num);
		}
	}

	public unsafe void* EnumDAdvise()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("EnumDAdvise failed", num);
		}
		return result;
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDataObject), new Guid("0000010e-0000-0000-C000-000000000046"), (IntPtr p, bool owns) => new __MicroComIDataObjectProxy(p, owns));
	}

	protected __MicroComIDataObjectProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
