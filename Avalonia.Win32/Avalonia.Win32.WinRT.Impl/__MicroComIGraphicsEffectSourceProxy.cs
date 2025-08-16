using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIGraphicsEffectSourceProxy : __MicroComIInspectableProxy, IGraphicsEffectSource, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize;

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IGraphicsEffectSource), new Guid("2D8F9DDC-4339-4EB9-9216-F9DEB75658A2"), (IntPtr p, bool owns) => new __MicroComIGraphicsEffectSourceProxy(p, owns));
	}

	protected __MicroComIGraphicsEffectSourceProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
