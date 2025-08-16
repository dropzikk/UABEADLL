using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionGeometryProxy : __MicroComIInspectableProxy, ICompositionGeometry, IInspectable, IUnknown, IDisposable
{
	public unsafe float TrimEnd
	{
		get
		{
			float result = 0f;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetTrimEnd failed", num);
			}
			return result;
		}
	}

	public unsafe float TrimOffset
	{
		get
		{
			float result = 0f;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetTrimOffset failed", num);
			}
			return result;
		}
	}

	public unsafe float TrimStart
	{
		get
		{
			float result = 0f;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetTrimStart failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 6;

	public unsafe void SetTrimEnd(float value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, float, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetTrimEnd failed", num);
		}
	}

	public unsafe void SetTrimOffset(float value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, float, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetTrimOffset failed", num);
		}
	}

	public unsafe void SetTrimStart(float value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, float, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetTrimStart failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionGeometry), new Guid("E985217C-6A17-4207-ABD8-5FD3DD612A9D"), (IntPtr p, bool owns) => new __MicroComICompositionGeometryProxy(p, owns));
	}

	protected __MicroComICompositionGeometryProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
