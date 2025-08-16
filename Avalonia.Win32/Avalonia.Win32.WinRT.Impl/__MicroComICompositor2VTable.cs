using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositor2VTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateAmbientLightDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateAnimationGroupDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateBackdropBrushDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateDistantLightDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateDropShadowDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateImplicitAnimationCollectionDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateLayerVisualDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateMaskBrushDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateNineGridBrushDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreatePointLightDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateSpotLightDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateStepEasingFunctionDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateStepEasingFunctionWithStepCountDelegate(void* @this, int stepCount, void** result);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateAmbientLight(void* @this, void** result)
	{
		ICompositor2 compositor = null;
		try
		{
			compositor = (ICompositor2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateAmbientLight();
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
	private unsafe static int CreateAnimationGroup(void* @this, void** result)
	{
		ICompositor2 compositor = null;
		try
		{
			compositor = (ICompositor2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateAnimationGroup();
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
	private unsafe static int CreateBackdropBrush(void* @this, void** result)
	{
		ICompositor2 compositor = null;
		try
		{
			compositor = (ICompositor2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionBackdropBrush obj = compositor.CreateBackdropBrush();
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
	private unsafe static int CreateDistantLight(void* @this, void** result)
	{
		ICompositor2 compositor = null;
		try
		{
			compositor = (ICompositor2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateDistantLight();
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
	private unsafe static int CreateDropShadow(void* @this, void** result)
	{
		ICompositor2 compositor = null;
		try
		{
			compositor = (ICompositor2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateDropShadow();
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
	private unsafe static int CreateImplicitAnimationCollection(void* @this, void** result)
	{
		ICompositor2 compositor = null;
		try
		{
			compositor = (ICompositor2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateImplicitAnimationCollection();
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
	private unsafe static int CreateLayerVisual(void* @this, void** result)
	{
		ICompositor2 compositor = null;
		try
		{
			compositor = (ICompositor2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateLayerVisual();
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
	private unsafe static int CreateMaskBrush(void* @this, void** result)
	{
		ICompositor2 compositor = null;
		try
		{
			compositor = (ICompositor2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateMaskBrush();
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
	private unsafe static int CreateNineGridBrush(void* @this, void** result)
	{
		ICompositor2 compositor = null;
		try
		{
			compositor = (ICompositor2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateNineGridBrush();
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
	private unsafe static int CreatePointLight(void* @this, void** result)
	{
		ICompositor2 compositor = null;
		try
		{
			compositor = (ICompositor2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreatePointLight();
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
	private unsafe static int CreateSpotLight(void* @this, void** result)
	{
		ICompositor2 compositor = null;
		try
		{
			compositor = (ICompositor2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateSpotLight();
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
	private unsafe static int CreateStepEasingFunction(void* @this, void** result)
	{
		ICompositor2 compositor = null;
		try
		{
			compositor = (ICompositor2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateStepEasingFunction();
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
	private unsafe static int CreateStepEasingFunctionWithStepCount(void* @this, int stepCount, void** result)
	{
		ICompositor2 compositor = null;
		try
		{
			compositor = (ICompositor2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateStepEasingFunctionWithStepCount(stepCount);
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

	protected unsafe __MicroComICompositor2VTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&CreateAmbientLight));
		AddMethod((delegate*<void*, void**, int>)(&CreateAnimationGroup));
		AddMethod((delegate*<void*, void**, int>)(&CreateBackdropBrush));
		AddMethod((delegate*<void*, void**, int>)(&CreateDistantLight));
		AddMethod((delegate*<void*, void**, int>)(&CreateDropShadow));
		AddMethod((delegate*<void*, void**, int>)(&CreateImplicitAnimationCollection));
		AddMethod((delegate*<void*, void**, int>)(&CreateLayerVisual));
		AddMethod((delegate*<void*, void**, int>)(&CreateMaskBrush));
		AddMethod((delegate*<void*, void**, int>)(&CreateNineGridBrush));
		AddMethod((delegate*<void*, void**, int>)(&CreatePointLight));
		AddMethod((delegate*<void*, void**, int>)(&CreateSpotLight));
		AddMethod((delegate*<void*, void**, int>)(&CreateStepEasingFunction));
		AddMethod((delegate*<void*, int, void**, int>)(&CreateStepEasingFunctionWithStepCount));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositor2), new __MicroComICompositor2VTable().CreateVTable());
	}
}
