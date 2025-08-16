using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositorVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateColorKeyFrameAnimationDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateColorBrushDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateColorBrushWithColorDelegate(void* @this, WinRTColor* color, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateContainerVisualDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateCubicBezierEasingFunctionDelegate(void* @this, Vector2 controlPoint1, Vector2 controlPoint2, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateEffectFactoryDelegate(void* @this, void* graphicsEffect, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateEffectFactoryWithPropertiesDelegate(void* @this, void* graphicsEffect, void* animatableProperties, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateExpressionAnimationDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateExpressionAnimationWithExpressionDelegate(void* @this, IntPtr expression, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateInsetClipDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateInsetClipWithInsetsDelegate(void* @this, float leftInset, float topInset, float rightInset, float bottomInset, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateLinearEasingFunctionDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreatePropertySetDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateQuaternionKeyFrameAnimationDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateScalarKeyFrameAnimationDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateScopedBatchDelegate(void* @this, CompositionBatchTypes batchType, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateSpriteVisualDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateSurfaceBrushDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateSurfaceBrushWithSurfaceDelegate(void* @this, void* surface, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateTargetForCurrentViewDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateVector2KeyFrameAnimationDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateVector3KeyFrameAnimationDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateVector4KeyFrameAnimationDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetCommitBatchDelegate(void* @this, CompositionBatchTypes batchType, void** result);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateColorKeyFrameAnimation(void* @this, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateColorKeyFrameAnimation();
			*result = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateColorBrush(void* @this, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateColorBrush();
			*result = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateColorBrushWithColor(void* @this, WinRTColor* color, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionColorBrush obj = compositor.CreateColorBrushWithColor(color);
			*result = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateContainerVisual(void* @this, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IContainerVisual obj = compositor.CreateContainerVisual();
			*result = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateCubicBezierEasingFunction(void* @this, Vector2 controlPoint1, Vector2 controlPoint2, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateCubicBezierEasingFunction(controlPoint1, controlPoint2);
			*result = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateEffectFactory(void* @this, void* graphicsEffect, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionEffectFactory obj = compositor.CreateEffectFactory(MicroComRuntime.CreateProxyOrNullFor<IGraphicsEffect>(graphicsEffect, ownsHandle: false));
			*result = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateEffectFactoryWithProperties(void* @this, void* graphicsEffect, void* animatableProperties, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateEffectFactoryWithProperties(graphicsEffect, animatableProperties);
			*result = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateExpressionAnimation(void* @this, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateExpressionAnimation();
			*result = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateExpressionAnimationWithExpression(void* @this, IntPtr expression, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateExpressionAnimationWithExpression(expression);
			*result = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateInsetClip(void* @this, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateInsetClip();
			*result = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateInsetClipWithInsets(void* @this, float leftInset, float topInset, float rightInset, float bottomInset, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateInsetClipWithInsets(leftInset, topInset, rightInset, bottomInset);
			*result = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateLinearEasingFunction(void* @this, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateLinearEasingFunction();
			*result = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreatePropertySet(void* @this, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreatePropertySet();
			*result = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateQuaternionKeyFrameAnimation(void* @this, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateQuaternionKeyFrameAnimation();
			*result = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateScalarKeyFrameAnimation(void* @this, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateScalarKeyFrameAnimation();
			*result = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateScopedBatch(void* @this, CompositionBatchTypes batchType, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionScopedBatch obj = compositor.CreateScopedBatch(batchType);
			*result = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateSpriteVisual(void* @this, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ISpriteVisual obj = compositor.CreateSpriteVisual();
			*result = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateSurfaceBrush(void* @this, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionSurfaceBrush obj = compositor.CreateSurfaceBrush();
			*result = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateSurfaceBrushWithSurface(void* @this, void* surface, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionSurfaceBrush obj = compositor.CreateSurfaceBrushWithSurface(MicroComRuntime.CreateProxyOrNullFor<ICompositionSurface>(surface, ownsHandle: false));
			*result = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateTargetForCurrentView(void* @this, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateTargetForCurrentView();
			*result = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateVector2KeyFrameAnimation(void* @this, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateVector2KeyFrameAnimation();
			*result = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateVector3KeyFrameAnimation(void* @this, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateVector3KeyFrameAnimation();
			*result = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateVector4KeyFrameAnimation(void* @this, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateVector4KeyFrameAnimation();
			*result = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetCommitBatch(void* @this, CompositionBatchTypes batchType, void** result)
	{
		ICompositor compositor = null;
		try
		{
			compositor = (ICompositor)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* commitBatch = compositor.GetCommitBatch(batchType);
			*result = commitBatch;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositorVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&CreateColorKeyFrameAnimation));
		AddMethod((delegate*<void*, void**, int>)(&CreateColorBrush));
		AddMethod((delegate*<void*, WinRTColor*, void**, int>)(&CreateColorBrushWithColor));
		AddMethod((delegate*<void*, void**, int>)(&CreateContainerVisual));
		AddMethod((delegate*<void*, Vector2, Vector2, void**, int>)(&CreateCubicBezierEasingFunction));
		AddMethod((delegate*<void*, void*, void**, int>)(&CreateEffectFactory));
		AddMethod((delegate*<void*, void*, void*, void**, int>)(&CreateEffectFactoryWithProperties));
		AddMethod((delegate*<void*, void**, int>)(&CreateExpressionAnimation));
		AddMethod((delegate*<void*, IntPtr, void**, int>)(&CreateExpressionAnimationWithExpression));
		AddMethod((delegate*<void*, void**, int>)(&CreateInsetClip));
		AddMethod((delegate*<void*, float, float, float, float, void**, int>)(&CreateInsetClipWithInsets));
		AddMethod((delegate*<void*, void**, int>)(&CreateLinearEasingFunction));
		AddMethod((delegate*<void*, void**, int>)(&CreatePropertySet));
		AddMethod((delegate*<void*, void**, int>)(&CreateQuaternionKeyFrameAnimation));
		AddMethod((delegate*<void*, void**, int>)(&CreateScalarKeyFrameAnimation));
		AddMethod((delegate*<void*, CompositionBatchTypes, void**, int>)(&CreateScopedBatch));
		AddMethod((delegate*<void*, void**, int>)(&CreateSpriteVisual));
		AddMethod((delegate*<void*, void**, int>)(&CreateSurfaceBrush));
		AddMethod((delegate*<void*, void*, void**, int>)(&CreateSurfaceBrushWithSurface));
		AddMethod((delegate*<void*, void**, int>)(&CreateTargetForCurrentView));
		AddMethod((delegate*<void*, void**, int>)(&CreateVector2KeyFrameAnimation));
		AddMethod((delegate*<void*, void**, int>)(&CreateVector3KeyFrameAnimation));
		AddMethod((delegate*<void*, void**, int>)(&CreateVector4KeyFrameAnimation));
		AddMethod((delegate*<void*, CompositionBatchTypes, void**, int>)(&GetCommitBatch));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositor), new __MicroComICompositorVTable().CreateVTable());
	}
}
