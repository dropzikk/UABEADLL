using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIDispatcherQueueProxy : __MicroComIInspectableProxy, IDispatcherQueue, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize;

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDispatcherQueue), new Guid("603E88E4-A338-4FFE-A457-A5CFB9CEB899"), (IntPtr p, bool owns) => new __MicroComIDispatcherQueueProxy(p, owns));
	}

	protected __MicroComIDispatcherQueueProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
