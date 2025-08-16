using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnWindowBaseEventsProxy : MicroComProxyBase, IAvnWindowBaseEvents, IUnknown, IDisposable
{
	public unsafe IAvnAutomationPeer AutomationPeer => MicroComRuntime.CreateProxyOrNullFor<IAvnAutomationPeer>(((delegate* unmanaged[Stdcall]<void*, void*>)(*base.PPV)[base.VTableSize + 13])(base.PPV), ownsHandle: true);

	protected override int VTableSize => base.VTableSize + 14;

	public unsafe void Paint()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize])(base.PPV);
		if (num != 0)
		{
			throw new COMException("Paint failed", num);
		}
	}

	public unsafe void Closed()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 1])(base.PPV);
	}

	public unsafe void Activated()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 2])(base.PPV);
	}

	public unsafe void Deactivated()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 3])(base.PPV);
	}

	public unsafe void Resized(AvnSize* size, AvnPlatformResizeReason reason)
	{
		((delegate* unmanaged[Stdcall]<void*, void*, AvnPlatformResizeReason, void>)(*base.PPV)[base.VTableSize + 4])(base.PPV, size, reason);
	}

	public unsafe void PositionChanged(AvnPoint position)
	{
		((delegate* unmanaged[Stdcall]<void*, AvnPoint, void>)(*base.PPV)[base.VTableSize + 5])(base.PPV, position);
	}

	public unsafe void RawMouseEvent(AvnRawMouseEventType type, ulong timeStamp, AvnInputModifiers modifiers, AvnPoint point, AvnVector delta)
	{
		((delegate* unmanaged[Stdcall]<void*, AvnRawMouseEventType, ulong, AvnInputModifiers, AvnPoint, AvnVector, void>)(*base.PPV)[base.VTableSize + 6])(base.PPV, type, timeStamp, modifiers, point, delta);
	}

	public unsafe int RawKeyEvent(AvnRawKeyEventType type, ulong timeStamp, AvnInputModifiers modifiers, uint key)
	{
		return ((delegate* unmanaged[Stdcall]<void*, AvnRawKeyEventType, ulong, AvnInputModifiers, uint, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, type, timeStamp, modifiers, key);
	}

	public unsafe int RawTextInputEvent(ulong timeStamp, string text)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(text) + 1];
		Encoding.UTF8.GetBytes(text, 0, text.Length, array, 0);
		int result;
		fixed (byte* ptr = array)
		{
			result = ((delegate* unmanaged[Stdcall]<void*, ulong, void*, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV, timeStamp, ptr);
		}
		return result;
	}

	public unsafe void ScalingChanged(double scaling)
	{
		((delegate* unmanaged[Stdcall]<void*, double, void>)(*base.PPV)[base.VTableSize + 9])(base.PPV, scaling);
	}

	public unsafe void RunRenderPriorityJobs()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 10])(base.PPV);
	}

	public unsafe void LostFocus()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 11])(base.PPV);
	}

	public unsafe AvnDragDropEffects DragEvent(AvnDragEventType type, AvnPoint position, AvnInputModifiers modifiers, AvnDragDropEffects effects, IAvnClipboard clipboard, IntPtr dataObjectHandle)
	{
		return ((delegate* unmanaged[Stdcall]<void*, AvnDragEventType, AvnPoint, AvnInputModifiers, AvnDragDropEffects, void*, IntPtr, AvnDragDropEffects>)(*base.PPV)[base.VTableSize + 12])(base.PPV, type, position, modifiers, effects, MicroComRuntime.GetNativePointer(clipboard), dataObjectHandle);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnWindowBaseEvents), new Guid("939b6599-40a8-4710-a4c8-5d72d8f174fb"), (IntPtr p, bool owns) => new __MicroComIAvnWindowBaseEventsProxy(p, owns));
	}

	protected __MicroComIAvnWindowBaseEventsProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
