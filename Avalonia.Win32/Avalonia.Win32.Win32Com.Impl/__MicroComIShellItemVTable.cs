using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIShellItemVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int BindToHandlerDelegate(void* @this, void* pbc, Guid* bhid, Guid* riid, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetParentDelegate(void* @this, void** ppsi);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetDisplayNameDelegate(void* @this, uint sigdnName, char** ppszName);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetAttributesDelegate(void* @this, uint sfgaoMask, uint* psfgaoAttribs);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CompareDelegate(void* @this, void* psi, uint hint, int* piOrder);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int BindToHandler(void* @this, void* pbc, Guid* bhid, Guid* riid, void** ppv)
	{
		IShellItem shellItem = null;
		try
		{
			shellItem = (IShellItem)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = shellItem.BindToHandler(pbc, bhid, riid);
			*ppv = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(shellItem, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetParent(void* @this, void** ppsi)
	{
		IShellItem shellItem = null;
		try
		{
			shellItem = (IShellItem)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IShellItem parent = shellItem.Parent;
			*ppsi = MicroComRuntime.GetNativePointer(parent, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(shellItem, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetDisplayName(void* @this, uint sigdnName, char** ppszName)
	{
		IShellItem shellItem = null;
		try
		{
			shellItem = (IShellItem)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			char* displayName = shellItem.GetDisplayName(sigdnName);
			*ppszName = displayName;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(shellItem, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetAttributes(void* @this, uint sfgaoMask, uint* psfgaoAttribs)
	{
		IShellItem shellItem = null;
		try
		{
			shellItem = (IShellItem)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			uint attributes = shellItem.GetAttributes(sfgaoMask);
			*psfgaoAttribs = attributes;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(shellItem, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Compare(void* @this, void* psi, uint hint, int* piOrder)
	{
		IShellItem shellItem = null;
		try
		{
			shellItem = (IShellItem)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int num = shellItem.Compare(MicroComRuntime.CreateProxyOrNullFor<IShellItem>(psi, ownsHandle: false), hint);
			*piOrder = num;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(shellItem, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIShellItemVTable()
	{
		AddMethod((delegate*<void*, void*, Guid*, Guid*, void**, int>)(&BindToHandler));
		AddMethod((delegate*<void*, void**, int>)(&GetParent));
		AddMethod((delegate*<void*, uint, char**, int>)(&GetDisplayName));
		AddMethod((delegate*<void*, uint, uint*, int>)(&GetAttributes));
		AddMethod((delegate*<void*, void*, uint, int*, int>)(&Compare));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IShellItem), new __MicroComIShellItemVTable().CreateVTable());
	}
}
