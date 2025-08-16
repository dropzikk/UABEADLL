using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIFileOpenDialogVTable : __MicroComIFileDialogVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetResultsDelegate(void* @this, void** ppenum);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetSelectedItemsDelegate(void* @this, void** ppsai);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetResults(void* @this, void** ppenum)
	{
		IFileOpenDialog fileOpenDialog = null;
		try
		{
			fileOpenDialog = (IFileOpenDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IShellItemArray results = fileOpenDialog.Results;
			*ppenum = MicroComRuntime.GetNativePointer(results, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileOpenDialog, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetSelectedItems(void* @this, void** ppsai)
	{
		IFileOpenDialog fileOpenDialog = null;
		try
		{
			fileOpenDialog = (IFileOpenDialog)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IShellItemArray selectedItems = fileOpenDialog.SelectedItems;
			*ppsai = MicroComRuntime.GetNativePointer(selectedItems, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(fileOpenDialog, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIFileOpenDialogVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&GetResults));
		AddMethod((delegate*<void*, void**, int>)(&GetSelectedItems));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IFileOpenDialog), new __MicroComIFileOpenDialogVTable().CreateVTable());
	}
}
