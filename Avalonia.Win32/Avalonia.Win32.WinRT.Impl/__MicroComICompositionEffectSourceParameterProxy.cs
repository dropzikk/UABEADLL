using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionEffectSourceParameterProxy : __MicroComIInspectableProxy, ICompositionEffectSourceParameter, IInspectable, IUnknown, IDisposable
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

	protected override int VTableSize => base.VTableSize + 1;

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionEffectSourceParameter), new Guid("858AB13A-3292-4E4E-B3BB-2B6C6544A6EE"), (IntPtr p, bool owns) => new __MicroComICompositionEffectSourceParameterProxy(p, owns));
	}

	protected __MicroComICompositionEffectSourceParameterProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
