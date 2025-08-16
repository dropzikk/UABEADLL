using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionGeometricClipProxy : __MicroComIInspectableProxy, ICompositionGeometricClip, IInspectable, IUnknown, IDisposable
{
	public unsafe ICompositionGeometry Geometry
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetGeometry failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<ICompositionGeometry>(pObject, ownsHandle: true);
		}
	}

	protected override int VTableSize => base.VTableSize + 2;

	public unsafe void SetGeometry(ICompositionGeometry value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, MicroComRuntime.GetNativePointer(value));
		if (num != 0)
		{
			throw new COMException("SetGeometry failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionGeometricClip), new Guid("C840B581-81C9-4444-A2C1-CCAECE3A50E5"), (IntPtr p, bool owns) => new __MicroComICompositionGeometricClipProxy(p, owns));
	}

	protected __MicroComICompositionGeometricClipProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
