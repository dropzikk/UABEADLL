using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIShellItemArrayVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int BindToHandlerDelegate(void* @this, void* pbc, Guid* bhid, Guid* riid, void** ppvOut);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetPropertyStoreDelegate(void* @this, ushort flags, Guid* riid, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetPropertyDescriptionListDelegate(void* @this, void* keyType, Guid* riid, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetAttributesDelegate(void* @this, int AttribFlags, ushort sfgaoMask, ushort* psfgaoAttribs);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetCountDelegate(void* @this, int* pdwNumItems);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetItemAtDelegate(void* @this, int dwIndex, void** ppsi);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int EnumItemsDelegate(void* @this, void** ppenumShellItems);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int BindToHandler(void* @this, void* pbc, Guid* bhid, Guid* riid, void** ppvOut)
	{
		IShellItemArray shellItemArray = null;
		try
		{
			shellItemArray = (IShellItemArray)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = shellItemArray.BindToHandler(pbc, bhid, riid);
			*ppvOut = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(shellItemArray, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetPropertyStore(void* @this, ushort flags, Guid* riid, void** ppv)
	{
		IShellItemArray shellItemArray = null;
		try
		{
			shellItemArray = (IShellItemArray)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* propertyStore = shellItemArray.GetPropertyStore(flags, riid);
			*ppv = propertyStore;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(shellItemArray, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetPropertyDescriptionList(void* @this, void* keyType, Guid* riid, void** ppv)
	{
		IShellItemArray shellItemArray = null;
		try
		{
			shellItemArray = (IShellItemArray)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* propertyDescriptionList = shellItemArray.GetPropertyDescriptionList(keyType, riid);
			*ppv = propertyDescriptionList;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(shellItemArray, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetAttributes(void* @this, int AttribFlags, ushort sfgaoMask, ushort* psfgaoAttribs)
	{
		IShellItemArray shellItemArray = null;
		try
		{
			shellItemArray = (IShellItemArray)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ushort attributes = shellItemArray.GetAttributes(AttribFlags, sfgaoMask);
			*psfgaoAttribs = attributes;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(shellItemArray, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetCount(void* @this, int* pdwNumItems)
	{
		IShellItemArray shellItemArray = null;
		try
		{
			shellItemArray = (IShellItemArray)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int count = shellItemArray.Count;
			*pdwNumItems = count;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(shellItemArray, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetItemAt(void* @this, int dwIndex, void** ppsi)
	{
		IShellItemArray shellItemArray = null;
		try
		{
			shellItemArray = (IShellItemArray)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IShellItem itemAt = shellItemArray.GetItemAt(dwIndex);
			*ppsi = MicroComRuntime.GetNativePointer(itemAt, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(shellItemArray, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int EnumItems(void* @this, void** ppenumShellItems)
	{
		IShellItemArray shellItemArray = null;
		try
		{
			shellItemArray = (IShellItemArray)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = shellItemArray.EnumItems();
			*ppenumShellItems = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(shellItemArray, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIShellItemArrayVTable()
	{
		AddMethod((delegate*<void*, void*, Guid*, Guid*, void**, int>)(&BindToHandler));
		AddMethod((delegate*<void*, ushort, Guid*, void**, int>)(&GetPropertyStore));
		AddMethod((delegate*<void*, void*, Guid*, void**, int>)(&GetPropertyDescriptionList));
		AddMethod((delegate*<void*, int, ushort, ushort*, int>)(&GetAttributes));
		AddMethod((delegate*<void*, int*, int>)(&GetCount));
		AddMethod((delegate*<void*, int, void**, int>)(&GetItemAt));
		AddMethod((delegate*<void*, void**, int>)(&EnumItems));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IShellItemArray), new __MicroComIShellItemArrayVTable().CreateVTable());
	}
}
