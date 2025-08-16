using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnMetalDisplayProxy : MicroComProxyBase, IAvnMetalDisplay, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe IAvnMetalDevice CreateDevice()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateDevice failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnMetalDevice>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnMetalDisplay), new Guid("da291767-4db3-4598-893d-09ecaa23893f"), (IntPtr p, bool owns) => new __MicroComIAvnMetalDisplayProxy(p, owns));
	}

	protected __MicroComIAvnMetalDisplayProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
