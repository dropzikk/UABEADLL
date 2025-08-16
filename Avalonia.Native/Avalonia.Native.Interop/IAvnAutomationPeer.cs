using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnAutomationPeer : IUnknown, IDisposable
{
	IAvnAutomationNode Node { get; }

	IAvnString AcceleratorKey { get; }

	IAvnString AccessKey { get; }

	AvnAutomationControlType AutomationControlType { get; }

	IAvnString AutomationId { get; }

	AvnRect BoundingRectangle { get; }

	IAvnAutomationPeerArray Children { get; }

	IAvnString ClassName { get; }

	IAvnAutomationPeer LabeledBy { get; }

	IAvnString Name { get; }

	IAvnAutomationPeer Parent { get; }

	IAvnAutomationPeer RootPeer { get; }

	void SetNode(IAvnAutomationNode node);

	int HasKeyboardFocus();

	int IsContentElement();

	int IsControlElement();

	int IsEnabled();

	int IsKeyboardFocusable();

	void SetFocus();

	int ShowContextMenu();

	int IsRootProvider();

	IAvnWindowBase RootProvider_GetWindow();

	IAvnAutomationPeer RootProvider_GetFocus();

	IAvnAutomationPeer RootProvider_GetPeerFromPoint(AvnPoint point);

	int IsExpandCollapseProvider();

	int ExpandCollapseProvider_GetIsExpanded();

	int ExpandCollapseProvider_GetShowsMenu();

	void ExpandCollapseProvider_Expand();

	void ExpandCollapseProvider_Collapse();

	int IsInvokeProvider();

	void InvokeProvider_Invoke();

	int IsRangeValueProvider();

	double RangeValueProvider_GetValue();

	double RangeValueProvider_GetMinimum();

	double RangeValueProvider_GetMaximum();

	double RangeValueProvider_GetSmallChange();

	double RangeValueProvider_GetLargeChange();

	void RangeValueProvider_SetValue(double value);

	int IsSelectionItemProvider();

	int SelectionItemProvider_IsSelected();

	int IsToggleProvider();

	int ToggleProvider_GetToggleState();

	void ToggleProvider_Toggle();

	int IsValueProvider();

	IAvnString ValueProvider_GetValue();

	void ValueProvider_SetValue(string value);
}
