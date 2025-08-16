using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionEffectSourceParameterFactoryProxy : __MicroComIInspectableProxy, ICompositionEffectSourceParameterFactory, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe ICompositionEffectSourceParameter Create(IntPtr name)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, name, &pObject);
		if (num != 0)
		{
			throw new COMException("Create failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionEffectSourceParameter>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionEffectSourceParameterFactory), new Guid("B3D9F276-ABA3-4724-ACF3-D0397464DB1C"), (IntPtr p, bool owns) => new __MicroComICompositionEffectSourceParameterFactoryProxy(p, owns));
	}

	protected __MicroComICompositionEffectSourceParameterFactoryProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
