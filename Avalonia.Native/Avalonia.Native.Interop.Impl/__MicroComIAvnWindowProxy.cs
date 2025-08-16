using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnWindowProxy : __MicroComIAvnWindowBaseProxy, IAvnWindow, IAvnWindowBase, IUnknown, IDisposable
{
	public unsafe AvnWindowState WindowState
	{
		get
		{
			AvnWindowState result = AvnWindowState.Normal;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetWindowState failed", num);
			}
			return result;
		}
	}

	public unsafe double ExtendTitleBarHeight
	{
		get
		{
			double result = 0.0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 11])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetExtendTitleBarHeight failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 13;

	public unsafe void SetEnabled(int enable)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize])(base.PPV, enable);
		if (num != 0)
		{
			throw new COMException("SetEnabled failed", num);
		}
	}

	public unsafe void SetParent(IAvnWindow parent)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, MicroComRuntime.GetNativePointer(parent));
		if (num != 0)
		{
			throw new COMException("SetParent failed", num);
		}
	}

	public unsafe void SetCanResize(int value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetCanResize failed", num);
		}
	}

	public unsafe void SetDecorations(SystemDecorations value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, SystemDecorations, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetDecorations failed", num);
		}
	}

	public unsafe void SetTitle(string utf8Title)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(utf8Title) + 1];
		Encoding.UTF8.GetBytes(utf8Title, 0, utf8Title.Length, array, 0);
		int num;
		fixed (byte* ptr = array)
		{
			num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, ptr);
		}
		if (num != 0)
		{
			throw new COMException("SetTitle failed", num);
		}
	}

	public unsafe void SetTitleBarColor(AvnColor color)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, AvnColor, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, color);
		if (num != 0)
		{
			throw new COMException("SetTitleBarColor failed", num);
		}
	}

	public unsafe void SetWindowState(AvnWindowState state)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, AvnWindowState, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, state);
		if (num != 0)
		{
			throw new COMException("SetWindowState failed", num);
		}
	}

	public unsafe void TakeFocusFromChildren()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV);
		if (num != 0)
		{
			throw new COMException("TakeFocusFromChildren failed", num);
		}
	}

	public unsafe void SetExtendClientArea(int enable)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize + 9])(base.PPV, enable);
		if (num != 0)
		{
			throw new COMException("SetExtendClientArea failed", num);
		}
	}

	public unsafe void SetExtendClientAreaHints(AvnExtendClientAreaChromeHints hints)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, AvnExtendClientAreaChromeHints, int>)(*base.PPV)[base.VTableSize + 10])(base.PPV, hints);
		if (num != 0)
		{
			throw new COMException("SetExtendClientAreaHints failed", num);
		}
	}

	public unsafe void SetExtendTitleBarHeight(double value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, double, int>)(*base.PPV)[base.VTableSize + 12])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetExtendTitleBarHeight failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnWindow), new Guid("cab661de-49d6-4ead-b59c-eac9b2b6c28d"), (IntPtr p, bool owns) => new __MicroComIAvnWindowProxy(p, owns));
	}

	protected __MicroComIAvnWindowProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
