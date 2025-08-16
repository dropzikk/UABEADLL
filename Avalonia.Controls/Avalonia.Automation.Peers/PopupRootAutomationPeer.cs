using System;
using Avalonia.Controls.Primitives;

namespace Avalonia.Automation.Peers;

public class PopupRootAutomationPeer : WindowBaseAutomationPeer
{
	public PopupRootAutomationPeer(PopupRoot owner)
		: base(owner)
	{
		if (owner.IsVisible)
		{
			StartTrackingFocus();
		}
		else
		{
			owner.Opened += OnOpened;
		}
		owner.Closed += OnClosed;
	}

	protected override bool IsContentElementCore()
	{
		return false;
	}

	protected override bool IsControlElementCore()
	{
		return false;
	}

	protected override AutomationPeer? GetParentCore()
	{
		return base.GetParentCore();
	}

	private void OnOpened(object? sender, EventArgs e)
	{
		((PopupRoot)base.Owner).Opened -= OnOpened;
		StartTrackingFocus();
	}

	private void OnClosed(object? sender, EventArgs e)
	{
		((PopupRoot)base.Owner).Closed -= OnClosed;
		StopTrackingFocus();
	}
}
