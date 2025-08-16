using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionGraphicsDeviceProxy : __MicroComIInspectableProxy, ICompositionGraphicsDevice, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 3;

	public unsafe ICompositionDrawingSurface CreateDrawingSurface(UnmanagedMethods.SIZE_F sizePixels, DirectXPixelFormat pixelFormat, DirectXAlphaMode alphaMode)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, UnmanagedMethods.SIZE_F, DirectXPixelFormat, DirectXAlphaMode, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, sizePixels, pixelFormat, alphaMode, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateDrawingSurface failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionDrawingSurface>(pObject, ownsHandle: true);
	}

	public unsafe void AddRenderingDeviceReplaced(void* handler, void* token)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, handler, token);
		if (num != 0)
		{
			throw new COMException("AddRenderingDeviceReplaced failed", num);
		}
	}

	public unsafe void RemoveRenderingDeviceReplaced(int token)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, token);
		if (num != 0)
		{
			throw new COMException("RemoveRenderingDeviceReplaced failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionGraphicsDevice), new Guid("FB22C6E1-80A2-4667-9936-DBEAF6EEFE95"), (IntPtr p, bool owns) => new __MicroComICompositionGraphicsDeviceProxy(p, owns));
	}

	protected __MicroComICompositionGraphicsDeviceProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
