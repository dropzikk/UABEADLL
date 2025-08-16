using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnPopupProxy : __MicroComIAvnWindowBaseProxy, IAvnPopup, IAvnWindowBase, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize;

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnPopup), new Guid("83e588f3-6981-4e48-9ea0-e1e569f79a91"), (IntPtr p, bool owns) => new __MicroComIAvnPopupProxy(p, owns));
	}

	protected __MicroComIAvnPopupProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
