using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIObjectVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetPrivateDataDelegate(void* @this, Guid* Name, ushort DataSize, void** pData);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetPrivateDataInterfaceDelegate(void* @this, Guid* Name, void* pUnknown);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetPrivateDataDelegate(void* @this, Guid* Name, ushort* pDataSize, void** pData);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetParentDelegate(void* @this, Guid* riid, void** ppParent);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetPrivateData(void* @this, Guid* Name, ushort DataSize, void** pData)
	{
		IDXGIObject iDXGIObject = null;
		try
		{
			iDXGIObject = (IDXGIObject)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIObject.SetPrivateData(Name, DataSize, pData);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIObject, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetPrivateDataInterface(void* @this, Guid* Name, void* pUnknown)
	{
		IDXGIObject iDXGIObject = null;
		try
		{
			iDXGIObject = (IDXGIObject)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIObject.SetPrivateDataInterface(Name, MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pUnknown, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIObject, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetPrivateData(void* @this, Guid* Name, ushort* pDataSize, void** pData)
	{
		IDXGIObject iDXGIObject = null;
		try
		{
			iDXGIObject = (IDXGIObject)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* privateData = iDXGIObject.GetPrivateData(Name, pDataSize);
			*pData = privateData;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIObject, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetParent(void* @this, Guid* riid, void** ppParent)
	{
		IDXGIObject iDXGIObject = null;
		try
		{
			iDXGIObject = (IDXGIObject)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* parent = iDXGIObject.GetParent(riid);
			*ppParent = parent;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIObject, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIDXGIObjectVTable()
	{
		AddMethod((delegate*<void*, Guid*, ushort, void**, int>)(&SetPrivateData));
		AddMethod((delegate*<void*, Guid*, void*, int>)(&SetPrivateDataInterface));
		AddMethod((delegate*<void*, Guid*, ushort*, void**, int>)(&GetPrivateData));
		AddMethod((delegate*<void*, Guid*, void**, int>)(&GetParent));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDXGIObject), new __MicroComIDXGIObjectVTable().CreateVTable());
	}
}
