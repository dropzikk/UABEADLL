using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionGraphicsDevice2Proxy : __MicroComIInspectableProxy, ICompositionGraphicsDevice2, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe ICompositionDrawingSurface CreateDrawingSurface2(UnmanagedMethods.SIZE sizePixels, DirectXPixelFormat pixelFormat, DirectXAlphaMode alphaMode)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, UnmanagedMethods.SIZE, DirectXPixelFormat, DirectXAlphaMode, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, sizePixels, pixelFormat, alphaMode, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateDrawingSurface2 failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionDrawingSurface>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionGraphicsDevice2), new Guid("0FB8BDF6-C0F0-4BCC-9FB8-084982490D7D"), (IntPtr p, bool owns) => new __MicroComICompositionGraphicsDevice2Proxy(p, owns));
	}

	protected __MicroComICompositionGraphicsDevice2Proxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
