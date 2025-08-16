using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositor5VTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetCommentDelegate(void* @this, IntPtr* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetCommentDelegate(void* @this, IntPtr value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetGlobalPlaybackRateDelegate(void* @this, float* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetGlobalPlaybackRateDelegate(void* @this, float value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateBounceScalarAnimationDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateBounceVector2AnimationDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateBounceVector3AnimationDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateContainerShapeDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateEllipseGeometryDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateLineGeometryDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreatePathGeometryDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreatePathGeometryWithPathDelegate(void* @this, void* path, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreatePathKeyFrameAnimationDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateRectangleGeometryDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateRoundedRectangleGeometryDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateShapeVisualDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateSpriteShapeDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateSpriteShapeWithGeometryDelegate(void* @this, void* geometry, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateViewBoxDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int RequestCommitAsyncDelegate(void* @this, void** operation);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetComment(void* @this, IntPtr* value)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr comment = compositor.Comment;
			*value = comment;
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
	private unsafe static int SetComment(void* @this, IntPtr value)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositor.SetComment(value);
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
	private unsafe static int GetGlobalPlaybackRate(void* @this, float* value)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			float globalPlaybackRate = compositor.GlobalPlaybackRate;
			*value = globalPlaybackRate;
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
	private unsafe static int SetGlobalPlaybackRate(void* @this, float value)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositor.SetGlobalPlaybackRate(value);
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
	private unsafe static int CreateBounceScalarAnimation(void* @this, void** result)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateBounceScalarAnimation();
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
	private unsafe static int CreateBounceVector2Animation(void* @this, void** result)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateBounceVector2Animation();
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
	private unsafe static int CreateBounceVector3Animation(void* @this, void** result)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateBounceVector3Animation();
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
	private unsafe static int CreateContainerShape(void* @this, void** result)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateContainerShape();
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
	private unsafe static int CreateEllipseGeometry(void* @this, void** result)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateEllipseGeometry();
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
	private unsafe static int CreateLineGeometry(void* @this, void** result)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateLineGeometry();
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
	private unsafe static int CreatePathGeometry(void* @this, void** result)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreatePathGeometry();
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
	private unsafe static int CreatePathGeometryWithPath(void* @this, void* path, void** result)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreatePathGeometryWithPath(path);
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
	private unsafe static int CreatePathKeyFrameAnimation(void* @this, void** result)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreatePathKeyFrameAnimation();
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
	private unsafe static int CreateRectangleGeometry(void* @this, void** result)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateRectangleGeometry();
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
	private unsafe static int CreateRoundedRectangleGeometry(void* @this, void** result)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionRoundedRectangleGeometry obj = compositor.CreateRoundedRectangleGeometry();
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
	private unsafe static int CreateShapeVisual(void* @this, void** result)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IShapeVisual obj = compositor.CreateShapeVisual();
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
	private unsafe static int CreateSpriteShape(void* @this, void** result)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateSpriteShape();
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
	private unsafe static int CreateSpriteShapeWithGeometry(void* @this, void* geometry, void** result)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateSpriteShapeWithGeometry(geometry);
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
	private unsafe static int CreateViewBox(void* @this, void** result)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = compositor.CreateViewBox();
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
	private unsafe static int RequestCommitAsync(void* @this, void** operation)
	{
		ICompositor5 compositor = null;
		try
		{
			compositor = (ICompositor5)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAsyncAction obj = compositor.RequestCommitAsync();
			*operation = MicroComRuntime.GetNativePointer(obj, owned: true);
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

	protected unsafe __MicroComICompositor5VTable()
	{
		AddMethod((delegate*<void*, IntPtr*, int>)(&GetComment));
		AddMethod((delegate*<void*, IntPtr, int>)(&SetComment));
		AddMethod((delegate*<void*, float*, int>)(&GetGlobalPlaybackRate));
		AddMethod((delegate*<void*, float, int>)(&SetGlobalPlaybackRate));
		AddMethod((delegate*<void*, void**, int>)(&CreateBounceScalarAnimation));
		AddMethod((delegate*<void*, void**, int>)(&CreateBounceVector2Animation));
		AddMethod((delegate*<void*, void**, int>)(&CreateBounceVector3Animation));
		AddMethod((delegate*<void*, void**, int>)(&CreateContainerShape));
		AddMethod((delegate*<void*, void**, int>)(&CreateEllipseGeometry));
		AddMethod((delegate*<void*, void**, int>)(&CreateLineGeometry));
		AddMethod((delegate*<void*, void**, int>)(&CreatePathGeometry));
		AddMethod((delegate*<void*, void*, void**, int>)(&CreatePathGeometryWithPath));
		AddMethod((delegate*<void*, void**, int>)(&CreatePathKeyFrameAnimation));
		AddMethod((delegate*<void*, void**, int>)(&CreateRectangleGeometry));
		AddMethod((delegate*<void*, void**, int>)(&CreateRoundedRectangleGeometry));
		AddMethod((delegate*<void*, void**, int>)(&CreateShapeVisual));
		AddMethod((delegate*<void*, void**, int>)(&CreateSpriteShape));
		AddMethod((delegate*<void*, void*, void**, int>)(&CreateSpriteShapeWithGeometry));
		AddMethod((delegate*<void*, void**, int>)(&CreateViewBox));
		AddMethod((delegate*<void*, void**, int>)(&RequestCommitAsync));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositor5), new __MicroComICompositor5VTable().CreateVTable());
	}
}
