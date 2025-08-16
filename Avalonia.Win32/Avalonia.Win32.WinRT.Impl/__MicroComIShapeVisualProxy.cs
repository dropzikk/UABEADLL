using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIShapeVisualProxy : __MicroComIInspectableProxy, IShapeVisual, IInspectable, IUnknown, IDisposable
{
	public unsafe IUnknown Shapes
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetShapes failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
		}
	}

	protected override int VTableSize => base.VTableSize + 1;

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IShapeVisual), new Guid("F2BD13C3-BA7E-4B0F-9126-FFB7536B8176"), (IntPtr p, bool owns) => new __MicroComIShapeVisualProxy(p, owns));
	}

	protected __MicroComIShapeVisualProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
