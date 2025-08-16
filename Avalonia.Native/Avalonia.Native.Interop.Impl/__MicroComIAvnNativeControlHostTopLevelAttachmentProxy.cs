using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnNativeControlHostTopLevelAttachmentProxy : MicroComProxyBase, IAvnNativeControlHostTopLevelAttachment, IUnknown, IDisposable
{
	public unsafe IntPtr ParentHandle => ((delegate* unmanaged[Stdcall]<void*, IntPtr>)(*base.PPV)[base.VTableSize])(base.PPV);

	protected override int VTableSize => base.VTableSize + 6;

	public unsafe void InitializeWithChildHandle(IntPtr child)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, child);
		if (num != 0)
		{
			throw new COMException("InitializeWithChildHandle failed", num);
		}
	}

	public unsafe void AttachTo(IAvnNativeControlHost host)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, MicroComRuntime.GetNativePointer(host));
		if (num != 0)
		{
			throw new COMException("AttachTo failed", num);
		}
	}

	public unsafe void ShowInBounds(float x, float y, float width, float height)
	{
		((delegate* unmanaged[Stdcall]<void*, float, float, float, float, void>)(*base.PPV)[base.VTableSize + 3])(base.PPV, x, y, width, height);
	}

	public unsafe void HideWithSize(float width, float height)
	{
		((delegate* unmanaged[Stdcall]<void*, float, float, void>)(*base.PPV)[base.VTableSize + 4])(base.PPV, width, height);
	}

	public unsafe void ReleaseChild()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 5])(base.PPV);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnNativeControlHostTopLevelAttachment), new Guid("14a9e164-1aae-4271-bb78-7b5230999b52"), (IntPtr p, bool owns) => new __MicroComIAvnNativeControlHostTopLevelAttachmentProxy(p, owns));
	}

	protected __MicroComIAvnNativeControlHostTopLevelAttachmentProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
