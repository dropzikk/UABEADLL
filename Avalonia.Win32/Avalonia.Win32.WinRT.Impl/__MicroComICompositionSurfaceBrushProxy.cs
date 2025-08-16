using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionSurfaceBrushProxy : __MicroComIInspectableProxy, ICompositionSurfaceBrush, IInspectable, IUnknown, IDisposable
{
	public unsafe CompositionBitmapInterpolationMode BitmapInterpolationMode
	{
		get
		{
			CompositionBitmapInterpolationMode result = CompositionBitmapInterpolationMode.NearestNeighbor;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetBitmapInterpolationMode failed", num);
			}
			return result;
		}
	}

	public unsafe float HorizontalAlignmentRatio
	{
		get
		{
			float result = 0f;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetHorizontalAlignmentRatio failed", num);
			}
			return result;
		}
	}

	public unsafe CompositionStretch Stretch
	{
		get
		{
			CompositionStretch result = CompositionStretch.None;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetStretch failed", num);
			}
			return result;
		}
	}

	public unsafe ICompositionSurface Surface
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetSurface failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<ICompositionSurface>(pObject, ownsHandle: true);
		}
	}

	public unsafe float VerticalAlignmentRatio
	{
		get
		{
			float result = 0f;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetVerticalAlignmentRatio failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 10;

	public unsafe void SetBitmapInterpolationMode(CompositionBitmapInterpolationMode value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, CompositionBitmapInterpolationMode, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetBitmapInterpolationMode failed", num);
		}
	}

	public unsafe void SetHorizontalAlignmentRatio(float value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, float, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetHorizontalAlignmentRatio failed", num);
		}
	}

	public unsafe void SetStretch(CompositionStretch value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, CompositionStretch, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetStretch failed", num);
		}
	}

	public unsafe void SetSurface(ICompositionSurface value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, MicroComRuntime.GetNativePointer(value));
		if (num != 0)
		{
			throw new COMException("SetSurface failed", num);
		}
	}

	public unsafe void SetVerticalAlignmentRatio(float value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, float, int>)(*base.PPV)[base.VTableSize + 9])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetVerticalAlignmentRatio failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionSurfaceBrush), new Guid("AD016D79-1E4C-4C0D-9C29-83338C87C162"), (IntPtr p, bool owns) => new __MicroComICompositionSurfaceBrushProxy(p, owns));
	}

	protected __MicroComICompositionSurfaceBrushProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
