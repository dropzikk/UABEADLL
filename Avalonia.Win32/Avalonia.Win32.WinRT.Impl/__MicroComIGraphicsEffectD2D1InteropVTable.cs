using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIGraphicsEffectD2D1InteropVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetEffectIdDelegate(void* @this, Guid* id);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetNamedPropertyMappingDelegate(void* @this, IntPtr name, uint* index, GRAPHICS_EFFECT_PROPERTY_MAPPING* mapping);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetPropertyCountDelegate(void* @this, uint* count);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetPropertyDelegate(void* @this, uint index, void** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetSourceDelegate(void* @this, uint index, void** source);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetSourceCountDelegate(void* @this, uint* count);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetEffectId(void* @this, Guid* id)
	{
		IGraphicsEffectD2D1Interop graphicsEffectD2D1Interop = null;
		try
		{
			graphicsEffectD2D1Interop = (IGraphicsEffectD2D1Interop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			Guid effectId = graphicsEffectD2D1Interop.EffectId;
			*id = effectId;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(graphicsEffectD2D1Interop, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetNamedPropertyMapping(void* @this, IntPtr name, uint* index, GRAPHICS_EFFECT_PROPERTY_MAPPING* mapping)
	{
		IGraphicsEffectD2D1Interop graphicsEffectD2D1Interop = null;
		try
		{
			graphicsEffectD2D1Interop = (IGraphicsEffectD2D1Interop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			graphicsEffectD2D1Interop.GetNamedPropertyMapping(name, index, mapping);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(graphicsEffectD2D1Interop, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetPropertyCount(void* @this, uint* count)
	{
		IGraphicsEffectD2D1Interop graphicsEffectD2D1Interop = null;
		try
		{
			graphicsEffectD2D1Interop = (IGraphicsEffectD2D1Interop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			uint propertyCount = graphicsEffectD2D1Interop.PropertyCount;
			*count = propertyCount;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(graphicsEffectD2D1Interop, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetProperty(void* @this, uint index, void** value)
	{
		IGraphicsEffectD2D1Interop graphicsEffectD2D1Interop = null;
		try
		{
			graphicsEffectD2D1Interop = (IGraphicsEffectD2D1Interop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IPropertyValue property = graphicsEffectD2D1Interop.GetProperty(index);
			*value = MicroComRuntime.GetNativePointer(property, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(graphicsEffectD2D1Interop, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetSource(void* @this, uint index, void** source)
	{
		IGraphicsEffectD2D1Interop graphicsEffectD2D1Interop = null;
		try
		{
			graphicsEffectD2D1Interop = (IGraphicsEffectD2D1Interop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IGraphicsEffectSource source2 = graphicsEffectD2D1Interop.GetSource(index);
			*source = MicroComRuntime.GetNativePointer(source2, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(graphicsEffectD2D1Interop, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetSourceCount(void* @this, uint* count)
	{
		IGraphicsEffectD2D1Interop graphicsEffectD2D1Interop = null;
		try
		{
			graphicsEffectD2D1Interop = (IGraphicsEffectD2D1Interop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			uint sourceCount = graphicsEffectD2D1Interop.SourceCount;
			*count = sourceCount;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(graphicsEffectD2D1Interop, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIGraphicsEffectD2D1InteropVTable()
	{
		AddMethod((delegate*<void*, Guid*, int>)(&GetEffectId));
		AddMethod((delegate*<void*, IntPtr, uint*, GRAPHICS_EFFECT_PROPERTY_MAPPING*, int>)(&GetNamedPropertyMapping));
		AddMethod((delegate*<void*, uint*, int>)(&GetPropertyCount));
		AddMethod((delegate*<void*, uint, void**, int>)(&GetProperty));
		AddMethod((delegate*<void*, uint, void**, int>)(&GetSource));
		AddMethod((delegate*<void*, uint*, int>)(&GetSourceCount));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IGraphicsEffectD2D1Interop), new __MicroComIGraphicsEffectD2D1InteropVTable().CreateVTable());
	}
}
