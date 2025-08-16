using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIInspectableProxy : MicroComProxyBase, IInspectable, IUnknown, IDisposable
{
	public unsafe IntPtr RuntimeClassName
	{
		get
		{
			IntPtr result = default(IntPtr);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetRuntimeClassName failed", num);
			}
			return result;
		}
	}

	public unsafe TrustLevel TrustLevel
	{
		get
		{
			TrustLevel result = TrustLevel.BaseTrust;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetTrustLevel failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 3;

	public unsafe void GetIids(ulong* iidCount, Guid** iids)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, iidCount, iids);
		if (num != 0)
		{
			throw new COMException("GetIids failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IInspectable), new Guid("AF86E2E0-B12D-4c6a-9C5A-D7AA65101E90"), (IntPtr p, bool owns) => new __MicroComIInspectableProxy(p, owns));
	}

	protected __MicroComIInspectableProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
