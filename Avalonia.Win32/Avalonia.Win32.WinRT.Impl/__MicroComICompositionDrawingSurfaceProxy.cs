using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionDrawingSurfaceProxy : __MicroComIInspectableProxy, ICompositionDrawingSurface, IInspectable, IUnknown, IDisposable
{
	public unsafe DirectXAlphaMode AlphaMode
	{
		get
		{
			DirectXAlphaMode result = DirectXAlphaMode.Unspecified;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetAlphaMode failed", num);
			}
			return result;
		}
	}

	public unsafe DirectXPixelFormat PixelFormat
	{
		get
		{
			DirectXPixelFormat result = DirectXPixelFormat.Unknown;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetPixelFormat failed", num);
			}
			return result;
		}
	}

	public unsafe UnmanagedMethods.SIZE_F Size
	{
		get
		{
			UnmanagedMethods.SIZE_F result = default(UnmanagedMethods.SIZE_F);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetSize failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 3;

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionDrawingSurface), new Guid("A166C300-FAD0-4D11-9E67-E433162FF49E"), (IntPtr p, bool owns) => new __MicroComICompositionDrawingSurfaceProxy(p, owns));
	}

	protected __MicroComICompositionDrawingSurfaceProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
