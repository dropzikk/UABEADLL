using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIGraphicsEffectProxy : __MicroComIInspectableProxy, IGraphicsEffect, IInspectable, IUnknown, IDisposable
{
	public unsafe IntPtr Name
	{
		get
		{
			IntPtr result = default(IntPtr);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetName failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 2;

	public unsafe void SetName(IntPtr name)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, name);
		if (num != 0)
		{
			throw new COMException("SetName failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IGraphicsEffect), new Guid("CB51C0CE-8FE6-4636-B202-861FAA07D8F3"), (IntPtr p, bool owns) => new __MicroComIGraphicsEffectProxy(p, owns));
	}

	protected __MicroComIGraphicsEffectProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
