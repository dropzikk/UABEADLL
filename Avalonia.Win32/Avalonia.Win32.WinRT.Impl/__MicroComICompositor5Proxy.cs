using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositor5Proxy : __MicroComIInspectableProxy, ICompositor5, IInspectable, IUnknown, IDisposable
{
	public unsafe IntPtr Comment
	{
		get
		{
			IntPtr result = default(IntPtr);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetComment failed", num);
			}
			return result;
		}
	}

	public unsafe float GlobalPlaybackRate
	{
		get
		{
			float result = 0f;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetGlobalPlaybackRate failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 20;

	public unsafe void SetComment(IntPtr value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetComment failed", num);
		}
	}

	public unsafe void SetGlobalPlaybackRate(float value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, float, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetGlobalPlaybackRate failed", num);
		}
	}

	public unsafe void* CreateBounceScalarAnimation()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateBounceScalarAnimation failed", num);
		}
		return result;
	}

	public unsafe void* CreateBounceVector2Animation()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateBounceVector2Animation failed", num);
		}
		return result;
	}

	public unsafe void* CreateBounceVector3Animation()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateBounceVector3Animation failed", num);
		}
		return result;
	}

	public unsafe void* CreateContainerShape()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateContainerShape failed", num);
		}
		return result;
	}

	public unsafe void* CreateEllipseGeometry()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateEllipseGeometry failed", num);
		}
		return result;
	}

	public unsafe void* CreateLineGeometry()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 9])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateLineGeometry failed", num);
		}
		return result;
	}

	public unsafe void* CreatePathGeometry()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 10])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreatePathGeometry failed", num);
		}
		return result;
	}

	public unsafe void* CreatePathGeometryWithPath(void* path)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 11])(base.PPV, path, &result);
		if (num != 0)
		{
			throw new COMException("CreatePathGeometryWithPath failed", num);
		}
		return result;
	}

	public unsafe void* CreatePathKeyFrameAnimation()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 12])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreatePathKeyFrameAnimation failed", num);
		}
		return result;
	}

	public unsafe void* CreateRectangleGeometry()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 13])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateRectangleGeometry failed", num);
		}
		return result;
	}

	public unsafe ICompositionRoundedRectangleGeometry CreateRoundedRectangleGeometry()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 14])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateRoundedRectangleGeometry failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionRoundedRectangleGeometry>(pObject, ownsHandle: true);
	}

	public unsafe IShapeVisual CreateShapeVisual()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 15])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateShapeVisual failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IShapeVisual>(pObject, ownsHandle: true);
	}

	public unsafe void* CreateSpriteShape()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 16])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateSpriteShape failed", num);
		}
		return result;
	}

	public unsafe void* CreateSpriteShapeWithGeometry(void* geometry)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 17])(base.PPV, geometry, &result);
		if (num != 0)
		{
			throw new COMException("CreateSpriteShapeWithGeometry failed", num);
		}
		return result;
	}

	public unsafe void* CreateViewBox()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 18])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateViewBox failed", num);
		}
		return result;
	}

	public unsafe IAsyncAction RequestCommitAsync()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 19])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("RequestCommitAsync failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAsyncAction>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositor5), new Guid("48EA31AD-7FCD-4076-A79C-90CC4B852C9B"), (IntPtr p, bool owns) => new __MicroComICompositor5Proxy(p, owns));
	}

	protected __MicroComICompositor5Proxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
