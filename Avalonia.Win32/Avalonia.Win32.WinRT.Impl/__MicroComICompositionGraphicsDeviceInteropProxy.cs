using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionGraphicsDeviceInteropProxy : MicroComProxyBase, ICompositionGraphicsDeviceInterop, IUnknown, IDisposable
{
	public unsafe IUnknown RenderingDevice
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetRenderingDevice failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
		}
	}

	protected override int VTableSize => base.VTableSize + 2;

	public unsafe void SetRenderingDevice(IUnknown value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, MicroComRuntime.GetNativePointer(value));
		if (num != 0)
		{
			throw new COMException("SetRenderingDevice failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionGraphicsDeviceInterop), new Guid("A116FF71-F8BF-4C8A-9C98-70779A32A9C8"), (IntPtr p, bool owns) => new __MicroComICompositionGraphicsDeviceInteropProxy(p, owns));
	}

	protected __MicroComICompositionGraphicsDeviceInteropProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
