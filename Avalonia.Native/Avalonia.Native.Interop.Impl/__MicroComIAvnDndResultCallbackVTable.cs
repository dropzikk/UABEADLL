using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnDndResultCallbackVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void OnDragAndDropCompleteDelegate(void* @this, AvnDragDropEffects effecct);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void OnDragAndDropComplete(void* @this, AvnDragDropEffects effecct)
	{
		IAvnDndResultCallback avnDndResultCallback = null;
		try
		{
			avnDndResultCallback = (IAvnDndResultCallback)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnDndResultCallback.OnDragAndDropComplete(effecct);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnDndResultCallback, e);
		}
	}

	protected unsafe __MicroComIAvnDndResultCallbackVTable()
	{
		AddMethod((delegate*<void*, AvnDragDropEffects, void>)(&OnDragAndDropComplete));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnDndResultCallback), new __MicroComIAvnDndResultCallbackVTable().CreateVTable());
	}
}
