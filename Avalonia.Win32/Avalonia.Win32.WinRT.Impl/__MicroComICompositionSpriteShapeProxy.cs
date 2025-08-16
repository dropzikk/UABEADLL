using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionSpriteShapeProxy : __MicroComIInspectableProxy, ICompositionSpriteShape, IInspectable, IUnknown, IDisposable
{
	public unsafe ICompositionBrush FillBrush
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetFillBrush failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<ICompositionBrush>(pObject, ownsHandle: true);
		}
	}

	public unsafe ICompositionGeometry Geometry
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetGeometry failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<ICompositionGeometry>(pObject, ownsHandle: true);
		}
	}

	public unsafe int IsStrokeNonScaling
	{
		get
		{
			int result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetIsStrokeNonScaling failed", num);
			}
			return result;
		}
	}

	public unsafe ICompositionBrush StrokeBrush
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetStrokeBrush failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<ICompositionBrush>(pObject, ownsHandle: true);
		}
	}

	protected override int VTableSize => base.VTableSize + 23;

	public unsafe void SetFillBrush(ICompositionBrush value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, MicroComRuntime.GetNativePointer(value));
		if (num != 0)
		{
			throw new COMException("SetFillBrush failed", num);
		}
	}

	public unsafe void SetGeometry(ICompositionGeometry value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, MicroComRuntime.GetNativePointer(value));
		if (num != 0)
		{
			throw new COMException("SetGeometry failed", num);
		}
	}

	public unsafe void SetIsStrokeNonScaling(int value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetIsStrokeNonScaling failed", num);
		}
	}

	public unsafe void SetStrokeBrush(ICompositionBrush value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, MicroComRuntime.GetNativePointer(value));
		if (num != 0)
		{
			throw new COMException("SetStrokeBrush failed", num);
		}
	}

	public unsafe void GetStrokeDashArray()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV);
		if (num != 0)
		{
			throw new COMException("GetStrokeDashArray failed", num);
		}
	}

	public unsafe void GetStrokeDashCap()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 9])(base.PPV);
		if (num != 0)
		{
			throw new COMException("GetStrokeDashCap failed", num);
		}
	}

	public unsafe void SetStrokeDashCap()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 10])(base.PPV);
		if (num != 0)
		{
			throw new COMException("SetStrokeDashCap failed", num);
		}
	}

	public unsafe void GetStrokeDashOffset()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 11])(base.PPV);
		if (num != 0)
		{
			throw new COMException("GetStrokeDashOffset failed", num);
		}
	}

	public unsafe void SetStrokeDashOffset()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 12])(base.PPV);
		if (num != 0)
		{
			throw new COMException("SetStrokeDashOffset failed", num);
		}
	}

	public unsafe void GetStrokeEndCap()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 13])(base.PPV);
		if (num != 0)
		{
			throw new COMException("GetStrokeEndCap failed", num);
		}
	}

	public unsafe void SetStrokeEndCap()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 14])(base.PPV);
		if (num != 0)
		{
			throw new COMException("SetStrokeEndCap failed", num);
		}
	}

	public unsafe void GetStrokeLineJoin()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 15])(base.PPV);
		if (num != 0)
		{
			throw new COMException("GetStrokeLineJoin failed", num);
		}
	}

	public unsafe void SetStrokeLineJoin()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 16])(base.PPV);
		if (num != 0)
		{
			throw new COMException("SetStrokeLineJoin failed", num);
		}
	}

	public unsafe void GetStrokeMiterLimit()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 17])(base.PPV);
		if (num != 0)
		{
			throw new COMException("GetStrokeMiterLimit failed", num);
		}
	}

	public unsafe void SetStrokeMiterLimit()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 18])(base.PPV);
		if (num != 0)
		{
			throw new COMException("SetStrokeMiterLimit failed", num);
		}
	}

	public unsafe void GetStrokeStartCap()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 19])(base.PPV);
		if (num != 0)
		{
			throw new COMException("GetStrokeStartCap failed", num);
		}
	}

	public unsafe void SetStrokeStartCap()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 20])(base.PPV);
		if (num != 0)
		{
			throw new COMException("SetStrokeStartCap failed", num);
		}
	}

	public unsafe void GetStrokeThickness()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 21])(base.PPV);
		if (num != 0)
		{
			throw new COMException("GetStrokeThickness failed", num);
		}
	}

	public unsafe void SetStrokeThickness()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 22])(base.PPV);
		if (num != 0)
		{
			throw new COMException("SetStrokeThickness failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionSpriteShape), new Guid("401B61BB-0007-4363-B1F3-6BCC003FB83E"), (IntPtr p, bool owns) => new __MicroComICompositionSpriteShapeProxy(p, owns));
	}

	protected __MicroComICompositionSpriteShapeProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
