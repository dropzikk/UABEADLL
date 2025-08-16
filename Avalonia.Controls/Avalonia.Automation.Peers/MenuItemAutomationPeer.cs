using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Avalonia.Automation.Peers;

public class MenuItemAutomationPeer : ControlAutomationPeer
{
	public new MenuItem Owner => (MenuItem)base.Owner;

	public MenuItemAutomationPeer(MenuItem owner)
		: base(owner)
	{
	}

	protected override string? GetAccessKeyCore()
	{
		string text = base.GetAccessKeyCore();
		if (string.IsNullOrWhiteSpace(text) && Owner.HeaderPresenter?.Child is AccessText { AccessKey: var accessKey })
		{
			text = accessKey.ToString();
		}
		return text;
	}

	protected override string? GetAcceleratorKeyCore()
	{
		string text = base.GetAcceleratorKeyCore();
		if (string.IsNullOrWhiteSpace(text))
		{
			text = Owner.InputGesture?.ToString();
		}
		return text;
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.MenuItem;
	}

	protected override string? GetNameCore()
	{
		string text = base.GetNameCore();
		if (text == null && Owner.Header is string text2)
		{
			text = AccessText.RemoveAccessKeyMarker(text2);
		}
		return text;
	}
}
