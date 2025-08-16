using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionEffectBrushProxy : __MicroComIInspectableProxy, ICompositionEffectBrush, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 2;

	public unsafe ICompositionBrush GetSourceParameter(IntPtr name)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, name, &pObject);
		if (num != 0)
		{
			throw new COMException("GetSourceParameter failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionBrush>(pObject, ownsHandle: true);
	}

	public unsafe void SetSourceParameter(IntPtr name, ICompositionBrush source)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, name, MicroComRuntime.GetNativePointer(source));
		if (num != 0)
		{
			throw new COMException("SetSourceParameter failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionEffectBrush), new Guid("BF7F795E-83CC-44BF-A447-3E3C071789EC"), (IntPtr p, bool owns) => new __MicroComICompositionEffectBrushProxy(p, owns));
	}

	protected __MicroComICompositionEffectBrushProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
