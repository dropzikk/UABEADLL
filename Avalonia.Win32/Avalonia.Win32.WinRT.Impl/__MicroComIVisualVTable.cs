using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIVisualVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetAnchorPointDelegate(void* @this, Vector2* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetAnchorPointDelegate(void* @this, Vector2 value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetBackfaceVisibilityDelegate(void* @this, CompositionBackfaceVisibility* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetBackfaceVisibilityDelegate(void* @this, CompositionBackfaceVisibility value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetBorderModeDelegate(void* @this, CompositionBorderMode* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetBorderModeDelegate(void* @this, CompositionBorderMode value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetCenterPointDelegate(void* @this, Vector3* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetCenterPointDelegate(void* @this, Vector3 value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetClipDelegate(void* @this, void** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetClipDelegate(void* @this, void* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetCompositeModeDelegate(void* @this, CompositionCompositeMode* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetCompositeModeDelegate(void* @this, CompositionCompositeMode value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetIsVisibleDelegate(void* @this, int* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetIsVisibleDelegate(void* @this, int value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetOffsetDelegate(void* @this, Vector3* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetOffsetDelegate(void* @this, Vector3 value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetOpacityDelegate(void* @this, float* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetOpacityDelegate(void* @this, float value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetOrientationDelegate(void* @this, Quaternion* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetOrientationDelegate(void* @this, Quaternion value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetParentDelegate(void* @this, void** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetRotationAngleDelegate(void* @this, float* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetRotationAngleDelegate(void* @this, float value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetRotationAngleInDegreesDelegate(void* @this, float* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetRotationAngleInDegreesDelegate(void* @this, float value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetRotationAxisDelegate(void* @this, Vector3* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetRotationAxisDelegate(void* @this, Vector3 value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetScaleDelegate(void* @this, Vector3* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetScaleDelegate(void* @this, Vector3 value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetSizeDelegate(void* @this, Vector2* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetSizeDelegate(void* @this, Vector2 value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetTransformMatrixDelegate(void* @this, Matrix4x4* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetTransformMatrixDelegate(void* @this, Matrix4x4 value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetAnchorPoint(void* @this, Vector2* value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			Vector2 anchorPoint = visual.AnchorPoint;
			*value = anchorPoint;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetAnchorPoint(void* @this, Vector2 value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetAnchorPoint(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetBackfaceVisibility(void* @this, CompositionBackfaceVisibility* value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			CompositionBackfaceVisibility backfaceVisibility = visual.BackfaceVisibility;
			*value = backfaceVisibility;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetBackfaceVisibility(void* @this, CompositionBackfaceVisibility value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetBackfaceVisibility(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetBorderMode(void* @this, CompositionBorderMode* value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			CompositionBorderMode borderMode = visual.BorderMode;
			*value = borderMode;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetBorderMode(void* @this, CompositionBorderMode value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetBorderMode(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetCenterPoint(void* @this, Vector3* value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			Vector3 centerPoint = visual.CenterPoint;
			*value = centerPoint;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetCenterPoint(void* @this, Vector3 value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetCenterPoint(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetClip(void* @this, void** value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionClip clip = visual.Clip;
			*value = MicroComRuntime.GetNativePointer(clip, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetClip(void* @this, void* value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetClip(MicroComRuntime.CreateProxyOrNullFor<ICompositionClip>(value, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetCompositeMode(void* @this, CompositionCompositeMode* value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			CompositionCompositeMode compositeMode = visual.CompositeMode;
			*value = compositeMode;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetCompositeMode(void* @this, CompositionCompositeMode value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetCompositeMode(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetIsVisible(void* @this, int* value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int isVisible = visual.IsVisible;
			*value = isVisible;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetIsVisible(void* @this, int value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetIsVisible(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetOffset(void* @this, Vector3* value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			Vector3 offset = visual.Offset;
			*value = offset;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetOffset(void* @this, Vector3 value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetOffset(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetOpacity(void* @this, float* value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			float opacity = visual.Opacity;
			*value = opacity;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetOpacity(void* @this, float value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetOpacity(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetOrientation(void* @this, Quaternion* value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			Quaternion orientation = visual.Orientation;
			*value = orientation;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetOrientation(void* @this, Quaternion value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetOrientation(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetParent(void* @this, void** value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IContainerVisual parent = visual.Parent;
			*value = MicroComRuntime.GetNativePointer(parent, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetRotationAngle(void* @this, float* value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			float rotationAngle = visual.RotationAngle;
			*value = rotationAngle;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetRotationAngle(void* @this, float value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetRotationAngle(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetRotationAngleInDegrees(void* @this, float* value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			float rotationAngleInDegrees = visual.RotationAngleInDegrees;
			*value = rotationAngleInDegrees;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetRotationAngleInDegrees(void* @this, float value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetRotationAngleInDegrees(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetRotationAxis(void* @this, Vector3* value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			Vector3 rotationAxis = visual.RotationAxis;
			*value = rotationAxis;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetRotationAxis(void* @this, Vector3 value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetRotationAxis(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetScale(void* @this, Vector3* value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			Vector3 scale = visual.Scale;
			*value = scale;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetScale(void* @this, Vector3 value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetScale(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetSize(void* @this, Vector2* value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			Vector2 size = visual.Size;
			*value = size;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetSize(void* @this, Vector2 value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetSize(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetTransformMatrix(void* @this, Matrix4x4* value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			Matrix4x4 transformMatrix = visual.TransformMatrix;
			*value = transformMatrix;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetTransformMatrix(void* @this, Matrix4x4 value)
	{
		IVisual visual = null;
		try
		{
			visual = (IVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetTransformMatrix(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIVisualVTable()
	{
		AddMethod((delegate*<void*, Vector2*, int>)(&GetAnchorPoint));
		AddMethod((delegate*<void*, Vector2, int>)(&SetAnchorPoint));
		AddMethod((delegate*<void*, CompositionBackfaceVisibility*, int>)(&GetBackfaceVisibility));
		AddMethod((delegate*<void*, CompositionBackfaceVisibility, int>)(&SetBackfaceVisibility));
		AddMethod((delegate*<void*, CompositionBorderMode*, int>)(&GetBorderMode));
		AddMethod((delegate*<void*, CompositionBorderMode, int>)(&SetBorderMode));
		AddMethod((delegate*<void*, Vector3*, int>)(&GetCenterPoint));
		AddMethod((delegate*<void*, Vector3, int>)(&SetCenterPoint));
		AddMethod((delegate*<void*, void**, int>)(&GetClip));
		AddMethod((delegate*<void*, void*, int>)(&SetClip));
		AddMethod((delegate*<void*, CompositionCompositeMode*, int>)(&GetCompositeMode));
		AddMethod((delegate*<void*, CompositionCompositeMode, int>)(&SetCompositeMode));
		AddMethod((delegate*<void*, int*, int>)(&GetIsVisible));
		AddMethod((delegate*<void*, int, int>)(&SetIsVisible));
		AddMethod((delegate*<void*, Vector3*, int>)(&GetOffset));
		AddMethod((delegate*<void*, Vector3, int>)(&SetOffset));
		AddMethod((delegate*<void*, float*, int>)(&GetOpacity));
		AddMethod((delegate*<void*, float, int>)(&SetOpacity));
		AddMethod((delegate*<void*, Quaternion*, int>)(&GetOrientation));
		AddMethod((delegate*<void*, Quaternion, int>)(&SetOrientation));
		AddMethod((delegate*<void*, void**, int>)(&GetParent));
		AddMethod((delegate*<void*, float*, int>)(&GetRotationAngle));
		AddMethod((delegate*<void*, float, int>)(&SetRotationAngle));
		AddMethod((delegate*<void*, float*, int>)(&GetRotationAngleInDegrees));
		AddMethod((delegate*<void*, float, int>)(&SetRotationAngleInDegrees));
		AddMethod((delegate*<void*, Vector3*, int>)(&GetRotationAxis));
		AddMethod((delegate*<void*, Vector3, int>)(&SetRotationAxis));
		AddMethod((delegate*<void*, Vector3*, int>)(&GetScale));
		AddMethod((delegate*<void*, Vector3, int>)(&SetScale));
		AddMethod((delegate*<void*, Vector2*, int>)(&GetSize));
		AddMethod((delegate*<void*, Vector2, int>)(&SetSize));
		AddMethod((delegate*<void*, Matrix4x4*, int>)(&GetTransformMatrix));
		AddMethod((delegate*<void*, Matrix4x4, int>)(&SetTransformMatrix));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IVisual), new __MicroComIVisualVTable().CreateVTable());
	}
}
