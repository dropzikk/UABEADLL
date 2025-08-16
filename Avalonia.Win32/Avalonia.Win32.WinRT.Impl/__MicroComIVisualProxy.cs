using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIVisualProxy : __MicroComIInspectableProxy, IVisual, IInspectable, IUnknown, IDisposable
{
	public unsafe Vector2 AnchorPoint
	{
		get
		{
			Vector2 result = default(Vector2);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetAnchorPoint failed", num);
			}
			return result;
		}
	}

	public unsafe CompositionBackfaceVisibility BackfaceVisibility
	{
		get
		{
			CompositionBackfaceVisibility result = CompositionBackfaceVisibility.Inherit;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetBackfaceVisibility failed", num);
			}
			return result;
		}
	}

	public unsafe CompositionBorderMode BorderMode
	{
		get
		{
			CompositionBorderMode result = CompositionBorderMode.Inherit;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetBorderMode failed", num);
			}
			return result;
		}
	}

	public unsafe Vector3 CenterPoint
	{
		get
		{
			Vector3 result = default(Vector3);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetCenterPoint failed", num);
			}
			return result;
		}
	}

	public unsafe ICompositionClip Clip
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetClip failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<ICompositionClip>(pObject, ownsHandle: true);
		}
	}

	public unsafe CompositionCompositeMode CompositeMode
	{
		get
		{
			CompositionCompositeMode result = CompositionCompositeMode.Inherit;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 10])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetCompositeMode failed", num);
			}
			return result;
		}
	}

	public unsafe int IsVisible
	{
		get
		{
			int result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 12])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetIsVisible failed", num);
			}
			return result;
		}
	}

	public unsafe Vector3 Offset
	{
		get
		{
			Vector3 result = default(Vector3);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 14])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetOffset failed", num);
			}
			return result;
		}
	}

	public unsafe float Opacity
	{
		get
		{
			float result = 0f;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 16])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetOpacity failed", num);
			}
			return result;
		}
	}

	public unsafe Quaternion Orientation
	{
		get
		{
			Quaternion result = default(Quaternion);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 18])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetOrientation failed", num);
			}
			return result;
		}
	}

	public unsafe IContainerVisual Parent
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 20])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetParent failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IContainerVisual>(pObject, ownsHandle: true);
		}
	}

	public unsafe float RotationAngle
	{
		get
		{
			float result = 0f;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 21])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetRotationAngle failed", num);
			}
			return result;
		}
	}

	public unsafe float RotationAngleInDegrees
	{
		get
		{
			float result = 0f;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 23])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetRotationAngleInDegrees failed", num);
			}
			return result;
		}
	}

	public unsafe Vector3 RotationAxis
	{
		get
		{
			Vector3 result = default(Vector3);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 25])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetRotationAxis failed", num);
			}
			return result;
		}
	}

	public unsafe Vector3 Scale
	{
		get
		{
			Vector3 result = default(Vector3);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 27])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetScale failed", num);
			}
			return result;
		}
	}

	public unsafe Vector2 Size
	{
		get
		{
			Vector2 result = default(Vector2);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 29])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetSize failed", num);
			}
			return result;
		}
	}

	public unsafe Matrix4x4 TransformMatrix
	{
		get
		{
			Matrix4x4 result = default(Matrix4x4);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 31])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetTransformMatrix failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 33;

	public unsafe void SetAnchorPoint(Vector2 value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, Vector2, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetAnchorPoint failed", num);
		}
	}

	public unsafe void SetBackfaceVisibility(CompositionBackfaceVisibility value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, CompositionBackfaceVisibility, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetBackfaceVisibility failed", num);
		}
	}

	public unsafe void SetBorderMode(CompositionBorderMode value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, CompositionBorderMode, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetBorderMode failed", num);
		}
	}

	public unsafe void SetCenterPoint(Vector3 value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, Vector3, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetCenterPoint failed", num);
		}
	}

	public unsafe void SetClip(ICompositionClip value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 9])(base.PPV, MicroComRuntime.GetNativePointer(value));
		if (num != 0)
		{
			throw new COMException("SetClip failed", num);
		}
	}

	public unsafe void SetCompositeMode(CompositionCompositeMode value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, CompositionCompositeMode, int>)(*base.PPV)[base.VTableSize + 11])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetCompositeMode failed", num);
		}
	}

	public unsafe void SetIsVisible(int value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize + 13])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetIsVisible failed", num);
		}
	}

	public unsafe void SetOffset(Vector3 value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, Vector3, int>)(*base.PPV)[base.VTableSize + 15])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetOffset failed", num);
		}
	}

	public unsafe void SetOpacity(float value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, float, int>)(*base.PPV)[base.VTableSize + 17])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetOpacity failed", num);
		}
	}

	public unsafe void SetOrientation(Quaternion value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, Quaternion, int>)(*base.PPV)[base.VTableSize + 19])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetOrientation failed", num);
		}
	}

	public unsafe void SetRotationAngle(float value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, float, int>)(*base.PPV)[base.VTableSize + 22])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetRotationAngle failed", num);
		}
	}

	public unsafe void SetRotationAngleInDegrees(float value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, float, int>)(*base.PPV)[base.VTableSize + 24])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetRotationAngleInDegrees failed", num);
		}
	}

	public unsafe void SetRotationAxis(Vector3 value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, Vector3, int>)(*base.PPV)[base.VTableSize + 26])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetRotationAxis failed", num);
		}
	}

	public unsafe void SetScale(Vector3 value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, Vector3, int>)(*base.PPV)[base.VTableSize + 28])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetScale failed", num);
		}
	}

	public unsafe void SetSize(Vector2 value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, Vector2, int>)(*base.PPV)[base.VTableSize + 30])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetSize failed", num);
		}
	}

	public unsafe void SetTransformMatrix(Matrix4x4 value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, Matrix4x4, int>)(*base.PPV)[base.VTableSize + 32])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetTransformMatrix failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IVisual), new Guid("117E202D-A859-4C89-873B-C2AA566788E3"), (IntPtr p, bool owns) => new __MicroComIVisualProxy(p, owns));
	}

	protected __MicroComIVisualProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
