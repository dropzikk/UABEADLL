using System;
using System.Runtime.CompilerServices;
using System.Text;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnTextInputMethodClientProxy : MicroComProxyBase, IAvnTextInputMethodClient, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 2;

	public unsafe void SetPreeditText(string preeditText)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(preeditText) + 1];
		Encoding.UTF8.GetBytes(preeditText, 0, preeditText.Length, array, 0);
		fixed (byte* ptr = array)
		{
			((delegate* unmanaged[Stdcall]<void*, void*, void>)(*base.PPV)[base.VTableSize])(base.PPV, ptr);
		}
	}

	public unsafe void SelectInSurroundingText(int start, int length)
	{
		((delegate* unmanaged[Stdcall]<void*, int, int, void>)(*base.PPV)[base.VTableSize + 1])(base.PPV, start, length);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnTextInputMethodClient), new Guid("f2079145-a2d9-42b8-a85e-2732e3c2b055"), (IntPtr p, bool owns) => new __MicroComIAvnTextInputMethodClientProxy(p, owns));
	}

	protected __MicroComIAvnTextInputMethodClientProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
