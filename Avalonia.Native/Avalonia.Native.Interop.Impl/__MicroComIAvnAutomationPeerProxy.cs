using System;
using System.Runtime.CompilerServices;
using System.Text;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnAutomationPeerProxy : MicroComProxyBase, IAvnAutomationPeer, IUnknown, IDisposable
{
	public unsafe IAvnAutomationNode Node => MicroComRuntime.CreateProxyOrNullFor<IAvnAutomationNode>(((delegate* unmanaged[Stdcall]<void*, void*>)(*base.PPV)[base.VTableSize])(base.PPV), ownsHandle: true);

	public unsafe IAvnString AcceleratorKey => MicroComRuntime.CreateProxyOrNullFor<IAvnString>(((delegate* unmanaged[Stdcall]<void*, void*>)(*base.PPV)[base.VTableSize + 2])(base.PPV), ownsHandle: true);

	public unsafe IAvnString AccessKey => MicroComRuntime.CreateProxyOrNullFor<IAvnString>(((delegate* unmanaged[Stdcall]<void*, void*>)(*base.PPV)[base.VTableSize + 3])(base.PPV), ownsHandle: true);

	public unsafe AvnAutomationControlType AutomationControlType => ((delegate* unmanaged[Stdcall]<void*, AvnAutomationControlType>)(*base.PPV)[base.VTableSize + 4])(base.PPV);

	public unsafe IAvnString AutomationId => MicroComRuntime.CreateProxyOrNullFor<IAvnString>(((delegate* unmanaged[Stdcall]<void*, void*>)(*base.PPV)[base.VTableSize + 5])(base.PPV), ownsHandle: true);

	public unsafe AvnRect BoundingRectangle => ((delegate* unmanaged[Stdcall]<void*, AvnRect>)(*base.PPV)[base.VTableSize + 6])(base.PPV);

	public unsafe IAvnAutomationPeerArray Children => MicroComRuntime.CreateProxyOrNullFor<IAvnAutomationPeerArray>(((delegate* unmanaged[Stdcall]<void*, void*>)(*base.PPV)[base.VTableSize + 7])(base.PPV), ownsHandle: true);

	public unsafe IAvnString ClassName => MicroComRuntime.CreateProxyOrNullFor<IAvnString>(((delegate* unmanaged[Stdcall]<void*, void*>)(*base.PPV)[base.VTableSize + 8])(base.PPV), ownsHandle: true);

	public unsafe IAvnAutomationPeer LabeledBy => MicroComRuntime.CreateProxyOrNullFor<IAvnAutomationPeer>(((delegate* unmanaged[Stdcall]<void*, void*>)(*base.PPV)[base.VTableSize + 9])(base.PPV), ownsHandle: true);

	public unsafe IAvnString Name => MicroComRuntime.CreateProxyOrNullFor<IAvnString>(((delegate* unmanaged[Stdcall]<void*, void*>)(*base.PPV)[base.VTableSize + 10])(base.PPV), ownsHandle: true);

	public unsafe IAvnAutomationPeer Parent => MicroComRuntime.CreateProxyOrNullFor<IAvnAutomationPeer>(((delegate* unmanaged[Stdcall]<void*, void*>)(*base.PPV)[base.VTableSize + 11])(base.PPV), ownsHandle: true);

	public unsafe IAvnAutomationPeer RootPeer => MicroComRuntime.CreateProxyOrNullFor<IAvnAutomationPeer>(((delegate* unmanaged[Stdcall]<void*, void*>)(*base.PPV)[base.VTableSize + 19])(base.PPV), ownsHandle: true);

	protected override int VTableSize => base.VTableSize + 46;

	public unsafe void SetNode(IAvnAutomationNode node)
	{
		((delegate* unmanaged[Stdcall]<void*, void*, void>)(*base.PPV)[base.VTableSize + 1])(base.PPV, MicroComRuntime.GetNativePointer(node));
	}

	public unsafe int HasKeyboardFocus()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 12])(base.PPV);
	}

	public unsafe int IsContentElement()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 13])(base.PPV);
	}

	public unsafe int IsControlElement()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 14])(base.PPV);
	}

	public unsafe int IsEnabled()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 15])(base.PPV);
	}

	public unsafe int IsKeyboardFocusable()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 16])(base.PPV);
	}

	public unsafe void SetFocus()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 17])(base.PPV);
	}

	public unsafe int ShowContextMenu()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 18])(base.PPV);
	}

	public unsafe int IsRootProvider()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 20])(base.PPV);
	}

	public unsafe IAvnWindowBase RootProvider_GetWindow()
	{
		return MicroComRuntime.CreateProxyOrNullFor<IAvnWindowBase>(((delegate* unmanaged[Stdcall]<void*, void*>)(*base.PPV)[base.VTableSize + 21])(base.PPV), ownsHandle: true);
	}

	public unsafe IAvnAutomationPeer RootProvider_GetFocus()
	{
		return MicroComRuntime.CreateProxyOrNullFor<IAvnAutomationPeer>(((delegate* unmanaged[Stdcall]<void*, void*>)(*base.PPV)[base.VTableSize + 22])(base.PPV), ownsHandle: true);
	}

	public unsafe IAvnAutomationPeer RootProvider_GetPeerFromPoint(AvnPoint point)
	{
		return MicroComRuntime.CreateProxyOrNullFor<IAvnAutomationPeer>(((delegate* unmanaged[Stdcall]<void*, AvnPoint, void*>)(*base.PPV)[base.VTableSize + 23])(base.PPV, point), ownsHandle: true);
	}

	public unsafe int IsExpandCollapseProvider()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 24])(base.PPV);
	}

	public unsafe int ExpandCollapseProvider_GetIsExpanded()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 25])(base.PPV);
	}

	public unsafe int ExpandCollapseProvider_GetShowsMenu()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 26])(base.PPV);
	}

	public unsafe void ExpandCollapseProvider_Expand()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 27])(base.PPV);
	}

	public unsafe void ExpandCollapseProvider_Collapse()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 28])(base.PPV);
	}

	public unsafe int IsInvokeProvider()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 29])(base.PPV);
	}

	public unsafe void InvokeProvider_Invoke()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 30])(base.PPV);
	}

	public unsafe int IsRangeValueProvider()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 31])(base.PPV);
	}

	public unsafe double RangeValueProvider_GetValue()
	{
		return ((delegate* unmanaged[Stdcall]<void*, double>)(*base.PPV)[base.VTableSize + 32])(base.PPV);
	}

	public unsafe double RangeValueProvider_GetMinimum()
	{
		return ((delegate* unmanaged[Stdcall]<void*, double>)(*base.PPV)[base.VTableSize + 33])(base.PPV);
	}

	public unsafe double RangeValueProvider_GetMaximum()
	{
		return ((delegate* unmanaged[Stdcall]<void*, double>)(*base.PPV)[base.VTableSize + 34])(base.PPV);
	}

	public unsafe double RangeValueProvider_GetSmallChange()
	{
		return ((delegate* unmanaged[Stdcall]<void*, double>)(*base.PPV)[base.VTableSize + 35])(base.PPV);
	}

	public unsafe double RangeValueProvider_GetLargeChange()
	{
		return ((delegate* unmanaged[Stdcall]<void*, double>)(*base.PPV)[base.VTableSize + 36])(base.PPV);
	}

	public unsafe void RangeValueProvider_SetValue(double value)
	{
		((delegate* unmanaged[Stdcall]<void*, double, void>)(*base.PPV)[base.VTableSize + 37])(base.PPV, value);
	}

	public unsafe int IsSelectionItemProvider()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 38])(base.PPV);
	}

	public unsafe int SelectionItemProvider_IsSelected()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 39])(base.PPV);
	}

	public unsafe int IsToggleProvider()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 40])(base.PPV);
	}

	public unsafe int ToggleProvider_GetToggleState()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 41])(base.PPV);
	}

	public unsafe void ToggleProvider_Toggle()
	{
		((delegate* unmanaged[Stdcall]<void*, void>)(*base.PPV)[base.VTableSize + 42])(base.PPV);
	}

	public unsafe int IsValueProvider()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 43])(base.PPV);
	}

	public unsafe IAvnString ValueProvider_GetValue()
	{
		return MicroComRuntime.CreateProxyOrNullFor<IAvnString>(((delegate* unmanaged[Stdcall]<void*, void*>)(*base.PPV)[base.VTableSize + 44])(base.PPV), ownsHandle: true);
	}

	public unsafe void ValueProvider_SetValue(string value)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(value) + 1];
		Encoding.UTF8.GetBytes(value, 0, value.Length, array, 0);
		fixed (byte* ptr = array)
		{
			((delegate* unmanaged[Stdcall]<void*, void*, void>)(*base.PPV)[base.VTableSize + 45])(base.PPV, ptr);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnAutomationPeer), new Guid("b87016f3-7eec-41de-b385-07844c268dc4"), (IntPtr p, bool owns) => new __MicroComIAvnAutomationPeerProxy(p, owns));
	}

	protected __MicroComIAvnAutomationPeerProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
