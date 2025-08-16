using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnStringProxy : MicroComProxyBase, IAvnString, IUnknown, IDisposable
{
	private string _managed;

	private byte[] _bytes;

	public unsafe string String
	{
		get
		{
			if (_managed == null)
			{
				void* ptr = Pointer();
				if (ptr == null)
				{
					return null;
				}
				_managed = Encoding.UTF8.GetString((byte*)ptr, Length());
			}
			return _managed;
		}
	}

	public unsafe byte[] Bytes
	{
		get
		{
			if (_bytes == null)
			{
				_bytes = new byte[Length()];
				Marshal.Copy(new IntPtr(Pointer()), _bytes, 0, _bytes.Length);
			}
			return _bytes;
		}
	}

	protected override int VTableSize => base.VTableSize + 2;

	public override string ToString()
	{
		return String;
	}

	public unsafe void* Pointer()
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("Pointer failed", num);
		}
		return result;
	}

	public unsafe int Length()
	{
		int result = 0;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("Length failed", num);
		}
		return result;
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnString), new Guid("233e094f-9b9f-44a3-9a6e-6948bbdd9fb1"), (IntPtr p, bool owns) => new __MicroComIAvnStringProxy(p, owns));
	}

	protected __MicroComIAvnStringProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
