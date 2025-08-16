using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionEffectFactoryProxy : __MicroComIInspectableProxy, ICompositionEffectFactory, IInspectable, IUnknown, IDisposable
{
	public unsafe int ExtendedError
	{
		get
		{
			int result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetExtendedError failed", num);
			}
			return result;
		}
	}

	public unsafe CompositionEffectFactoryLoadStatus LoadStatus
	{
		get
		{
			CompositionEffectFactoryLoadStatus result = CompositionEffectFactoryLoadStatus.Success;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetLoadStatus failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 3;

	public unsafe ICompositionEffectBrush CreateBrush()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateBrush failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionEffectBrush>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionEffectFactory), new Guid("BE5624AF-BA7E-4510-9850-41C0B4FF74DF"), (IntPtr p, bool owns) => new __MicroComICompositionEffectFactoryProxy(p, owns));
	}

	protected __MicroComICompositionEffectFactoryProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
