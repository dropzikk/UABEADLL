using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnClipboardProxy : MicroComProxyBase, IAvnClipboard, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 7;

	public unsafe IAvnString GetText(string type)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(type) + 1];
		Encoding.UTF8.GetBytes(type, 0, type.Length, array, 0);
		void* pObject = null;
		int num;
		fixed (byte* ptr = array)
		{
			num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, ptr, &pObject);
		}
		if (num != 0)
		{
			throw new COMException("GetText failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnString>(pObject, ownsHandle: true);
	}

	public unsafe void SetText(string type, string utf8Text)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(type) + 1];
		Encoding.UTF8.GetBytes(type, 0, type.Length, array, 0);
		byte[] array2 = new byte[Encoding.UTF8.GetByteCount(utf8Text) + 1];
		Encoding.UTF8.GetBytes(utf8Text, 0, utf8Text.Length, array2, 0);
		int num;
		fixed (byte* ptr2 = array2)
		{
			fixed (byte* ptr = array)
			{
				num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, ptr, ptr2);
			}
		}
		if (num != 0)
		{
			throw new COMException("SetText failed", num);
		}
	}

	public unsafe IAvnStringArray ObtainFormats()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("ObtainFormats failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnStringArray>(pObject, ownsHandle: true);
	}

	public unsafe IAvnStringArray GetStrings(string type)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(type) + 1];
		Encoding.UTF8.GetBytes(type, 0, type.Length, array, 0);
		void* pObject = null;
		int num;
		fixed (byte* ptr = array)
		{
			num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, ptr, &pObject);
		}
		if (num != 0)
		{
			throw new COMException("GetStrings failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnStringArray>(pObject, ownsHandle: true);
	}

	public unsafe void SetBytes(string type, void* utf8Text, int len)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(type) + 1];
		Encoding.UTF8.GetBytes(type, 0, type.Length, array, 0);
		int num;
		fixed (byte* ptr = array)
		{
			num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, ptr, utf8Text, len);
		}
		if (num != 0)
		{
			throw new COMException("SetBytes failed", num);
		}
	}

	public unsafe IAvnString GetBytes(string type)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(type) + 1];
		Encoding.UTF8.GetBytes(type, 0, type.Length, array, 0);
		void* pObject = null;
		int num;
		fixed (byte* ptr = array)
		{
			num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, ptr, &pObject);
		}
		if (num != 0)
		{
			throw new COMException("GetBytes failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnString>(pObject, ownsHandle: true);
	}

	public unsafe void Clear()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV);
		if (num != 0)
		{
			throw new COMException("Clear failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnClipboard), new Guid("792b1bd4-76cc-46ea-bfd0-9d642154b1b3"), (IntPtr p, bool owns) => new __MicroComIAvnClipboardProxy(p, owns));
	}

	protected __MicroComIAvnClipboardProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
