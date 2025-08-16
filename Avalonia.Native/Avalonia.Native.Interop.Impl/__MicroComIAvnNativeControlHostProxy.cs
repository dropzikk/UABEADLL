using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnNativeControlHostProxy : MicroComProxyBase, IAvnNativeControlHost, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 3;

	public unsafe IntPtr CreateDefaultChild(IntPtr parent)
	{
		IntPtr result = default(IntPtr);
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, parent, &result);
		if (num != 0)
		{
			throw new COMException("CreateDefaultChild failed", num);
		}
		return result;
	}

	public unsafe IAvnNativeControlHostTopLevelAttachment CreateAttachment()
	{
		return MicroComRuntime.CreateProxyOrNullFor<IAvnNativeControlHostTopLevelAttachment>(((delegate* unmanaged[Stdcall]<void*, void*>)(*base.PPV)[base.VTableSize + 1])(base.PPV), ownsHandle: true);
	}

	public unsafe void DestroyDefaultChild(IntPtr child)
	{
		((delegate* unmanaged[Stdcall]<void*, IntPtr, void>)(*base.PPV)[base.VTableSize + 2])(base.PPV, child);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnNativeControlHost), new Guid("91c7f677-f26b-4ff3-93cc-cf15aa966ffa"), (IntPtr p, bool owns) => new __MicroComIAvnNativeControlHostProxy(p, owns));
	}

	protected __MicroComIAvnNativeControlHostProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
