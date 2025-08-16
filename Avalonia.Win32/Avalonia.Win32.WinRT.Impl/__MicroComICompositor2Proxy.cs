using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositor2Proxy : __MicroComIInspectableProxy, ICompositor2, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 13;

	public unsafe void* CreateAmbientLight()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateAmbientLight failed", num);
		}
		return result;
	}

	public unsafe void* CreateAnimationGroup()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateAnimationGroup failed", num);
		}
		return result;
	}

	public unsafe ICompositionBackdropBrush CreateBackdropBrush()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateBackdropBrush failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionBackdropBrush>(pObject, ownsHandle: true);
	}

	public unsafe void* CreateDistantLight()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateDistantLight failed", num);
		}
		return result;
	}

	public unsafe void* CreateDropShadow()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateDropShadow failed", num);
		}
		return result;
	}

	public unsafe void* CreateImplicitAnimationCollection()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateImplicitAnimationCollection failed", num);
		}
		return result;
	}

	public unsafe void* CreateLayerVisual()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateLayerVisual failed", num);
		}
		return result;
	}

	public unsafe void* CreateMaskBrush()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateMaskBrush failed", num);
		}
		return result;
	}

	public unsafe void* CreateNineGridBrush()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateNineGridBrush failed", num);
		}
		return result;
	}

	public unsafe void* CreatePointLight()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 9])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreatePointLight failed", num);
		}
		return result;
	}

	public unsafe void* CreateSpotLight()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 10])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateSpotLight failed", num);
		}
		return result;
	}

	public unsafe void* CreateStepEasingFunction()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 11])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateStepEasingFunction failed", num);
		}
		return result;
	}

	public unsafe void* CreateStepEasingFunctionWithStepCount(int stepCount)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, int, void*, int>)(*base.PPV)[base.VTableSize + 12])(base.PPV, stepCount, &result);
		if (num != 0)
		{
			throw new COMException("CreateStepEasingFunctionWithStepCount failed", num);
		}
		return result;
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositor2), new Guid("735081DC-5E24-45DA-A38F-E32CC349A9A0"), (IntPtr p, bool owns) => new __MicroComICompositor2Proxy(p, owns));
	}

	protected __MicroComICompositor2Proxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
