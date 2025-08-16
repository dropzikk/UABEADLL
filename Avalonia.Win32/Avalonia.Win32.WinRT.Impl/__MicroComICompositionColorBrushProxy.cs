using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionColorBrushProxy : __MicroComIInspectableProxy, ICompositionColorBrush, IInspectable, IUnknown, IDisposable
{
	public unsafe WinRTColor Color
	{
		get
		{
			WinRTColor result = default(WinRTColor);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetColor failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 2;

	public unsafe void SetColor(WinRTColor value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, WinRTColor, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetColor failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionColorBrush), new Guid("2B264C5E-BF35-4831-8642-CF70C20FFF2F"), (IntPtr p, bool owns) => new __MicroComICompositionColorBrushProxy(p, owns));
	}

	protected __MicroComICompositionColorBrushProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
