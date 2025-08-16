using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIVisualCollectionProxy : __MicroComIInspectableProxy, IVisualCollection, IInspectable, IUnknown, IDisposable
{
	public unsafe int Count
	{
		get
		{
			int result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetCount failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 7;

	public unsafe void InsertAbove(IVisual newChild, IVisual sibling)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, MicroComRuntime.GetNativePointer(newChild), MicroComRuntime.GetNativePointer(sibling));
		if (num != 0)
		{
			throw new COMException("InsertAbove failed", num);
		}
	}

	public unsafe void InsertAtBottom(IVisual newChild)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, MicroComRuntime.GetNativePointer(newChild));
		if (num != 0)
		{
			throw new COMException("InsertAtBottom failed", num);
		}
	}

	public unsafe void InsertAtTop(IVisual newChild)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, MicroComRuntime.GetNativePointer(newChild));
		if (num != 0)
		{
			throw new COMException("InsertAtTop failed", num);
		}
	}

	public unsafe void InsertBelow(IVisual newChild, IVisual sibling)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, MicroComRuntime.GetNativePointer(newChild), MicroComRuntime.GetNativePointer(sibling));
		if (num != 0)
		{
			throw new COMException("InsertBelow failed", num);
		}
	}

	public unsafe void Remove(IVisual child)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, MicroComRuntime.GetNativePointer(child));
		if (num != 0)
		{
			throw new COMException("Remove failed", num);
		}
	}

	public unsafe void RemoveAll()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV);
		if (num != 0)
		{
			throw new COMException("RemoveAll failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IVisualCollection), new Guid("8B745505-FD3E-4A98-84A8-E949468C6BCB"), (IntPtr p, bool owns) => new __MicroComIVisualCollectionProxy(p, owns));
	}

	protected __MicroComIVisualCollectionProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
