using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIVectorOfCompositionShapeProxy : __MicroComIInspectableProxy, IVectorOfCompositionShape, IInspectable, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 10;

	public unsafe void GetAt()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize])(base.PPV);
		if (num != 0)
		{
			throw new COMException("GetAt failed", num);
		}
	}

	public unsafe void GetSize()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV);
		if (num != 0)
		{
			throw new COMException("GetSize failed", num);
		}
	}

	public unsafe void GetView()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV);
		if (num != 0)
		{
			throw new COMException("GetView failed", num);
		}
	}

	public unsafe void IndexOf()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV);
		if (num != 0)
		{
			throw new COMException("IndexOf failed", num);
		}
	}

	public unsafe void SetAt()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV);
		if (num != 0)
		{
			throw new COMException("SetAt failed", num);
		}
	}

	public unsafe void InsertAt()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV);
		if (num != 0)
		{
			throw new COMException("InsertAt failed", num);
		}
	}

	public unsafe void RemoveAt()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV);
		if (num != 0)
		{
			throw new COMException("RemoveAt failed", num);
		}
	}

	public unsafe void Append(ICompositionShape value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, MicroComRuntime.GetNativePointer(value));
		if (num != 0)
		{
			throw new COMException("Append failed", num);
		}
	}

	public unsafe void RemoveAtEnd()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV);
		if (num != 0)
		{
			throw new COMException("RemoveAtEnd failed", num);
		}
	}

	public unsafe void Clear()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 9])(base.PPV);
		if (num != 0)
		{
			throw new COMException("Clear failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IVectorOfCompositionShape), new Guid("42d4219a-be1b-5091-8f1e-90270840fc2d"), (IntPtr p, bool owns) => new __MicroComIVectorOfCompositionShapeProxy(p, owns));
	}

	protected __MicroComIVectorOfCompositionShapeProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
