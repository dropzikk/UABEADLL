using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnStringArrayProxy : MicroComProxyBase, IAvnStringArray, IUnknown, IDisposable
{
	public unsafe uint Count => ((delegate* unmanaged[Stdcall]<void*, uint>)(*base.PPV)[base.VTableSize])(base.PPV);

	protected override int VTableSize => base.VTableSize + 2;

	public string[] ToStringArray()
	{
		string[] array = new string[Count];
		for (uint num = 0u; num < array.Length; num++)
		{
			using IAvnString avnString = Get(num);
			array[num] = avnString.String;
		}
		return array;
	}

	public unsafe IAvnString Get(uint index)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, uint, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, index, &pObject);
		if (num != 0)
		{
			throw new COMException("Get failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnString>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnStringArray), new Guid("5142bb41-66ab-49e7-bb37-cd079c000f27"), (IntPtr p, bool owns) => new __MicroComIAvnStringArrayProxy(p, owns));
	}

	protected __MicroComIAvnStringArrayProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
