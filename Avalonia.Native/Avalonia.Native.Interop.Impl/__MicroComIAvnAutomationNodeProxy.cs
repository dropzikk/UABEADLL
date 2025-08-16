using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnAutomationNodeProxy : MicroComProxyBase, IAvnAutomationNode, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 4;

	public new unsafe void Dispose()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize])(base.PPV);
	}

	public unsafe void ChildrenChanged()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 1])(base.PPV);
	}

	public unsafe void PropertyChanged(AvnAutomationProperty property)
	{
		((delegate* unmanaged[Stdcall]<void*, AvnAutomationProperty, void>)(*base.PPV)[base.VTableSize + 2])(base.PPV, property);
	}

	public unsafe void FocusChanged()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 3])(base.PPV);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnAutomationNode), new Guid("004dc40b-e435-49dc-bac5-6272ee35382a"), (IntPtr p, bool owns) => new __MicroComIAvnAutomationNodeProxy(p, owns));
	}

	protected __MicroComIAvnAutomationNodeProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
