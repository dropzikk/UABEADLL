using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnCursorProxy : MicroComProxyBase, IAvnCursor, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize;

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnCursor), new Guid("3f998545-f027-4d4d-bd2a-1a80926d984e"), (IntPtr p, bool owns) => new __MicroComIAvnCursorProxy(p, owns));
	}

	protected __MicroComIAvnCursorProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
