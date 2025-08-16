using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIOutputVTable : __MicroComIDXGIObjectVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetDescDelegate(void* @this, DXGI_OUTPUT_DESC* pDesc);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetDisplayModeListDelegate(void* @this, DXGI_FORMAT EnumFormat, ushort Flags, ushort* pNumModes, DXGI_MODE_DESC* pDesc);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int FindClosestMatchingModeDelegate(void* @this, DXGI_MODE_DESC* pModeToMatch, DXGI_MODE_DESC* pClosestMatch, void* pConcernedDevice);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int WaitForVBlankDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int TakeOwnershipDelegate(void* @this, void* pDevice, int Exclusive);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void ReleaseOwnershipDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetGammaControlCapabilitiesDelegate(void* @this, IntPtr pGammaCaps);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetGammaControlDelegate(void* @this, void* pArray);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetGammaControlDelegate(void* @this, IntPtr pArray);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetDisplaySurfaceDelegate(void* @this, void* pScanoutSurface);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetDisplaySurfaceDataDelegate(void* @this, void* pDestination);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetFrameStatisticsDelegate(void* @this, DXGI_FRAME_STATISTICS* pStats);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetDesc(void* @this, DXGI_OUTPUT_DESC* pDesc)
	{
		IDXGIOutput iDXGIOutput = null;
		try
		{
			iDXGIOutput = (IDXGIOutput)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			DXGI_OUTPUT_DESC desc = iDXGIOutput.Desc;
			*pDesc = desc;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIOutput, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetDisplayModeList(void* @this, DXGI_FORMAT EnumFormat, ushort Flags, ushort* pNumModes, DXGI_MODE_DESC* pDesc)
	{
		IDXGIOutput iDXGIOutput = null;
		try
		{
			iDXGIOutput = (IDXGIOutput)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			DXGI_MODE_DESC displayModeList = iDXGIOutput.GetDisplayModeList(EnumFormat, Flags, pNumModes);
			*pDesc = displayModeList;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIOutput, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int FindClosestMatchingMode(void* @this, DXGI_MODE_DESC* pModeToMatch, DXGI_MODE_DESC* pClosestMatch, void* pConcernedDevice)
	{
		IDXGIOutput iDXGIOutput = null;
		try
		{
			iDXGIOutput = (IDXGIOutput)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIOutput.FindClosestMatchingMode(pModeToMatch, pClosestMatch, MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pConcernedDevice, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIOutput, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int WaitForVBlank(void* @this)
	{
		IDXGIOutput iDXGIOutput = null;
		try
		{
			iDXGIOutput = (IDXGIOutput)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIOutput.WaitForVBlank();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIOutput, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int TakeOwnership(void* @this, void* pDevice, int Exclusive)
	{
		IDXGIOutput iDXGIOutput = null;
		try
		{
			iDXGIOutput = (IDXGIOutput)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIOutput.TakeOwnership(MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pDevice, ownsHandle: false), Exclusive);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIOutput, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void ReleaseOwnership(void* @this)
	{
		IDXGIOutput iDXGIOutput = null;
		try
		{
			iDXGIOutput = (IDXGIOutput)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIOutput.ReleaseOwnership();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIOutput, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetGammaControlCapabilities(void* @this, IntPtr pGammaCaps)
	{
		IDXGIOutput iDXGIOutput = null;
		try
		{
			iDXGIOutput = (IDXGIOutput)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIOutput.GetGammaControlCapabilities(pGammaCaps);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIOutput, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetGammaControl(void* @this, void* pArray)
	{
		IDXGIOutput iDXGIOutput = null;
		try
		{
			iDXGIOutput = (IDXGIOutput)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIOutput.SetGammaControl(pArray);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIOutput, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetGammaControl(void* @this, IntPtr pArray)
	{
		IDXGIOutput iDXGIOutput = null;
		try
		{
			iDXGIOutput = (IDXGIOutput)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIOutput.GetGammaControl(pArray);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIOutput, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetDisplaySurface(void* @this, void* pScanoutSurface)
	{
		IDXGIOutput iDXGIOutput = null;
		try
		{
			iDXGIOutput = (IDXGIOutput)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIOutput.SetDisplaySurface(MicroComRuntime.CreateProxyOrNullFor<IDXGISurface>(pScanoutSurface, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIOutput, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetDisplaySurfaceData(void* @this, void* pDestination)
	{
		IDXGIOutput iDXGIOutput = null;
		try
		{
			iDXGIOutput = (IDXGIOutput)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIOutput.GetDisplaySurfaceData(MicroComRuntime.CreateProxyOrNullFor<IDXGISurface>(pDestination, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIOutput, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetFrameStatistics(void* @this, DXGI_FRAME_STATISTICS* pStats)
	{
		IDXGIOutput iDXGIOutput = null;
		try
		{
			iDXGIOutput = (IDXGIOutput)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			DXGI_FRAME_STATISTICS frameStatistics = iDXGIOutput.FrameStatistics;
			*pStats = frameStatistics;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIOutput, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIDXGIOutputVTable()
	{
		AddMethod((delegate*<void*, DXGI_OUTPUT_DESC*, int>)(&GetDesc));
		AddMethod((delegate*<void*, DXGI_FORMAT, ushort, ushort*, DXGI_MODE_DESC*, int>)(&GetDisplayModeList));
		AddMethod((delegate*<void*, DXGI_MODE_DESC*, DXGI_MODE_DESC*, void*, int>)(&FindClosestMatchingMode));
		AddMethod((delegate*<void*, int>)(&WaitForVBlank));
		AddMethod((delegate*<void*, void*, int, int>)(&TakeOwnership));
		AddMethod((delegate*<void*, void>)(&ReleaseOwnership));
		AddMethod((delegate*<void*, IntPtr, int>)(&GetGammaControlCapabilities));
		AddMethod((delegate*<void*, void*, int>)(&SetGammaControl));
		AddMethod((delegate*<void*, IntPtr, int>)(&GetGammaControl));
		AddMethod((delegate*<void*, void*, int>)(&SetDisplaySurface));
		AddMethod((delegate*<void*, void*, int>)(&GetDisplaySurfaceData));
		AddMethod((delegate*<void*, DXGI_FRAME_STATISTICS*, int>)(&GetFrameStatistics));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDXGIOutput), new __MicroComIDXGIOutputVTable().CreateVTable());
	}
}
