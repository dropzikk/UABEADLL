using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIShellItemProxy : MicroComProxyBase, IShellItem, IUnknown, IDisposable
{
	public unsafe IShellItem Parent
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetParent failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IShellItem>(pObject, ownsHandle: true);
		}
	}

	protected override int VTableSize => base.VTableSize + 5;

	public unsafe void* BindToHandler(void* pbc, Guid* bhid, Guid* riid)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, pbc, bhid, riid, &result);
		if (num != 0)
		{
			throw new COMException("BindToHandler failed", num);
		}
		return result;
	}

	public unsafe char* GetDisplayName(uint sigdnName)
	{
		char* result = default(char*);
		int num = ((delegate* unmanaged[Stdcall]<void*, uint, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, sigdnName, &result);
		if (num != 0)
		{
			throw new COMException("GetDisplayName failed", num);
		}
		return result;
	}

	public unsafe uint GetAttributes(uint sfgaoMask)
	{
		uint result = 0u;
		int num = ((delegate* unmanaged[Stdcall]<void*, uint, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, sfgaoMask, &result);
		if (num != 0)
		{
			throw new COMException("GetAttributes failed", num);
		}
		return result;
	}

	public unsafe int Compare(IShellItem psi, uint hint)
	{
		int result = 0;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, uint, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, MicroComRuntime.GetNativePointer(psi), hint, &result);
		if (num != 0)
		{
			throw new COMException("Compare failed", num);
		}
		return result;
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IShellItem), new Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe"), (IntPtr p, bool owns) => new __MicroComIShellItemProxy(p, owns));
	}

	protected __MicroComIShellItemProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
