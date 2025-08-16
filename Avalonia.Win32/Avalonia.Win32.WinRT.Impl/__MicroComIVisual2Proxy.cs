using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIVisual2Proxy : __MicroComIInspectableProxy, IVisual2, IInspectable, IUnknown, IDisposable
{
	public unsafe IVisual ParentForTransform
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetParentForTransform failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IVisual>(pObject, ownsHandle: true);
		}
	}

	public unsafe Vector3 RelativeOffsetAdjustment
	{
		get
		{
			Vector3 result = default(Vector3);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetRelativeOffsetAdjustment failed", num);
			}
			return result;
		}
	}

	public unsafe Vector2 RelativeSizeAdjustment
	{
		get
		{
			Vector2 result = default(Vector2);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetRelativeSizeAdjustment failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 6;

	public unsafe void SetParentForTransform(IVisual value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, MicroComRuntime.GetNativePointer(value));
		if (num != 0)
		{
			throw new COMException("SetParentForTransform failed", num);
		}
	}

	public unsafe void SetRelativeOffsetAdjustment(Vector3 value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, Vector3, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetRelativeOffsetAdjustment failed", num);
		}
	}

	public unsafe void SetRelativeSizeAdjustment(Vector2 value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, Vector2, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetRelativeSizeAdjustment failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IVisual2), new Guid("3052B611-56C3-4C3E-8BF3-F6E1AD473F06"), (IntPtr p, bool owns) => new __MicroComIVisual2Proxy(p, owns));
	}

	protected __MicroComIVisual2Proxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
