using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnTextInputMethodProxy : MicroComProxyBase, IAvnTextInputMethod, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 4;

	public unsafe void SetClient(IAvnTextInputMethodClient client)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, MicroComRuntime.GetNativePointer(client));
		if (num != 0)
		{
			throw new COMException("SetClient failed", num);
		}
	}

	public unsafe void Reset()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 1])(base.PPV);
	}

	public unsafe void SetCursorRect(AvnRect rect)
	{
		((delegate* unmanaged[Stdcall]<void*, AvnRect, void>)(*base.PPV)[base.VTableSize + 2])(base.PPV, rect);
	}

	public unsafe void SetSurroundingText(string text, int anchorOffset, int cursorOffset)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(text) + 1];
		Encoding.UTF8.GetBytes(text, 0, text.Length, array, 0);
		fixed (byte* ptr = array)
		{
			((delegate* unmanaged[Stdcall]<void*, void*, int, int, void>)(*base.PPV)[base.VTableSize + 3])(base.PPV, ptr, anchorOffset, cursorOffset);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnTextInputMethod), new Guid("1382a29f-e260-4c7a-b83f-c99fc72e27c2"), (IntPtr p, bool owns) => new __MicroComIAvnTextInputMethodProxy(p, owns));
	}

	protected __MicroComIAvnTextInputMethodProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
