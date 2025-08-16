using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionClipProxy : __MicroComIInspectableProxy, ICompositionClip, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize;

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionClip), new Guid("1CCD2A52-CFC7-4ACE-9983-146BB8EB6A3C"), (IntPtr p, bool owns) => new __MicroComICompositionClipProxy(p, owns));
	}

	protected __MicroComICompositionClipProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
