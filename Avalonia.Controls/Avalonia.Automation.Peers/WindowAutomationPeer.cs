using System;
using Avalonia.Controls;

namespace Avalonia.Automation.Peers;

public class WindowAutomationPeer : WindowBaseAutomationPeer
{
	public new Window Owner => (Window)base.Owner;

	public WindowAutomationPeer(Window owner)
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

	protected override string? GetNameCore()
	{
		return Owner.Title;
	}

	private void OnOpened(object? sender, EventArgs e)
	{
		Owner.Opened -= OnOpened;
		StartTrackingFocus();
	}

	private void OnClosed(object? sender, EventArgs e)
	{
		Owner.Closed -= OnClosed;
		StopTrackingFocus();
	}
}
