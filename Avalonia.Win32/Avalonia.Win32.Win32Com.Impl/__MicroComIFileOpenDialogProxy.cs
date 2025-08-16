using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIFileOpenDialogProxy : __MicroComIFileDialogProxy, IFileOpenDialog, IFileDialog, IModalWindow, IUnknown, IDisposable
{
	public unsafe IShellItemArray Results
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetResults failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IShellItemArray>(pObject, ownsHandle: true);
		}
	}

	public unsafe IShellItemArray SelectedItems
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetSelectedItems failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IShellItemArray>(pObject, ownsHandle: true);
		}
	}

	protected override int VTableSize => base.VTableSize + 2;

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IFileOpenDialog), new Guid("D57C7288-D4AD-4768-BE02-9D969532D960"), (IntPtr p, bool owns) => new __MicroComIFileOpenDialogProxy(p, owns));
	}

	protected __MicroComIFileOpenDialogProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
