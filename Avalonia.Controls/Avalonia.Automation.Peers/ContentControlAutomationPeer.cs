using Avalonia.Controls;

namespace Avalonia.Automation.Peers;

public class ContentControlAutomationPeer : ControlAutomationPeer
{
	public new ContentControl Owner => (ContentControl)base.Owner;

	protected ContentControlAutomationPeer(ContentControl owner)
		: base(owner)
	{
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.Pane;
	}

	protected override string? GetNameCore()
	{
		string text = base.GetNameCore();
		if (text == null && Owner.Presenter?.Child is TextBlock textBlock)
		{
			text = textBlock.Text;
		}
		if (text == null)
		{
			text = Owner.Content?.ToString();
		}
		return text;
	}

	protected override bool IsContentElementCore()
	{
		return false;
	}

	protected override bool IsControlElementCore()
	{
		return false;
	}
}
