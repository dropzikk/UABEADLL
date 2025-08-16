using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGISurfaceVTable : __MicroComIDXGIDeviceSubObjectVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetDescDelegate(void* @this, DXGI_SURFACE_DESC* pDesc);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int MapDelegate(void* @this, DXGI_MAPPED_RECT* pLockedRect, ushort MapFlags);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int UnmapDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetDesc(void* @this, DXGI_SURFACE_DESC* pDesc)
	{
		IDXGISurface iDXGISurface = null;
		try
		{
			iDXGISurface = (IDXGISurface)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			DXGI_SURFACE_DESC desc = iDXGISurface.Desc;
			*pDesc = desc;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGISurface, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Map(void* @this, DXGI_MAPPED_RECT* pLockedRect, ushort MapFlags)
	{
		IDXGISurface iDXGISurface = null;
		try
		{
			iDXGISurface = (IDXGISurface)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGISurface.Map(pLockedRect, MapFlags);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGISurface, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Unmap(void* @this)
	{
		IDXGISurface iDXGISurface = null;
		try
		{
			iDXGISurface = (IDXGISurface)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGISurface.Unmap();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGISurface, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIDXGISurfaceVTable()
	{
		AddMethod((delegate*<void*, DXGI_SURFACE_DESC*, int>)(&GetDesc));
		AddMethod((delegate*<void*, DXGI_MAPPED_RECT*, ushort, int>)(&Map));
		AddMethod((delegate*<void*, int>)(&Unmap));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDXGISurface), new __MicroComIDXGISurfaceVTable().CreateVTable());
	}
}
