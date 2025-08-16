using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComISpriteVisualVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetBrushDelegate(void* @this, void** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetBrushDelegate(void* @this, void* value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetBrush(void* @this, void** value)
	{
		ISpriteVisual spriteVisual = null;
		try
		{
			spriteVisual = (ISpriteVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionBrush brush = spriteVisual.Brush;
			*value = MicroComRuntime.GetNativePointer(brush, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(spriteVisual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetBrush(void* @this, void* value)
	{
		ISpriteVisual spriteVisual = null;
		try
		{
			spriteVisual = (ISpriteVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			spriteVisual.SetBrush(MicroComRuntime.CreateProxyOrNullFor<ICompositionBrush>(value, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(spriteVisual, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComISpriteVisualVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&GetBrush));
		AddMethod((delegate*<void*, void*, int>)(&SetBrush));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ISpriteVisual), new __MicroComISpriteVisualVTable().CreateVTable());
	}
}
