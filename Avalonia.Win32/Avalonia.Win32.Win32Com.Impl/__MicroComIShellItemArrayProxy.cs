using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIShellItemArrayProxy : MicroComProxyBase, IShellItemArray, IUnknown, IDisposable
{
	public unsafe int Count
	{
		get
		{
			int result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetCount failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 7;

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

	public unsafe void* GetPropertyStore(ushort flags, Guid* riid)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, ushort, void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, flags, riid, &result);
		if (num != 0)
		{
			throw new COMException("GetPropertyStore failed", num);
		}
		return result;
	}

	public unsafe void* GetPropertyDescriptionList(void* keyType, Guid* riid)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, keyType, riid, &result);
		if (num != 0)
		{
			throw new COMException("GetPropertyDescriptionList failed", num);
		}
		return result;
	}

	public unsafe ushort GetAttributes(int AttribFlags, ushort sfgaoMask)
	{
		ushort result = 0;
		int num = ((delegate* unmanaged[Stdcall]<void*, int, ushort, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, AttribFlags, sfgaoMask, &result);
		if (num != 0)
		{
			throw new COMException("GetAttributes failed", num);
		}
		return result;
	}

	public unsafe IShellItem GetItemAt(int dwIndex)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, int, void*, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, dwIndex, &pObject);
		if (num != 0)
		{
			throw new COMException("GetItemAt failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IShellItem>(pObject, ownsHandle: true);
	}

	public unsafe void* EnumItems()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("EnumItems failed", num);
		}
		return result;
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IShellItemArray), new Guid("B63EA76D-1F85-456F-A19C-48159EFA858B"), (IntPtr p, bool owns) => new __MicroComIShellItemArrayProxy(p, owns));
	}

	protected __MicroComIShellItemArrayProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
