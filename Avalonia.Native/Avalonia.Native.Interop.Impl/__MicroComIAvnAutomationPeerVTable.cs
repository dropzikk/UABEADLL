using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnAutomationPeerVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* GetNodeDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void SetNodeDelegate(void* @this, void* node);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* GetAcceleratorKeyDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* GetAccessKeyDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate AvnAutomationControlType GetAutomationControlTypeDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* GetAutomationIdDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate AvnRect GetBoundingRectangleDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* GetChildrenDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* GetClassNameDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* GetLabeledByDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* GetNameDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* GetParentDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int HasKeyboardFocusDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int IsContentElementDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int IsControlElementDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int IsEnabledDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int IsKeyboardFocusableDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void SetFocusDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ShowContextMenuDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* GetRootPeerDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int IsRootProviderDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* RootProvider_GetWindowDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* RootProvider_GetFocusDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* RootProvider_GetPeerFromPointDelegate(void* @this, AvnPoint point);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int IsExpandCollapseProviderDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ExpandCollapseProvider_GetIsExpandedDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ExpandCollapseProvider_GetShowsMenuDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void ExpandCollapseProvider_ExpandDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void ExpandCollapseProvider_CollapseDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int IsInvokeProviderDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void InvokeProvider_InvokeDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int IsRangeValueProviderDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate double RangeValueProvider_GetValueDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate double RangeValueProvider_GetMinimumDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate double RangeValueProvider_GetMaximumDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate double RangeValueProvider_GetSmallChangeDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate double RangeValueProvider_GetLargeChangeDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void RangeValueProvider_SetValueDelegate(void* @this, double value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int IsSelectionItemProviderDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SelectionItemProvider_IsSelectedDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int IsToggleProviderDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ToggleProvider_GetToggleStateDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void ToggleProvider_ToggleDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int IsValueProviderDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void* ValueProvider_GetValueDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void ValueProvider_SetValueDelegate(void* @this, byte* value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* GetNode(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avnAutomationPeer.Node, owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return null;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void SetNode(void* @this, void* node)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnAutomationPeer.SetNode(MicroComRuntime.CreateProxyOrNullFor<IAvnAutomationNode>(node, ownsHandle: false));
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* GetAcceleratorKey(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avnAutomationPeer.AcceleratorKey, owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return null;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* GetAccessKey(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avnAutomationPeer.AccessKey, owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return null;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static AvnAutomationControlType GetAutomationControlType(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.AutomationControlType;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return AvnAutomationControlType.AutomationNone;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* GetAutomationId(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avnAutomationPeer.AutomationId, owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return null;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static AvnRect GetBoundingRectangle(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.BoundingRectangle;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return default(AvnRect);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* GetChildren(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avnAutomationPeer.Children, owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return null;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* GetClassName(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avnAutomationPeer.ClassName, owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return null;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* GetLabeledBy(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avnAutomationPeer.LabeledBy, owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return null;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* GetName(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avnAutomationPeer.Name, owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return null;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* GetParent(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avnAutomationPeer.Parent, owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return null;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int HasKeyboardFocus(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.HasKeyboardFocus();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int IsContentElement(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.IsContentElement();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int IsControlElement(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.IsControlElement();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int IsEnabled(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.IsEnabled();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int IsKeyboardFocusable(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.IsKeyboardFocusable();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void SetFocus(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnAutomationPeer.SetFocus();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ShowContextMenu(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.ShowContextMenu();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* GetRootPeer(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avnAutomationPeer.RootPeer, owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return null;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int IsRootProvider(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.IsRootProvider();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* RootProvider_GetWindow(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avnAutomationPeer.RootProvider_GetWindow(), owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return null;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* RootProvider_GetFocus(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avnAutomationPeer.RootProvider_GetFocus(), owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return null;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* RootProvider_GetPeerFromPoint(void* @this, AvnPoint point)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avnAutomationPeer.RootProvider_GetPeerFromPoint(point), owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return null;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int IsExpandCollapseProvider(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.IsExpandCollapseProvider();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ExpandCollapseProvider_GetIsExpanded(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.ExpandCollapseProvider_GetIsExpanded();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ExpandCollapseProvider_GetShowsMenu(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.ExpandCollapseProvider_GetShowsMenu();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void ExpandCollapseProvider_Expand(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnAutomationPeer.ExpandCollapseProvider_Expand();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void ExpandCollapseProvider_Collapse(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnAutomationPeer.ExpandCollapseProvider_Collapse();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int IsInvokeProvider(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.IsInvokeProvider();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void InvokeProvider_Invoke(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnAutomationPeer.InvokeProvider_Invoke();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int IsRangeValueProvider(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.IsRangeValueProvider();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static double RangeValueProvider_GetValue(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.RangeValueProvider_GetValue();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0.0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static double RangeValueProvider_GetMinimum(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.RangeValueProvider_GetMinimum();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0.0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static double RangeValueProvider_GetMaximum(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.RangeValueProvider_GetMaximum();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0.0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static double RangeValueProvider_GetSmallChange(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.RangeValueProvider_GetSmallChange();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0.0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static double RangeValueProvider_GetLargeChange(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.RangeValueProvider_GetLargeChange();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0.0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void RangeValueProvider_SetValue(void* @this, double value)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnAutomationPeer.RangeValueProvider_SetValue(value);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int IsSelectionItemProvider(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.IsSelectionItemProvider();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SelectionItemProvider_IsSelected(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.SelectionItemProvider_IsSelected();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int IsToggleProvider(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.IsToggleProvider();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ToggleProvider_GetToggleState(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.ToggleProvider_GetToggleState();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void ToggleProvider_Toggle(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnAutomationPeer.ToggleProvider_Toggle();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int IsValueProvider(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeer.IsValueProvider();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void* ValueProvider_GetValue(void* @this)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return MicroComRuntime.GetNativePointer(avnAutomationPeer.ValueProvider_GetValue(), owned: true);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
			return null;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void ValueProvider_SetValue(void* @this, byte* value)
	{
		IAvnAutomationPeer avnAutomationPeer = null;
		try
		{
			avnAutomationPeer = (IAvnAutomationPeer)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnAutomationPeer.ValueProvider_SetValue((value == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(value)));
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeer, e);
		}
	}

	protected unsafe __MicroComIAvnAutomationPeerVTable()
	{
		AddMethod((delegate*<void*, void*>)(&GetNode));
		AddMethod((delegate*<void*, void*, void>)(&SetNode));
		AddMethod((delegate*<void*, void*>)(&GetAcceleratorKey));
		AddMethod((delegate*<void*, void*>)(&GetAccessKey));
		AddMethod((delegate*<void*, AvnAutomationControlType>)(&GetAutomationControlType));
		AddMethod((delegate*<void*, void*>)(&GetAutomationId));
		AddMethod((delegate*<void*, AvnRect>)(&GetBoundingRectangle));
		AddMethod((delegate*<void*, void*>)(&GetChildren));
		AddMethod((delegate*<void*, void*>)(&GetClassName));
		AddMethod((delegate*<void*, void*>)(&GetLabeledBy));
		AddMethod((delegate*<void*, void*>)(&GetName));
		AddMethod((delegate*<void*, void*>)(&GetParent));
		AddMethod((delegate*<void*, int>)(&HasKeyboardFocus));
		AddMethod((delegate*<void*, int>)(&IsContentElement));
		AddMethod((delegate*<void*, int>)(&IsControlElement));
		AddMethod((delegate*<void*, int>)(&IsEnabled));
		AddMethod((delegate*<void*, int>)(&IsKeyboardFocusable));
		AddMethod((delegate*<void*, void>)(&SetFocus));
		AddMethod((delegate*<void*, int>)(&ShowContextMenu));
		AddMethod((delegate*<void*, void*>)(&GetRootPeer));
		AddMethod((delegate*<void*, int>)(&IsRootProvider));
		AddMethod((delegate*<void*, void*>)(&RootProvider_GetWindow));
		AddMethod((delegate*<void*, void*>)(&RootProvider_GetFocus));
		AddMethod((delegate*<void*, AvnPoint, void*>)(&RootProvider_GetPeerFromPoint));
		AddMethod((delegate*<void*, int>)(&IsExpandCollapseProvider));
		AddMethod((delegate*<void*, int>)(&ExpandCollapseProvider_GetIsExpanded));
		AddMethod((delegate*<void*, int>)(&ExpandCollapseProvider_GetShowsMenu));
		AddMethod((delegate*<void*, void>)(&ExpandCollapseProvider_Expand));
		AddMethod((delegate*<void*, void>)(&ExpandCollapseProvider_Collapse));
		AddMethod((delegate*<void*, int>)(&IsInvokeProvider));
		AddMethod((delegate*<void*, void>)(&InvokeProvider_Invoke));
		AddMethod((delegate*<void*, int>)(&IsRangeValueProvider));
		AddMethod((delegate*<void*, double>)(&RangeValueProvider_GetValue));
		AddMethod((delegate*<void*, double>)(&RangeValueProvider_GetMinimum));
		AddMethod((delegate*<void*, double>)(&RangeValueProvider_GetMaximum));
		AddMethod((delegate*<void*, double>)(&RangeValueProvider_GetSmallChange));
		AddMethod((delegate*<void*, double>)(&RangeValueProvider_GetLargeChange));
		AddMethod((delegate*<void*, double, void>)(&RangeValueProvider_SetValue));
		AddMethod((delegate*<void*, int>)(&IsSelectionItemProvider));
		AddMethod((delegate*<void*, int>)(&SelectionItemProvider_IsSelected));
		AddMethod((delegate*<void*, int>)(&IsToggleProvider));
		AddMethod((delegate*<void*, int>)(&ToggleProvider_GetToggleState));
		AddMethod((delegate*<void*, void>)(&ToggleProvider_Toggle));
		AddMethod((delegate*<void*, int>)(&IsValueProvider));
		AddMethod((delegate*<void*, void*>)(&ValueProvider_GetValue));
		AddMethod((delegate*<void*, byte*, void>)(&ValueProvider_SetValue));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnAutomationPeer), new __MicroComIAvnAutomationPeerVTable().CreateVTable());
	}
}
