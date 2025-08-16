using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace Avalonia.Automation.Peers;

public class ControlAutomationPeer : AutomationPeer
{
	private IReadOnlyList<AutomationPeer>? _children;

	private bool _childrenValid;

	private AutomationPeer? _parent;

	private bool _parentValid;

	public Control Owner { get; }

	public ControlAutomationPeer(Control owner)
	{
		Owner = owner ?? throw new ArgumentNullException("owner");
		Initialize();
	}

	public AutomationPeer GetOrCreate(Control element)
	{
		if (element == Owner)
		{
			return this;
		}
		return CreatePeerForElement(element);
	}

	public static AutomationPeer CreatePeerForElement(Control element)
	{
		return element.GetOrCreateAutomationPeer();
	}

	public static AutomationPeer? FromElement(Control element)
	{
		return element.GetAutomationPeer();
	}

	protected override void BringIntoViewCore()
	{
		Owner.BringIntoView();
	}

	protected override IReadOnlyList<AutomationPeer> GetOrCreateChildrenCore()
	{
		IReadOnlyList<AutomationPeer> readOnlyList = _children ?? Array.Empty<AutomationPeer>();
		if (_childrenValid)
		{
			return readOnlyList;
		}
		IReadOnlyList<AutomationPeer> readOnlyList2 = GetChildrenCore() ?? Array.Empty<AutomationPeer>();
		foreach (AutomationPeer item in readOnlyList.Except(readOnlyList2))
		{
			item.TrySetParent(null);
		}
		foreach (AutomationPeer item2 in readOnlyList2)
		{
			item2.TrySetParent(this);
		}
		_childrenValid = true;
		return _children = readOnlyList2;
	}

	protected virtual IReadOnlyList<AutomationPeer>? GetChildrenCore()
	{
		IAvaloniaList<Visual> visualChildren = Owner.VisualChildren;
		if (visualChildren.Count == 0)
		{
			return null;
		}
		List<AutomationPeer> list = new List<AutomationPeer>();
		foreach (Visual item in visualChildren)
		{
			if (item is Control { IsVisible: not false } control)
			{
				list.Add(GetOrCreate(control));
			}
		}
		return list;
	}

	protected override AutomationPeer? GetLabeledByCore()
	{
		Control labeledBy = AutomationProperties.GetLabeledBy(Owner);
		if (labeledBy != null)
		{
			Control element = labeledBy;
			return GetOrCreate(element);
		}
		return null;
	}

	protected override string? GetNameCore()
	{
		string name = AutomationProperties.GetName(Owner);
		if (string.IsNullOrWhiteSpace(name))
		{
			AutomationPeer labeledBy = GetLabeledBy();
			if (labeledBy != null)
			{
				name = labeledBy.GetName();
			}
		}
		return name;
	}

	protected override AutomationPeer? GetParentCore()
	{
		EnsureConnected();
		return _parent;
	}

	protected void InvalidateChildren()
	{
		_childrenValid = false;
		RaiseChildrenChangedEvent();
	}

	protected void InvalidateParent()
	{
		_parent = null;
		_parentValid = false;
	}

	protected override bool ShowContextMenuCore()
	{
		for (Control control = Owner; control != null; control = control.Parent as Control)
		{
			if (control.ContextMenu != null)
			{
				control.ContextMenu.Open(control);
				return true;
			}
		}
		return false;
	}

	protected internal override bool TrySetParent(AutomationPeer? parent)
	{
		_parent = parent;
		return true;
	}

	protected override string? GetAcceleratorKeyCore()
	{
		return AutomationProperties.GetAcceleratorKey(Owner);
	}

	protected override string? GetAccessKeyCore()
	{
		return AutomationProperties.GetAccessKey(Owner);
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.Custom;
	}

	protected override string? GetAutomationIdCore()
	{
		return AutomationProperties.GetAutomationId(Owner) ?? Owner.Name;
	}

	protected override Rect GetBoundingRectangleCore()
	{
		return GetBounds(Owner);
	}

	protected override string GetClassNameCore()
	{
		return Owner.GetType().Name;
	}

	protected override bool HasKeyboardFocusCore()
	{
		return Owner.IsFocused;
	}

	protected override bool IsContentElementCore()
	{
		return true;
	}

	protected override bool IsControlElementCore()
	{
		return true;
	}

	protected override bool IsEnabledCore()
	{
		return Owner.IsEffectivelyEnabled;
	}

	protected override bool IsKeyboardFocusableCore()
	{
		return Owner.Focusable;
	}

	protected override void SetFocusCore()
	{
		Owner.Focus();
	}

	protected override AutomationControlType GetControlTypeOverrideCore()
	{
		return AutomationProperties.GetControlTypeOverride(Owner) ?? GetAutomationControlTypeCore();
	}

	protected override bool IsContentElementOverrideCore()
	{
		AccessibilityView accessibilityView = AutomationProperties.GetAccessibilityView(Owner);
		if (accessibilityView != 0)
		{
			return accessibilityView >= AccessibilityView.Content;
		}
		return IsContentElementCore();
	}

	protected override bool IsControlElementOverrideCore()
	{
		AccessibilityView accessibilityView = AutomationProperties.GetAccessibilityView(Owner);
		if (accessibilityView != 0)
		{
			return accessibilityView >= AccessibilityView.Control;
		}
		return IsControlElementCore();
	}

	private static Rect GetBounds(Control control)
	{
		if (!(control.GetVisualRoot() is Visual to))
		{
			return default(Rect);
		}
		Matrix? matrix = control.TransformToVisual(to);
		if (!matrix.HasValue)
		{
			return default(Rect);
		}
		return new Rect(control.Bounds.Size).TransformToAABB(matrix.Value);
	}

	private void Initialize()
	{
		Owner.PropertyChanged += OwnerPropertyChanged;
		Owner.VisualChildren.CollectionChanged += VisualChildrenChanged;
	}

	private void VisualChildrenChanged(object? sender, EventArgs e)
	{
		InvalidateChildren();
	}

	private void OwnerPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == Visual.IsVisibleProperty)
		{
			if (Owner.GetVisualParent() is Control element)
			{
				(GetOrCreate(element) as ControlAutomationPeer)?.InvalidateChildren();
			}
		}
		else if (e.Property == Visual.BoundsProperty || e.Property == Visual.RenderTransformProperty || e.Property == Visual.RenderTransformOriginProperty)
		{
			RaisePropertyChangedEvent(AutomationElementIdentifiers.BoundingRectangleProperty, null, GetBounds(Owner));
		}
		else if (e.Property == Visual.VisualParentProperty)
		{
			InvalidateParent();
		}
	}

	private void EnsureConnected()
	{
		if (_parentValid)
		{
			return;
		}
		for (Visual visualParent = Owner.GetVisualParent(); visualParent != null; visualParent = visualParent.GetVisualParent())
		{
			if (visualParent is Control element)
			{
				GetOrCreate(element).GetChildren();
			}
		}
		_parentValid = true;
	}
}
