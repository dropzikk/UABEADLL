using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIUISettings3Proxy : __MicroComIInspectableProxy, IUISettings3, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe WinRTColor GetColorValue(UIColorType desiredColor)
	{
		WinRTColor result = default(WinRTColor);
		int num = ((delegate* unmanaged[Stdcall]<void*, UIColorType, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, desiredColor, &result);
		if (num != 0)
		{
			throw new COMException("GetColorValue failed", num);
		}
		return result;
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IUISettings3), new Guid("03021BE4-5254-4781-8194-5168F7D06D7B"), (IntPtr p, bool owns) => new __MicroComIUISettings3Proxy(p, owns));
	}

	protected __MicroComIUISettings3Proxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
