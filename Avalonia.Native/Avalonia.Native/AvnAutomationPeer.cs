using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Native.Interop;
using MicroCom.Runtime;

namespace Avalonia.Native;

internal class AvnAutomationPeer : NativeCallbackBase, IAvnAutomationPeer, IUnknown, IDisposable
{
	private static readonly ConditionalWeakTable<AutomationPeer, AvnAutomationPeer> s_wrappers = new ConditionalWeakTable<AutomationPeer, AvnAutomationPeer>();

	private readonly AutomationPeer _inner;

	public IAvnAutomationNode? Node { get; private set; }

	public IAvnString? AcceleratorKey => _inner.GetAcceleratorKey().ToAvnString();

	public IAvnString? AccessKey => _inner.GetAccessKey().ToAvnString();

	public AvnAutomationControlType AutomationControlType => (AvnAutomationControlType)_inner.GetAutomationControlType();

	public IAvnString? AutomationId => _inner.GetAutomationId().ToAvnString();

	public AvnRect BoundingRectangle => _inner.GetBoundingRectangle().ToAvnRect();

	public IAvnAutomationPeerArray Children => new AvnAutomationPeerArray(_inner.GetChildren());

	public IAvnString ClassName => _inner.GetClassName().ToAvnString();

	public IAvnAutomationPeer? LabeledBy => Wrap(_inner.GetLabeledBy());

	public IAvnString Name => _inner.GetName().ToAvnString();

	public IAvnAutomationPeer? Parent => Wrap(_inner.GetParent());

	public IAvnAutomationPeer? RootPeer
	{
		get
		{
			AutomationPeer automationPeer = _inner;
			AutomationPeer parent = automationPeer.GetParent();
			while (!(automationPeer is IRootProvider) && parent != null)
			{
				automationPeer = parent;
				parent = automationPeer.GetParent();
			}
			return Wrap(automationPeer);
		}
	}

	private AvnAutomationPeer(AutomationPeer inner)
	{
		_inner = inner;
		_inner.ChildrenChanged += delegate
		{
			Node?.ChildrenChanged();
		};
		if (inner is WindowBaseAutomationPeer windowBaseAutomationPeer)
		{
			windowBaseAutomationPeer.FocusChanged += delegate
			{
				Node?.FocusChanged();
			};
		}
	}

	~AvnAutomationPeer()
	{
		Node?.Dispose();
	}

	public int HasKeyboardFocus()
	{
		return _inner.HasKeyboardFocus().AsComBool();
	}

	public int IsContentElement()
	{
		return _inner.IsContentElement().AsComBool();
	}

	public int IsControlElement()
	{
		return _inner.IsControlElement().AsComBool();
	}

	public int IsEnabled()
	{
		return _inner.IsEnabled().AsComBool();
	}

	public int IsKeyboardFocusable()
	{
		return _inner.IsKeyboardFocusable().AsComBool();
	}

	public void SetFocus()
	{
		_inner.SetFocus();
	}

	public int ShowContextMenu()
	{
		return _inner.ShowContextMenu().AsComBool();
	}

	public void SetNode(IAvnAutomationNode node)
	{
		if (Node != null)
		{
			throw new InvalidOperationException("The AvnAutomationPeer already has a node.");
		}
		Node = node;
	}

	public int IsRootProvider()
	{
		return (_inner is IRootProvider).AsComBool();
	}

	public IAvnWindowBase RootProvider_GetWindow()
	{
		return ((WindowBaseImpl)((WindowBase)((ControlAutomationPeer)_inner).Owner).PlatformImpl).Native;
	}

	public IAvnAutomationPeer? RootProvider_GetFocus()
	{
		return Wrap(((IRootProvider)_inner).GetFocus());
	}

	public IAvnAutomationPeer? RootProvider_GetPeerFromPoint(AvnPoint point)
	{
		AutomationPeer automationPeer = ((IRootProvider)_inner).GetPeerFromPoint(point.ToAvaloniaPoint());
		if (automationPeer == null)
		{
			return null;
		}
		while (!automationPeer.IsControlElement())
		{
			AutomationPeer parent = automationPeer.GetParent();
			if (parent == null)
			{
				break;
			}
			automationPeer = parent;
		}
		return Wrap(automationPeer);
	}

	public int IsExpandCollapseProvider()
	{
		return (_inner is IExpandCollapseProvider).AsComBool();
	}

	public int ExpandCollapseProvider_GetIsExpanded()
	{
		return ((IExpandCollapseProvider)_inner).ExpandCollapseState switch
		{
			ExpandCollapseState.Expanded => 1, 
			ExpandCollapseState.PartiallyExpanded => 1, 
			_ => 0, 
		};
	}

	public int ExpandCollapseProvider_GetShowsMenu()
	{
		return ((IExpandCollapseProvider)_inner).ShowsMenu.AsComBool();
	}

	public void ExpandCollapseProvider_Expand()
	{
		((IExpandCollapseProvider)_inner).Expand();
	}

	public void ExpandCollapseProvider_Collapse()
	{
		((IExpandCollapseProvider)_inner).Collapse();
	}

	public int IsInvokeProvider()
	{
		return (_inner is IInvokeProvider).AsComBool();
	}

	public void InvokeProvider_Invoke()
	{
		((IInvokeProvider)_inner).Invoke();
	}

	public int IsRangeValueProvider()
	{
		return (_inner is IRangeValueProvider).AsComBool();
	}

	public double RangeValueProvider_GetValue()
	{
		return ((IRangeValueProvider)_inner).Value;
	}

	public double RangeValueProvider_GetMinimum()
	{
		return ((IRangeValueProvider)_inner).Minimum;
	}

	public double RangeValueProvider_GetMaximum()
	{
		return ((IRangeValueProvider)_inner).Maximum;
	}

	public double RangeValueProvider_GetSmallChange()
	{
		return ((IRangeValueProvider)_inner).SmallChange;
	}

	public double RangeValueProvider_GetLargeChange()
	{
		return ((IRangeValueProvider)_inner).LargeChange;
	}

	public void RangeValueProvider_SetValue(double value)
	{
		((IRangeValueProvider)_inner).SetValue(value);
	}

	public int IsSelectionItemProvider()
	{
		return (_inner is ISelectionItemProvider).AsComBool();
	}

	public int SelectionItemProvider_IsSelected()
	{
		return ((ISelectionItemProvider)_inner).IsSelected.AsComBool();
	}

	public int IsToggleProvider()
	{
		return (_inner is IToggleProvider).AsComBool();
	}

	public int ToggleProvider_GetToggleState()
	{
		return (int)((IToggleProvider)_inner).ToggleState;
	}

	public void ToggleProvider_Toggle()
	{
		((IToggleProvider)_inner).Toggle();
	}

	public int IsValueProvider()
	{
		return (_inner is IValueProvider).AsComBool();
	}

	public IAvnString ValueProvider_GetValue()
	{
		return ((IValueProvider)_inner).Value.ToAvnString();
	}

	public void ValueProvider_SetValue(string value)
	{
		((IValueProvider)_inner).SetValue(value);
	}

	[return: NotNullIfNotNull("peer")]
	public static AvnAutomationPeer? Wrap(AutomationPeer? peer)
	{
		if (peer != null)
		{
			return s_wrappers.GetValue(peer, (AutomationPeer x) => new AvnAutomationPeer(peer));
		}
		return null;
	}
}
