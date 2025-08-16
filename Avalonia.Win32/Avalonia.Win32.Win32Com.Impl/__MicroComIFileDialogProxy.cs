using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIFileDialogProxy : __MicroComIModalWindowProxy, IFileDialog, IModalWindow, IUnknown, IDisposable
{
	public unsafe ushort FileTypeIndex
	{
		get
		{
			ushort result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetFileTypeIndex failed", num);
			}
			return result;
		}
	}

	public unsafe FILEOPENDIALOGOPTIONS Options
	{
		get
		{
			FILEOPENDIALOGOPTIONS result = (FILEOPENDIALOGOPTIONS)0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetOptions failed", num);
			}
			return result;
		}
	}

	public unsafe IShellItem Folder
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 9])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetFolder failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IShellItem>(pObject, ownsHandle: true);
		}
	}

	public unsafe IShellItem CurrentSelection
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 10])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetCurrentSelection failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IShellItem>(pObject, ownsHandle: true);
		}
	}

	public unsafe char* FileName
	{
		get
		{
			char* result = default(char*);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 12])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetFileName failed", num);
			}
			return result;
		}
	}

	public unsafe IShellItem Result
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 16])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetResult failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IShellItem>(pObject, ownsHandle: true);
		}
	}

	protected override int VTableSize => base.VTableSize + 23;

	public unsafe void SetFileTypes(ushort cFileTypes, void* rgFilterSpec)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, ushort, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, cFileTypes, rgFilterSpec);
		if (num != 0)
		{
			throw new COMException("SetFileTypes failed", num);
		}
	}

	public unsafe void SetFileTypeIndex(ushort iFileType)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, ushort, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, iFileType);
		if (num != 0)
		{
			throw new COMException("SetFileTypeIndex failed", num);
		}
	}

	public unsafe int Advise(void* pfde)
	{
		int result = 0;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, pfde, &result);
		if (num != 0)
		{
			throw new COMException("Advise failed", num);
		}
		return result;
	}

	public unsafe void Unadvise(int dwCookie)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, dwCookie);
		if (num != 0)
		{
			throw new COMException("Unadvise failed", num);
		}
	}

	public unsafe void SetOptions(FILEOPENDIALOGOPTIONS fos)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, FILEOPENDIALOGOPTIONS, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, fos);
		if (num != 0)
		{
			throw new COMException("SetOptions failed", num);
		}
	}

	public unsafe void SetDefaultFolder(IShellItem psi)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, MicroComRuntime.GetNativePointer(psi));
		if (num != 0)
		{
			throw new COMException("SetDefaultFolder failed", num);
		}
	}

	public unsafe void SetFolder(IShellItem psi)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV, MicroComRuntime.GetNativePointer(psi));
		if (num != 0)
		{
			throw new COMException("SetFolder failed", num);
		}
	}

	public unsafe void SetFileName(char* pszName)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 11])(base.PPV, pszName);
		if (num != 0)
		{
			throw new COMException("SetFileName failed", num);
		}
	}

	public unsafe void SetTitle(char* pszTitle)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 13])(base.PPV, pszTitle);
		if (num != 0)
		{
			throw new COMException("SetTitle failed", num);
		}
	}

	public unsafe void SetOkButtonLabel(char* pszText)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 14])(base.PPV, pszText);
		if (num != 0)
		{
			throw new COMException("SetOkButtonLabel failed", num);
		}
	}

	public unsafe void SetFileNameLabel(char* pszLabel)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 15])(base.PPV, pszLabel);
		if (num != 0)
		{
			throw new COMException("SetFileNameLabel failed", num);
		}
	}

	public unsafe void AddPlace(IShellItem psi, int fdap)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int, int>)(*base.PPV)[base.VTableSize + 17])(base.PPV, MicroComRuntime.GetNativePointer(psi), fdap);
		if (num != 0)
		{
			throw new COMException("AddPlace failed", num);
		}
	}

	public unsafe void SetDefaultExtension(char* pszDefaultExtension)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 18])(base.PPV, pszDefaultExtension);
		if (num != 0)
		{
			throw new COMException("SetDefaultExtension failed", num);
		}
	}

	public unsafe void Close(int hr)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize + 19])(base.PPV, hr);
		if (num != 0)
		{
			throw new COMException("Close failed", num);
		}
	}

	public unsafe void SetClientGuid(Guid* guid)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 20])(base.PPV, guid);
		if (num != 0)
		{
			throw new COMException("SetClientGuid failed", num);
		}
	}

	public unsafe void ClearClientData()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 21])(base.PPV);
		if (num != 0)
		{
			throw new COMException("ClearClientData failed", num);
		}
	}

	public unsafe void SetFilter(void* pFilter)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 22])(base.PPV, pFilter);
		if (num != 0)
		{
			throw new COMException("SetFilter failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IFileDialog), new Guid("42F85136-DB7E-439C-85F1-E4075D135FC8"), (IntPtr p, bool owns) => new __MicroComIFileDialogProxy(p, owns));
	}

	protected __MicroComIFileDialogProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
