using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositorWithBlurredWallpaperBackdropBrushVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int TryCreateBlurredWallpaperBackdropBrushDelegate(void* @this, void** result);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int TryCreateBlurredWallpaperBackdropBrush(void* @this, void** result)
	{
		ICompositorWithBlurredWallpaperBackdropBrush compositorWithBlurredWallpaperBackdropBrush = null;
		try
		{
			compositorWithBlurredWallpaperBackdropBrush = (ICompositorWithBlurredWallpaperBackdropBrush)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionBackdropBrush obj = compositorWithBlurredWallpaperBackdropBrush.TryCreateBlurredWallpaperBackdropBrush();
			*result = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositorWithBlurredWallpaperBackdropBrush, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositorWithBlurredWallpaperBackdropBrushVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&TryCreateBlurredWallpaperBackdropBrush));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositorWithBlurredWallpaperBackdropBrush), new __MicroComICompositorWithBlurredWallpaperBackdropBrushVTable().CreateVTable());
	}
}
