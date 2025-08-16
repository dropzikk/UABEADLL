using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositor6Proxy : __MicroComIInspectableProxy, ICompositor6, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 2;

	public unsafe ICompositionGeometricClip CreateGeometricClip()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateGeometricClip failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionGeometricClip>(pObject, ownsHandle: true);
	}

	public unsafe ICompositionGeometricClip CreateGeometricClipWithGeometry(ICompositionGeometry geometry)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, MicroComRuntime.GetNativePointer(geometry), &pObject);
		if (num != 0)
		{
			throw new COMException("CreateGeometricClipWithGeometry failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionGeometricClip>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositor6), new Guid("7A38B2BD-CEC8-4EEB-830F-D8D07AEDEBC3"), (IntPtr p, bool owns) => new __MicroComICompositor6Proxy(p, owns));
	}

	protected __MicroComICompositor6Proxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
