using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIShapeVisualVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetShapesDelegate(void* @this, void** value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetShapes(void* @this, void** value)
	{
		IShapeVisual shapeVisual = null;
		try
		{
			shapeVisual = (IShapeVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IUnknown shapes = shapeVisual.Shapes;
			*value = MicroComRuntime.GetNativePointer(shapes, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(shapeVisual, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIShapeVisualVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&GetShapes));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IShapeVisual), new __MicroComIShapeVisualVTable().CreateVTable());
	}
}
