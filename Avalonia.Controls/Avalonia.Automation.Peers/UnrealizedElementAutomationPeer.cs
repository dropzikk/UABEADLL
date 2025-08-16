using System;
using System.Collections.Generic;

namespace Avalonia.Automation.Peers;

public abstract class UnrealizedElementAutomationPeer : AutomationPeer
{
	public void SetParent(AutomationPeer? parent)
	{
		TrySetParent(parent);
	}

	protected override void BringIntoViewCore()
	{
		GetParent()?.BringIntoView();
	}

	protected override Rect GetBoundingRectangleCore()
	{
		return GetParent()?.GetBoundingRectangle() ?? default(Rect);
	}

	protected override IReadOnlyList<AutomationPeer> GetOrCreateChildrenCore()
	{
		return Array.Empty<AutomationPeer>();
	}

	protected override bool HasKeyboardFocusCore()
	{
		return false;
	}

	protected override bool IsContentElementCore()
	{
		return false;
	}

	protected override bool IsControlElementCore()
	{
		return false;
	}

	protected override bool IsEnabledCore()
	{
		return true;
	}

	protected override bool IsKeyboardFocusableCore()
	{
		return false;
	}

	protected override void SetFocusCore()
	{
	}

	protected override bool ShowContextMenuCore()
	{
		return false;
	}

	protected internal override bool TrySetParent(AutomationPeer? parent)
	{
		return false;
	}
}
