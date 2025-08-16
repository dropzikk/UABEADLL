using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionRoundedRectangleGeometryProxy : __MicroComIInspectableProxy, ICompositionRoundedRectangleGeometry, IInspectable, IUnknown, IDisposable
{
	public unsafe Vector2 CornerRadius
	{
		get
		{
			Vector2 result = default(Vector2);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetCornerRadius failed", num);
			}
			return result;
		}
	}

	public unsafe Vector2 Offset
	{
		get
		{
			Vector2 result = default(Vector2);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetOffset failed", num);
			}
			return result;
		}
	}

	public unsafe Vector2 Size
	{
		get
		{
			Vector2 result = default(Vector2);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetSize failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 6;

	public unsafe void SetCornerRadius(Vector2 value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, Vector2, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetCornerRadius failed", num);
		}
	}

	public unsafe void SetOffset(Vector2 value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, Vector2, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetOffset failed", num);
		}
	}

	public unsafe void SetSize(Vector2 value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, Vector2, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetSize failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionRoundedRectangleGeometry), new Guid("8770C822-1D50-4B8B-B013-7C9A0E46935F"), (IntPtr p, bool owns) => new __MicroComICompositionRoundedRectangleGeometryProxy(p, owns));
	}

	protected __MicroComICompositionRoundedRectangleGeometryProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
