using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionGraphicsDevice2VTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateDrawingSurface2Delegate(void* @this, UnmanagedMethods.SIZE sizePixels, DirectXPixelFormat pixelFormat, DirectXAlphaMode alphaMode, void** result);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateDrawingSurface2(void* @this, UnmanagedMethods.SIZE sizePixels, DirectXPixelFormat pixelFormat, DirectXAlphaMode alphaMode, void** result)
	{
		ICompositionGraphicsDevice2 compositionGraphicsDevice = null;
		try
		{
			compositionGraphicsDevice = (ICompositionGraphicsDevice2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionDrawingSurface obj = compositionGraphicsDevice.CreateDrawingSurface2(sizePixels, pixelFormat, alphaMode);
			*result = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionGraphicsDevice, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionGraphicsDevice2VTable()
	{
		AddMethod((delegate*<void*, UnmanagedMethods.SIZE, DirectXPixelFormat, DirectXAlphaMode, void**, int>)(&CreateDrawingSurface2));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionGraphicsDevice2), new __MicroComICompositionGraphicsDevice2VTable().CreateVTable());
	}
}
