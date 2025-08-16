using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnAutomationPeerArrayProxy : MicroComProxyBase, IAvnAutomationPeerArray, IUnknown, IDisposable
{
	public unsafe uint Count => ((delegate* unmanaged[Stdcall]<void*, uint>)(*base.PPV)[base.VTableSize])(base.PPV);

	protected override int VTableSize => base.VTableSize + 2;

	public unsafe IAvnAutomationPeer Get(uint index)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, uint, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, index, &pObject);
		if (num != 0)
		{
			throw new COMException("Get failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnAutomationPeer>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnAutomationPeerArray), new Guid("b00af5da-78af-4b33-bfff-4ce13a6239a9"), (IntPtr p, bool owns) => new __MicroComIAvnAutomationPeerArrayProxy(p, owns));
	}

	protected __MicroComIAvnAutomationPeerArrayProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
