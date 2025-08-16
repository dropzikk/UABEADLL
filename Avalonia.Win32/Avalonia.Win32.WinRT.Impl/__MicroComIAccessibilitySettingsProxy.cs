using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIAccessibilitySettingsProxy : __MicroComIInspectableProxy, IAccessibilitySettings, IInspectable, IUnknown, IDisposable
{
	public unsafe int HighContrast
	{
		get
		{
			int result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetHighContrast failed", num);
			}
			return result;
		}
	}

	public unsafe IntPtr HighContrastScheme
	{
		get
		{
			IntPtr result = default(IntPtr);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetHighContrastScheme failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 2;

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAccessibilitySettings), new Guid("FE0E8147-C4C0-4562-B962-1327B52AD5B9"), (IntPtr p, bool owns) => new __MicroComIAccessibilitySettingsProxy(p, owns));
	}

	protected __MicroComIAccessibilitySettingsProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
