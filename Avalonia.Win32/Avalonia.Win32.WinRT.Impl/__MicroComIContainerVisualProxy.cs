using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIContainerVisualProxy : __MicroComIInspectableProxy, IContainerVisual, IInspectable, IUnknown, IDisposable
{
	public unsafe IVisualCollection Children
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetChildren failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IVisualCollection>(pObject, ownsHandle: true);
		}
	}

	protected override int VTableSize => base.VTableSize + 1;

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IContainerVisual), new Guid("02F6BC74-ED20-4773-AFE6-D49B4A93DB32"), (IntPtr p, bool owns) => new __MicroComIContainerVisualProxy(p, owns));
	}

	protected __MicroComIContainerVisualProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
