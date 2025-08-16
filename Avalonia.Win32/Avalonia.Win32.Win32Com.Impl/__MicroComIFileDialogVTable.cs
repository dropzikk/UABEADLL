using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIFileDialogVTable : __MicroComIModalWindowVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetFileTypesDelegate(void* @this, ushort cFileTypes, void* rgFilterSpec);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetFileTypeIndexDelegate(void* @this, ushort iFileType);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetFileTypeIndexDelegate(void* @this, ushort* piFileType);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int AdviseDelegate(void* @this, void* pfde, int* pdwCookie);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int UnadviseDelegate(void* @this, int dwCookie);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetOptionsDelegate(void* @this, FILEOPENDIALOGOPTIONS fos);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetOptionsDelegate(void* @this, FILEOPENDIALOGOPTIONS* pfos);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetDefaultFolderDelegate(void* @this, void* psi);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetFolderDelegate(void* @this, void* psi);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetFolderDelegate(void* @this, void** ppsi);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetCurrentSelectionDelegate(void* @this, void** ppsi);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetFileNameDelegate(void* @this, char* pszName);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetFileNameDelegate(void* @this, char** pszName);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetTitleDelegate(void* @this, char* pszTitle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetOkButtonLabelDelegate(void* @this, char* pszText);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetFileNameLabelDelegate(void* @this, char* pszLabel);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetResultDelegate(void* @this, void** ppsi);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int AddPlaceDelegate(void* @this, void* psi, int fdap);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetDefaultExtensionDelegate(void* @this, char* pszDefaultExtension);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CloseDelegate(void* @this, int hr);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetClientGuidDelegate(void* @this, Guid* guid);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ClearClientDataDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetFilterDelegate(void* @this, void* pFilter);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetFileTypes(void* @this, ushort cFileTypes, void* rgFilterSpec)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			fileDialog.SetFileTypes(cFileTypes, rgFilterSpec);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetFileTypeIndex(void* @this, ushort iFileType)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			fileDialog.SetFileTypeIndex(iFileType);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetFileTypeIndex(void* @this, ushort* piFileType)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ushort fileTypeIndex = fileDialog.FileTypeIndex;
			*piFileType = fileTypeIndex;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Advise(void* @this, void* pfde, int* pdwCookie)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int num = fileDialog.Advise(pfde);
			*pdwCookie = num;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Unadvise(void* @this, int dwCookie)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			fileDialog.Unadvise(dwCookie);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetOptions(void* @this, FILEOPENDIALOGOPTIONS fos)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			fileDialog.SetOptions(fos);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetOptions(void* @this, FILEOPENDIALOGOPTIONS* pfos)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			FILEOPENDIALOGOPTIONS options = fileDialog.Options;
			*pfos = options;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetDefaultFolder(void* @this, void* psi)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			fileDialog.SetDefaultFolder(MicroComRuntime.CreateProxyOrNullFor<IShellItem>(psi, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetFolder(void* @this, void* psi)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			fileDialog.SetFolder(MicroComRuntime.CreateProxyOrNullFor<IShellItem>(psi, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetFolder(void* @this, void** ppsi)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IShellItem folder = fileDialog.Folder;
			*ppsi = MicroComRuntime.GetNativePointer(folder, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetCurrentSelection(void* @this, void** ppsi)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IShellItem currentSelection = fileDialog.CurrentSelection;
			*ppsi = MicroComRuntime.GetNativePointer(currentSelection, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetFileName(void* @this, char* pszName)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			fileDialog.SetFileName(pszName);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetFileName(void* @this, char** pszName)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			char* fileName = fileDialog.FileName;
			*pszName = fileName;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetTitle(void* @this, char* pszTitle)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			fileDialog.SetTitle(pszTitle);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetOkButtonLabel(void* @this, char* pszText)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			fileDialog.SetOkButtonLabel(pszText);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetFileNameLabel(void* @this, char* pszLabel)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			fileDialog.SetFileNameLabel(pszLabel);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetResult(void* @this, void** ppsi)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IShellItem result = fileDialog.Result;
			*ppsi = MicroComRuntime.GetNativePointer(result, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int AddPlace(void* @this, void* psi, int fdap)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			fileDialog.AddPlace(MicroComRuntime.CreateProxyOrNullFor<IShellItem>(psi, ownsHandle: false), fdap);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetDefaultExtension(void* @this, char* pszDefaultExtension)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			fileDialog.SetDefaultExtension(pszDefaultExtension);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Close(void* @this, int hr)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			fileDialog.Close(hr);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetClientGuid(void* @this, Guid* guid)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			fileDialog.SetClientGuid(guid);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ClearClientData(void* @this)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			fileDialog.ClearClientData();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetFilter(void* @this, void* pFilter)
	{
		IFileDialog fileDialog = null;
		try
		{
			fileDialog = (IFileDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			fileDialog.SetFilter(pFilter);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileDialog, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIFileDialogVTable()
	{
		AddMethod((delegate*<void*, ushort, void*, int>)(&SetFileTypes));
		AddMethod((delegate*<void*, ushort, int>)(&SetFileTypeIndex));
		AddMethod((delegate*<void*, ushort*, int>)(&GetFileTypeIndex));
		AddMethod((delegate*<void*, void*, int*, int>)(&Advise));
		AddMethod((delegate*<void*, int, int>)(&Unadvise));
		AddMethod((delegate*<void*, FILEOPENDIALOGOPTIONS, int>)(&SetOptions));
		AddMethod((delegate*<void*, FILEOPENDIALOGOPTIONS*, int>)(&GetOptions));
		AddMethod((delegate*<void*, void*, int>)(&SetDefaultFolder));
		AddMethod((delegate*<void*, void*, int>)(&SetFolder));
		AddMethod((delegate*<void*, void**, int>)(&GetFolder));
		AddMethod((delegate*<void*, void**, int>)(&GetCurrentSelection));
		AddMethod((delegate*<void*, char*, int>)(&SetFileName));
		AddMethod((delegate*<void*, char**, int>)(&GetFileName));
		AddMethod((delegate*<void*, char*, int>)(&SetTitle));
		AddMethod((delegate*<void*, char*, int>)(&SetOkButtonLabel));
		AddMethod((delegate*<void*, char*, int>)(&SetFileNameLabel));
		AddMethod((delegate*<void*, void**, int>)(&GetResult));
		AddMethod((delegate*<void*, void*, int, int>)(&AddPlace));
		AddMethod((delegate*<void*, char*, int>)(&SetDefaultExtension));
		AddMethod((delegate*<void*, int, int>)(&Close));
		AddMethod((delegate*<void*, Guid*, int>)(&SetClientGuid));
		AddMethod((delegate*<void*, int>)(&ClearClientData));
		AddMethod((delegate*<void*, void*, int>)(&SetFilter));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IFileDialog), new __MicroComIFileDialogVTable().CreateVTable());
	}
}
