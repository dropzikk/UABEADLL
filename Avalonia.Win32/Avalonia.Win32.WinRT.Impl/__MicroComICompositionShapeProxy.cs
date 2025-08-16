using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionShapeProxy : __MicroComIInspectableProxy, ICompositionShape, IInspectable, IUnknown, IDisposable
{
	public unsafe Vector2 CenterPoint
	{
		get
		{
			Vector2 result = default(Vector2);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetCenterPoint failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 2;

	public unsafe void SetCenterPoint(Vector2 value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, Vector2, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetCenterPoint failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionShape), new Guid("B47CE2F7-9A88-42C4-9E87-2E500CA8688C"), (IntPtr p, bool owns) => new __MicroComICompositionShapeProxy(p, owns));
	}

	protected __MicroComICompositionShapeProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
