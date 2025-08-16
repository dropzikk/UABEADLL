using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;

namespace Avalonia.Automation.Peers;

public class PopupAutomationPeer : ControlAutomationPeer
{
	public PopupAutomationPeer(Popup owner)
		: base(owner)
	{
		owner.Opened += PopupOpenedClosed;
		owner.Closed += PopupOpenedClosed;
	}

	protected override IReadOnlyList<AutomationPeer>? GetChildrenCore()
	{
		if (!(((IPopupHostProvider)base.Owner).PopupHost is Control element))
		{
			return null;
		}
		return new AutomationPeer[1] { GetOrCreate(element) };
	}

	protected override bool IsContentElementCore()
	{
		return false;
	}

	protected override bool IsControlElementCore()
	{
		return false;
	}

	private void PopupOpenedClosed(object? sender, EventArgs e)
	{
		GetPopupRoot()?.TrySetParent(this);
		InvalidateChildren();
	}

	private AutomationPeer? GetPopupRoot()
	{
		if (!(((IPopupHostProvider)base.Owner).PopupHost is Control element))
		{
			return null;
		}
		return GetOrCreate(element);
	}
}
