using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionTargetProxy : __MicroComIInspectableProxy, ICompositionTarget, IInspectable, IUnknown, IDisposable
{
	public unsafe IVisual Root
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetRoot failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IVisual>(pObject, ownsHandle: true);
		}
	}

	protected override int VTableSize => base.VTableSize + 2;

	public unsafe void SetRoot(IVisual value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, MicroComRuntime.GetNativePointer(value));
		if (num != 0)
		{
			throw new COMException("SetRoot failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionTarget), new Guid("A1BEA8BA-D726-4663-8129-6B5E7927FFA6"), (IntPtr p, bool owns) => new __MicroComICompositionTargetProxy(p, owns));
	}

	protected __MicroComICompositionTargetProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
