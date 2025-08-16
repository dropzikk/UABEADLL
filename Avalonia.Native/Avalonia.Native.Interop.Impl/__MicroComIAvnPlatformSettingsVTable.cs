using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnPlatformSettingsVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate AvnPlatformThemeVariant GetPlatformThemeDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate uint GetAccentColorDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void RegisterColorsChangeDelegate(void* @this, void* callback);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static AvnPlatformThemeVariant GetPlatformTheme(void* @this)
	{
		IAvnPlatformSettings avnPlatformSettings = null;
		try
		{
			avnPlatformSettings = (IAvnPlatformSettings)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnPlatformSettings.PlatformTheme;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnPlatformSettings, e);
			return AvnPlatformThemeVariant.Light;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static uint GetAccentColor(void* @this)
	{
		IAvnPlatformSettings avnPlatformSettings = null;
		try
		{
			avnPlatformSettings = (IAvnPlatformSettings)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnPlatformSettings.AccentColor;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnPlatformSettings, e);
			return 0u;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void RegisterColorsChange(void* @this, void* callback)
	{
		IAvnPlatformSettings avnPlatformSettings = null;
		try
		{
			avnPlatformSettings = (IAvnPlatformSettings)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnPlatformSettings.RegisterColorsChange(MicroComRuntime.CreateProxyOrNullFor<IAvnActionCallback>(callback, ownsHandle: false));
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnPlatformSettings, e);
		}
	}

	protected unsafe __MicroComIAvnPlatformSettingsVTable()
	{
		AddMethod((delegate*<void*, AvnPlatformThemeVariant>)(&GetPlatformTheme));
		AddMethod((delegate*<void*, uint>)(&GetAccentColor));
		AddMethod((delegate*<void*, void*, void>)(&RegisterColorsChange));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnPlatformSettings), new __MicroComIAvnPlatformSettingsVTable().CreateVTable());
	}
}
