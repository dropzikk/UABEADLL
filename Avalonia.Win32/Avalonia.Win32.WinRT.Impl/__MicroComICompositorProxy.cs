using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositorProxy : __MicroComIInspectableProxy, ICompositor, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 24;

	public unsafe void* CreateColorKeyFrameAnimation()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateColorKeyFrameAnimation failed", num);
		}
		return result;
	}

	public unsafe void* CreateColorBrush()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateColorBrush failed", num);
		}
		return result;
	}

	public unsafe ICompositionColorBrush CreateColorBrushWithColor(WinRTColor* color)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, color, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateColorBrushWithColor failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionColorBrush>(pObject, ownsHandle: true);
	}

	public unsafe IContainerVisual CreateContainerVisual()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateContainerVisual failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IContainerVisual>(pObject, ownsHandle: true);
	}

	public unsafe void* CreateCubicBezierEasingFunction(Vector2 controlPoint1, Vector2 controlPoint2)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, Vector2, Vector2, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, controlPoint1, controlPoint2, &result);
		if (num != 0)
		{
			throw new COMException("CreateCubicBezierEasingFunction failed", num);
		}
		return result;
	}

	public unsafe ICompositionEffectFactory CreateEffectFactory(IGraphicsEffect graphicsEffect)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, MicroComRuntime.GetNativePointer(graphicsEffect), &pObject);
		if (num != 0)
		{
			throw new COMException("CreateEffectFactory failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionEffectFactory>(pObject, ownsHandle: true);
	}

	public unsafe void* CreateEffectFactoryWithProperties(void* graphicsEffect, void* animatableProperties)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, graphicsEffect, animatableProperties, &result);
		if (num != 0)
		{
			throw new COMException("CreateEffectFactoryWithProperties failed", num);
		}
		return result;
	}

	public unsafe void* CreateExpressionAnimation()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateExpressionAnimation failed", num);
		}
		return result;
	}

	public unsafe void* CreateExpressionAnimationWithExpression(IntPtr expression)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV, expression, &result);
		if (num != 0)
		{
			throw new COMException("CreateExpressionAnimationWithExpression failed", num);
		}
		return result;
	}

	public unsafe void* CreateInsetClip()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 9])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateInsetClip failed", num);
		}
		return result;
	}

	public unsafe void* CreateInsetClipWithInsets(float leftInset, float topInset, float rightInset, float bottomInset)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, float, float, float, float, void*, int>)(*base.PPV)[base.VTableSize + 10])(base.PPV, leftInset, topInset, rightInset, bottomInset, &result);
		if (num != 0)
		{
			throw new COMException("CreateInsetClipWithInsets failed", num);
		}
		return result;
	}

	public unsafe void* CreateLinearEasingFunction()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 11])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateLinearEasingFunction failed", num);
		}
		return result;
	}

	public unsafe void* CreatePropertySet()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 12])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreatePropertySet failed", num);
		}
		return result;
	}

	public unsafe void* CreateQuaternionKeyFrameAnimation()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 13])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateQuaternionKeyFrameAnimation failed", num);
		}
		return result;
	}

	public unsafe void* CreateScalarKeyFrameAnimation()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 14])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateScalarKeyFrameAnimation failed", num);
		}
		return result;
	}

	public unsafe ICompositionScopedBatch CreateScopedBatch(CompositionBatchTypes batchType)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, CompositionBatchTypes, void*, int>)(*base.PPV)[base.VTableSize + 15])(base.PPV, batchType, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateScopedBatch failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionScopedBatch>(pObject, ownsHandle: true);
	}

	public unsafe ISpriteVisual CreateSpriteVisual()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 16])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateSpriteVisual failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ISpriteVisual>(pObject, ownsHandle: true);
	}

	public unsafe ICompositionSurfaceBrush CreateSurfaceBrush()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 17])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateSurfaceBrush failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionSurfaceBrush>(pObject, ownsHandle: true);
	}

	public unsafe ICompositionSurfaceBrush CreateSurfaceBrushWithSurface(ICompositionSurface surface)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 18])(base.PPV, MicroComRuntime.GetNativePointer(surface), &pObject);
		if (num != 0)
		{
			throw new COMException("CreateSurfaceBrushWithSurface failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ICompositionSurfaceBrush>(pObject, ownsHandle: true);
	}

	public unsafe void* CreateTargetForCurrentView()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 19])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateTargetForCurrentView failed", num);
		}
		return result;
	}

	public unsafe void* CreateVector2KeyFrameAnimation()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 20])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateVector2KeyFrameAnimation failed", num);
		}
		return result;
	}

	public unsafe void* CreateVector3KeyFrameAnimation()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 21])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateVector3KeyFrameAnimation failed", num);
		}
		return result;
	}

	public unsafe void* CreateVector4KeyFrameAnimation()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 22])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("CreateVector4KeyFrameAnimation failed", num);
		}
		return result;
	}

	public unsafe void* GetCommitBatch(CompositionBatchTypes batchType)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, CompositionBatchTypes, void*, int>)(*base.PPV)[base.VTableSize + 23])(base.PPV, batchType, &result);
		if (num != 0)
		{
			throw new COMException("GetCommitBatch failed", num);
		}
		return result;
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositor), new Guid("B403CA50-7F8C-4E83-985F-CC45060036D8"), (IntPtr p, bool owns) => new __MicroComICompositorProxy(p, owns));
	}

	protected __MicroComICompositorProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
