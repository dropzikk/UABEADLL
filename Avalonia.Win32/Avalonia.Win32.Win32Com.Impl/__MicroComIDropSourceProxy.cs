using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIDropSourceProxy : MicroComProxyBase, IDropSource, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 2;

	public unsafe int QueryContinueDrag(int fEscapePressed, int grfKeyState)
	{
		return ((delegate* unmanaged[Stdcall]<void*, int, int, int>)(*base.PPV)[base.VTableSize])(base.PPV, fEscapePressed, grfKeyState);
	}

	public unsafe int GiveFeedback(DropEffect dwEffect)
	{
		return ((delegate* unmanaged[Stdcall]<void*, DropEffect, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, dwEffect);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDropSource), new Guid("00000121-0000-0000-C000-000000000046"), (IntPtr p, bool owns) => new __MicroComIDropSourceProxy(p, owns));
	}

	protected __MicroComIDropSourceProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
