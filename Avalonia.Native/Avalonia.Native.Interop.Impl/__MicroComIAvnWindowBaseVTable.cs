using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnWindowBaseVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ShowDelegate(void* @this, int activate, int isDialog);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int HideDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CloseDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ActivateDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetClientSizeDelegate(void* @this, AvnSize* ret);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetFrameSizeDelegate(void* @this, AvnSize* result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetScalingDelegate(void* @this, double* ret);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetMinMaxSizeDelegate(void* @this, AvnSize minSize, AvnSize maxSize);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ResizeDelegate(void* @this, double width, double height, AvnPlatformResizeReason reason);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int InvalidateDelegate(void* @this, AvnRect rect);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int BeginMoveDragDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int BeginResizeDragDelegate(void* @this, AvnWindowEdge edge);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetPositionDelegate(void* @this, AvnPoint* ret);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetPositionDelegate(void* @this, AvnPoint point);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int PointToClientDelegate(void* @this, AvnPoint point, AvnPoint* ret);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int PointToScreenDelegate(void* @this, AvnPoint point, AvnPoint* ret);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetTopMostDelegate(void* @this, int value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetCursorDelegate(void* @this, void* cursor);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateGlRenderTargetDelegate(void* @this, void* context, void** ret);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateSoftwareRenderTargetDelegate(void* @this, void** ret);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateMetalRenderTargetDelegate(void* @this, void* device, void** ret);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetMainMenuDelegate(void* @this, void* menu);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ObtainNSWindowHandleDelegate(void* @this, IntPtr* retOut);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ObtainNSWindowHandleRetainedDelegate(void* @this, IntPtr* retOut);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ObtainNSViewHandleDelegate(void* @this, IntPtr* retOut);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ObtainNSViewHandleRetainedDelegate(void* @this, IntPtr* retOut);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateNativeControlHostDelegate(void* @this, void** retOut);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int BeginDragAndDropOperationDelegate(void* @this, AvnDragDropEffects effects, AvnPoint point, void* clipboard, void* cb, IntPtr sourceHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetTransparencyModeDelegate(void* @this, AvnWindowTransparencyMode mode);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetFrameThemeVariantDelegate(void* @this, AvnPlatformThemeVariant mode);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetInputMethodDelegate(void* @this, void** ppv);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Show(void* @this, int activate, int isDialog)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBase.Show(activate, isDialog);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Hide(void* @this)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBase.Hide();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Close(void* @this)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBase.Close();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Activate(void* @this)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBase.Activate();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetClientSize(void* @this, AvnSize* ret)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			AvnSize clientSize = avnWindowBase.ClientSize;
			*ret = clientSize;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetFrameSize(void* @this, AvnSize* result)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBase.GetFrameSize(result);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetScaling(void* @this, double* ret)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			double scaling = avnWindowBase.Scaling;
			*ret = scaling;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetMinMaxSize(void* @this, AvnSize minSize, AvnSize maxSize)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBase.SetMinMaxSize(minSize, maxSize);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Resize(void* @this, double width, double height, AvnPlatformResizeReason reason)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBase.Resize(width, height, reason);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Invalidate(void* @this, AvnRect rect)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBase.Invalidate(rect);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int BeginMoveDrag(void* @this)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBase.BeginMoveDrag();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int BeginResizeDrag(void* @this, AvnWindowEdge edge)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBase.BeginResizeDrag(edge);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetPosition(void* @this, AvnPoint* ret)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			AvnPoint position = avnWindowBase.Position;
			*ret = position;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetPosition(void* @this, AvnPoint point)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBase.SetPosition(point);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int PointToClient(void* @this, AvnPoint point, AvnPoint* ret)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			AvnPoint avnPoint = avnWindowBase.PointToClient(point);
			*ret = avnPoint;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int PointToScreen(void* @this, AvnPoint point, AvnPoint* ret)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			AvnPoint avnPoint = avnWindowBase.PointToScreen(point);
			*ret = avnPoint;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetTopMost(void* @this, int value)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBase.SetTopMost(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetCursor(void* @this, void* cursor)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBase.SetCursor(MicroComRuntime.CreateProxyOrNullFor<IAvnCursor>(cursor, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateGlRenderTarget(void* @this, void* context, void** ret)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnGlSurfaceRenderTarget obj = avnWindowBase.CreateGlRenderTarget(MicroComRuntime.CreateProxyOrNullFor<IAvnGlContext>(context, ownsHandle: false));
			*ret = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateSoftwareRenderTarget(void* @this, void** ret)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnSoftwareRenderTarget obj = avnWindowBase.CreateSoftwareRenderTarget();
			*ret = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateMetalRenderTarget(void* @this, void* device, void** ret)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnMetalRenderTarget obj = avnWindowBase.CreateMetalRenderTarget(MicroComRuntime.CreateProxyOrNullFor<IAvnMetalDevice>(device, ownsHandle: false));
			*ret = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetMainMenu(void* @this, void* menu)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBase.SetMainMenu(MicroComRuntime.CreateProxyOrNullFor<IAvnMenu>(menu, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ObtainNSWindowHandle(void* @this, IntPtr* retOut)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr intPtr = avnWindowBase.ObtainNSWindowHandle();
			*retOut = intPtr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ObtainNSWindowHandleRetained(void* @this, IntPtr* retOut)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr intPtr = avnWindowBase.ObtainNSWindowHandleRetained();
			*retOut = intPtr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ObtainNSViewHandle(void* @this, IntPtr* retOut)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr intPtr = avnWindowBase.ObtainNSViewHandle();
			*retOut = intPtr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ObtainNSViewHandleRetained(void* @this, IntPtr* retOut)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr intPtr = avnWindowBase.ObtainNSViewHandleRetained();
			*retOut = intPtr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateNativeControlHost(void* @this, void** retOut)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnNativeControlHost obj = avnWindowBase.CreateNativeControlHost();
			*retOut = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int BeginDragAndDropOperation(void* @this, AvnDragDropEffects effects, AvnPoint point, void* clipboard, void* cb, IntPtr sourceHandle)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBase.BeginDragAndDropOperation(effects, point, MicroComRuntime.CreateProxyOrNullFor<IAvnClipboard>(clipboard, ownsHandle: false), MicroComRuntime.CreateProxyOrNullFor<IAvnDndResultCallback>(cb, ownsHandle: false), sourceHandle);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetTransparencyMode(void* @this, AvnWindowTransparencyMode mode)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBase.SetTransparencyMode(mode);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetFrameThemeVariant(void* @this, AvnPlatformThemeVariant mode)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindowBase.SetFrameThemeVariant(mode);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetInputMethod(void* @this, void** ppv)
	{
		IAvnWindowBase avnWindowBase = null;
		try
		{
			avnWindowBase = (IAvnWindowBase)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnTextInputMethod inputMethod = avnWindowBase.InputMethod;
			*ppv = MicroComRuntime.GetNativePointer(inputMethod, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindowBase, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnWindowBaseVTable()
	{
		AddMethod((delegate*<void*, int, int, int>)(&Show));
		AddMethod((delegate*<void*, int>)(&Hide));
		AddMethod((delegate*<void*, int>)(&Close));
		AddMethod((delegate*<void*, int>)(&Activate));
		AddMethod((delegate*<void*, AvnSize*, int>)(&GetClientSize));
		AddMethod((delegate*<void*, AvnSize*, int>)(&GetFrameSize));
		AddMethod((delegate*<void*, double*, int>)(&GetScaling));
		AddMethod((delegate*<void*, AvnSize, AvnSize, int>)(&SetMinMaxSize));
		AddMethod((delegate*<void*, double, double, AvnPlatformResizeReason, int>)(&Resize));
		AddMethod((delegate*<void*, AvnRect, int>)(&Invalidate));
		AddMethod((delegate*<void*, int>)(&BeginMoveDrag));
		AddMethod((delegate*<void*, AvnWindowEdge, int>)(&BeginResizeDrag));
		AddMethod((delegate*<void*, AvnPoint*, int>)(&GetPosition));
		AddMethod((delegate*<void*, AvnPoint, int>)(&SetPosition));
		AddMethod((delegate*<void*, AvnPoint, AvnPoint*, int>)(&PointToClient));
		AddMethod((delegate*<void*, AvnPoint, AvnPoint*, int>)(&PointToScreen));
		AddMethod((delegate*<void*, int, int>)(&SetTopMost));
		AddMethod((delegate*<void*, void*, int>)(&SetCursor));
		AddMethod((delegate*<void*, void*, void**, int>)(&CreateGlRenderTarget));
		AddMethod((delegate*<void*, void**, int>)(&CreateSoftwareRenderTarget));
		AddMethod((delegate*<void*, void*, void**, int>)(&CreateMetalRenderTarget));
		AddMethod((delegate*<void*, void*, int>)(&SetMainMenu));
		AddMethod((delegate*<void*, IntPtr*, int>)(&ObtainNSWindowHandle));
		AddMethod((delegate*<void*, IntPtr*, int>)(&ObtainNSWindowHandleRetained));
		AddMethod((delegate*<void*, IntPtr*, int>)(&ObtainNSViewHandle));
		AddMethod((delegate*<void*, IntPtr*, int>)(&ObtainNSViewHandleRetained));
		AddMethod((delegate*<void*, void**, int>)(&CreateNativeControlHost));
		AddMethod((delegate*<void*, AvnDragDropEffects, AvnPoint, void*, void*, IntPtr, int>)(&BeginDragAndDropOperation));
		AddMethod((delegate*<void*, AvnWindowTransparencyMode, int>)(&SetTransparencyMode));
		AddMethod((delegate*<void*, AvnPlatformThemeVariant, int>)(&SetFrameThemeVariant));
		AddMethod((delegate*<void*, void**, int>)(&GetInputMethod));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnWindowBase), new __MicroComIAvnWindowBaseVTable().CreateVTable());
	}
}
