using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIDataObjectVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate uint GetDataDelegate(void* @this, FORMATETC* pformatetcIn, STGMEDIUM* pmedium);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate uint GetDataHereDelegate(void* @this, FORMATETC* pformatetc, STGMEDIUM* pmedium);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate uint QueryGetDataDelegate(void* @this, FORMATETC* pformatetc);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetCanonicalFormatEtcDelegate(void* @this, FORMATETC* pformatectIn, FORMATETC* pformatetcOut);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate uint SetDataDelegate(void* @this, FORMATETC* pformatetc, STGMEDIUM* pmedium, int fRelease);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int EnumFormatEtcDelegate(void* @this, int dwDirection, void** ppenumFormatEtc);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int DAdviseDelegate(void* @this, FORMATETC* pformatetc, int advf, void* pAdvSink, int* pdwConnection);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int DUnadviseDelegate(void* @this, int dwConnection);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int EnumDAdviseDelegate(void* @this, void** ppenumAdvise);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static uint GetData(void* @this, FORMATETC* pformatetcIn, STGMEDIUM* pmedium)
	{
		IDataObject dataObject = null;
		try
		{
			dataObject = (IDataObject)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return dataObject.GetData(pformatetcIn, pmedium);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(dataObject, e);
			return 0u;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static uint GetDataHere(void* @this, FORMATETC* pformatetc, STGMEDIUM* pmedium)
	{
		IDataObject dataObject = null;
		try
		{
			dataObject = (IDataObject)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return dataObject.GetDataHere(pformatetc, pmedium);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(dataObject, e);
			return 0u;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static uint QueryGetData(void* @this, FORMATETC* pformatetc)
	{
		IDataObject dataObject = null;
		try
		{
			dataObject = (IDataObject)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return dataObject.QueryGetData(pformatetc);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(dataObject, e);
			return 0u;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetCanonicalFormatEtc(void* @this, FORMATETC* pformatectIn, FORMATETC* pformatetcOut)
	{
		IDataObject dataObject = null;
		try
		{
			dataObject = (IDataObject)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			FORMATETC canonicalFormatEtc = dataObject.GetCanonicalFormatEtc(pformatectIn);
			*pformatetcOut = canonicalFormatEtc;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(dataObject, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static uint SetData(void* @this, FORMATETC* pformatetc, STGMEDIUM* pmedium, int fRelease)
	{
		IDataObject dataObject = null;
		try
		{
			dataObject = (IDataObject)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return dataObject.SetData(pformatetc, pmedium, fRelease);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(dataObject, e);
			return 0u;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int EnumFormatEtc(void* @this, int dwDirection, void** ppenumFormatEtc)
	{
		IDataObject dataObject = null;
		try
		{
			dataObject = (IDataObject)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IEnumFORMATETC obj = dataObject.EnumFormatEtc(dwDirection);
			*ppenumFormatEtc = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(dataObject, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int DAdvise(void* @this, FORMATETC* pformatetc, int advf, void* pAdvSink, int* pdwConnection)
	{
		IDataObject dataObject = null;
		try
		{
			dataObject = (IDataObject)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int num = dataObject.DAdvise(pformatetc, advf, pAdvSink);
			*pdwConnection = num;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(dataObject, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int DUnadvise(void* @this, int dwConnection)
	{
		IDataObject dataObject = null;
		try
		{
			dataObject = (IDataObject)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			dataObject.DUnadvise(dwConnection);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(dataObject, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int EnumDAdvise(void* @this, void** ppenumAdvise)
	{
		IDataObject dataObject = null;
		try
		{
			dataObject = (IDataObject)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = dataObject.EnumDAdvise();
			*ppenumAdvise = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(dataObject, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIDataObjectVTable()
	{
		AddMethod((delegate*<void*, FORMATETC*, STGMEDIUM*, uint>)(&GetData));
		AddMethod((delegate*<void*, FORMATETC*, STGMEDIUM*, uint>)(&GetDataHere));
		AddMethod((delegate*<void*, FORMATETC*, uint>)(&QueryGetData));
		AddMethod((delegate*<void*, FORMATETC*, FORMATETC*, int>)(&GetCanonicalFormatEtc));
		AddMethod((delegate*<void*, FORMATETC*, STGMEDIUM*, int, uint>)(&SetData));
		AddMethod((delegate*<void*, int, void**, int>)(&EnumFormatEtc));
		AddMethod((delegate*<void*, FORMATETC*, int, void*, int*, int>)(&DAdvise));
		AddMethod((delegate*<void*, int, int>)(&DUnadvise));
		AddMethod((delegate*<void*, void**, int>)(&EnumDAdvise));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDataObject), new __MicroComIDataObjectVTable().CreateVTable());
	}
}
