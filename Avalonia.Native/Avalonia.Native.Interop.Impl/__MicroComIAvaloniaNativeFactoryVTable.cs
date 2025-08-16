using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvaloniaNativeFactoryVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int InitializeDelegate(void* @this, void* deallocator, void* appCb, void* dispatcher);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* GetMacOptionsDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateWindowDelegate(void* @this, void* cb, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreatePopupDelegate(void* @this, void* cb, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreatePlatformThreadingInterfaceDelegate(void* @this, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateSystemDialogsDelegate(void* @this, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateScreensDelegate(void* @this, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateClipboardDelegate(void* @this, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateDndClipboardDelegate(void* @this, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateCursorFactoryDelegate(void* @this, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ObtainGlDisplayDelegate(void* @this, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ObtainMetalDisplayDelegate(void* @this, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetAppMenuDelegate(void* @this, void* menu);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetServicesMenuDelegate(void* @this, void* menu);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateMenuDelegate(void* @this, void* cb, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateMenuItemDelegate(void* @this, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateMenuItemSeparatorDelegate(void* @this, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateTrayIconDelegate(void* @this, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateApplicationCommandsDelegate(void* @this, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreatePlatformSettingsDelegate(void* @this, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreatePlatformBehaviorInhibitionDelegate(void* @this, void** ppv);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Initialize(void* @this, void* deallocator, void* appCb, void* dispatcher)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avaloniaNativeFactory.Initialize(MicroComRuntime.CreateProxyOrNullFor<IAvnGCHandleDeallocatorCallback>(deallocator, ownsHandle: false), MicroComRuntime.CreateProxyOrNullFor<IAvnApplicationEvents>(appCb, ownsHandle: false), MicroComRuntime.CreateProxyOrNullFor<IAvnDispatcher>(dispatcher, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* GetMacOptions(void* @this)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avaloniaNativeFactory.MacOptions, owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return null;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateWindow(void* @this, void* cb, void** ppv)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnWindow obj = avaloniaNativeFactory.CreateWindow(MicroComRuntime.CreateProxyOrNullFor<IAvnWindowEvents>(cb, ownsHandle: false));
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreatePopup(void* @this, void* cb, void** ppv)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnPopup obj = avaloniaNativeFactory.CreatePopup(MicroComRuntime.CreateProxyOrNullFor<IAvnWindowEvents>(cb, ownsHandle: false));
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreatePlatformThreadingInterface(void* @this, void** ppv)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnPlatformThreadingInterface obj = avaloniaNativeFactory.CreatePlatformThreadingInterface();
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateSystemDialogs(void* @this, void** ppv)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnSystemDialogs obj = avaloniaNativeFactory.CreateSystemDialogs();
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateScreens(void* @this, void** ppv)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnScreens obj = avaloniaNativeFactory.CreateScreens();
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateClipboard(void* @this, void** ppv)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnClipboard obj = avaloniaNativeFactory.CreateClipboard();
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateDndClipboard(void* @this, void** ppv)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnClipboard obj = avaloniaNativeFactory.CreateDndClipboard();
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateCursorFactory(void* @this, void** ppv)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnCursorFactory obj = avaloniaNativeFactory.CreateCursorFactory();
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ObtainGlDisplay(void* @this, void** ppv)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnGlDisplay obj = avaloniaNativeFactory.ObtainGlDisplay();
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ObtainMetalDisplay(void* @this, void** ppv)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnMetalDisplay obj = avaloniaNativeFactory.ObtainMetalDisplay();
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetAppMenu(void* @this, void* menu)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avaloniaNativeFactory.SetAppMenu(MicroComRuntime.CreateProxyOrNullFor<IAvnMenu>(menu, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetServicesMenu(void* @this, void* menu)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avaloniaNativeFactory.SetServicesMenu(MicroComRuntime.CreateProxyOrNullFor<IAvnMenu>(menu, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateMenu(void* @this, void* cb, void** ppv)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnMenu obj = avaloniaNativeFactory.CreateMenu(MicroComRuntime.CreateProxyOrNullFor<IAvnMenuEvents>(cb, ownsHandle: false));
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateMenuItem(void* @this, void** ppv)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnMenuItem obj = avaloniaNativeFactory.CreateMenuItem();
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateMenuItemSeparator(void* @this, void** ppv)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnMenuItem obj = avaloniaNativeFactory.CreateMenuItemSeparator();
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateTrayIcon(void* @this, void** ppv)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnTrayIcon obj = avaloniaNativeFactory.CreateTrayIcon();
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateApplicationCommands(void* @this, void** ppv)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnApplicationCommands obj = avaloniaNativeFactory.CreateApplicationCommands();
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreatePlatformSettings(void* @this, void** ppv)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnPlatformSettings obj = avaloniaNativeFactory.CreatePlatformSettings();
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreatePlatformBehaviorInhibition(void* @this, void** ppv)
	{
		IAvaloniaNativeFactory avaloniaNativeFactory = null;
		try
		{
			avaloniaNativeFactory = (IAvaloniaNativeFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnPlatformBehaviorInhibition obj = avaloniaNativeFactory.CreatePlatformBehaviorInhibition();
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avaloniaNativeFactory, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvaloniaNativeFactoryVTable()
	{
		AddMethod((delegate*<void*, void*, void*, void*, int>)(&Initialize));
		AddMethod((delegate*<void*, void*>)(&GetMacOptions));
		AddMethod((delegate*<void*, void*, void**, int>)(&CreateWindow));
		AddMethod((delegate*<void*, void*, void**, int>)(&CreatePopup));
		AddMethod((delegate*<void*, void**, int>)(&CreatePlatformThreadingInterface));
		AddMethod((delegate*<void*, void**, int>)(&CreateSystemDialogs));
		AddMethod((delegate*<void*, void**, int>)(&CreateScreens));
		AddMethod((delegate*<void*, void**, int>)(&CreateClipboard));
		AddMethod((delegate*<void*, void**, int>)(&CreateDndClipboard));
		AddMethod((delegate*<void*, void**, int>)(&CreateCursorFactory));
		AddMethod((delegate*<void*, void**, int>)(&ObtainGlDisplay));
		AddMethod((delegate*<void*, void**, int>)(&ObtainMetalDisplay));
		AddMethod((delegate*<void*, void*, int>)(&SetAppMenu));
		AddMethod((delegate*<void*, void*, int>)(&SetServicesMenu));
		AddMethod((delegate*<void*, void*, void**, int>)(&CreateMenu));
		AddMethod((delegate*<void*, void**, int>)(&CreateMenuItem));
		AddMethod((delegate*<void*, void**, int>)(&CreateMenuItemSeparator));
		AddMethod((delegate*<void*, void**, int>)(&CreateTrayIcon));
		AddMethod((delegate*<void*, void**, int>)(&CreateApplicationCommands));
		AddMethod((delegate*<void*, void**, int>)(&CreatePlatformSettings));
		AddMethod((delegate*<void*, void**, int>)(&CreatePlatformBehaviorInhibition));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvaloniaNativeFactory), new __MicroComIAvaloniaNativeFactoryVTable().CreateVTable());
	}
}
