using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnMacOptionsProxy : MicroComProxyBase, IAvnMacOptions, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 4;

	public unsafe void SetShowInDock(int show)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize])(base.PPV, show);
		if (num != 0)
		{
			throw new COMException("SetShowInDock failed", num);
		}
	}

	public unsafe void SetApplicationTitle(string utf8string)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(utf8string) + 1];
		Encoding.UTF8.GetBytes(utf8string, 0, utf8string.Length, array, 0);
		int num;
		fixed (byte* ptr = array)
		{
			num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, ptr);
		}
		if (num != 0)
		{
			throw new COMException("SetApplicationTitle failed", num);
		}
	}

	public unsafe void SetDisableSetProcessName(int disable)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, disable);
		if (num != 0)
		{
			throw new COMException("SetDisableSetProcessName failed", num);
		}
	}

	public unsafe void SetDisableAppDelegate(int disable)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, disable);
		if (num != 0)
		{
			throw new COMException("SetDisableAppDelegate failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnMacOptions), new Guid("e34ae0f8-18b4-48a3-b09d-2e6b19a3cf5e"), (IntPtr p, bool owns) => new __MicroComIAvnMacOptionsProxy(p, owns));
	}

	protected __MicroComIAvnMacOptionsProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
