using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnSystemDialogsVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void SelectFolderDialogDelegate(void* @this, void* parentWindowHandle, void* events, int allowMultiple, byte* title, byte* initialPath);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void OpenFileDialogDelegate(void* @this, void* parentWindowHandle, void* events, int allowMultiple, byte* title, byte* initialDirectory, byte* initialFile, byte* filters);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void SaveFileDialogDelegate(void* @this, void* parentWindowHandle, void* events, byte* title, byte* initialDirectory, byte* initialFile, byte* filters);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void SelectFolderDialog(void* @this, void* parentWindowHandle, void* events, int allowMultiple, byte* title, byte* initialPath)
	{
		IAvnSystemDialogs avnSystemDialogs = null;
		try
		{
			avnSystemDialogs = (IAvnSystemDialogs)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnSystemDialogs.SelectFolderDialog(MicroComRuntime.CreateProxyOrNullFor<IAvnWindow>(parentWindowHandle, ownsHandle: false), MicroComRuntime.CreateProxyOrNullFor<IAvnSystemDialogEvents>(events, ownsHandle: false), allowMultiple, (title == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(title)), (initialPath == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(initialPath)));
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnSystemDialogs, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void OpenFileDialog(void* @this, void* parentWindowHandle, void* events, int allowMultiple, byte* title, byte* initialDirectory, byte* initialFile, byte* filters)
	{
		IAvnSystemDialogs avnSystemDialogs = null;
		try
		{
			avnSystemDialogs = (IAvnSystemDialogs)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnSystemDialogs.OpenFileDialog(MicroComRuntime.CreateProxyOrNullFor<IAvnWindow>(parentWindowHandle, ownsHandle: false), MicroComRuntime.CreateProxyOrNullFor<IAvnSystemDialogEvents>(events, ownsHandle: false), allowMultiple, (title == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(title)), (initialDirectory == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(initialDirectory)), (initialFile == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(initialFile)), (filters == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(filters)));
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnSystemDialogs, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void SaveFileDialog(void* @this, void* parentWindowHandle, void* events, byte* title, byte* initialDirectory, byte* initialFile, byte* filters)
	{
		IAvnSystemDialogs avnSystemDialogs = null;
		try
		{
			avnSystemDialogs = (IAvnSystemDialogs)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnSystemDialogs.SaveFileDialog(MicroComRuntime.CreateProxyOrNullFor<IAvnWindow>(parentWindowHandle, ownsHandle: false), MicroComRuntime.CreateProxyOrNullFor<IAvnSystemDialogEvents>(events, ownsHandle: false), (title == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(title)), (initialDirectory == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(initialDirectory)), (initialFile == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(initialFile)), (filters == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(filters)));
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnSystemDialogs, e);
		}
	}

	protected unsafe __MicroComIAvnSystemDialogsVTable()
	{
		AddMethod((delegate*<void*, void*, void*, int, byte*, byte*, void>)(&SelectFolderDialog));
		AddMethod((delegate*<void*, void*, void*, int, byte*, byte*, byte*, byte*, void>)(&OpenFileDialog));
		AddMethod((delegate*<void*, void*, void*, byte*, byte*, byte*, byte*, void>)(&SaveFileDialog));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnSystemDialogs), new __MicroComIAvnSystemDialogsVTable().CreateVTable());
	}
}
