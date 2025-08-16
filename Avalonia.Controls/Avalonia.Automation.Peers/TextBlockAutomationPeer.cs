using Avalonia.Controls;

namespace Avalonia.Automation.Peers;

public class TextBlockAutomationPeer : ControlAutomationPeer
{
	public new TextBlock Owner => (TextBlock)base.Owner;

	public TextBlockAutomationPeer(TextBlock owner)
		: base(owner)
	{
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.Text;
	}

	protected override string? GetNameCore()
	{
		return Owner.Text;
	}

	protected override bool IsControlElementCore()
	{
		if (Owner.TemplatedParent == null)
		{
			return base.IsControlElementCore();
		}
		return false;
	}
}
