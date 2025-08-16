using Avalonia.Automation.Peers;
using Avalonia.Controls.Primitives;

namespace Avalonia.Controls.Automation.Peers;

public class LabelAutomationPeer : ControlAutomationPeer
{
	public LabelAutomationPeer(Label owner)
		: base(owner)
	{
	}

	protected override string GetClassNameCore()
	{
		return "Text";
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.Text;
	}

	protected override string? GetNameCore()
	{
		string text = ((Label)base.Owner).Content as string;
		if (string.IsNullOrEmpty(text))
		{
			return base.GetNameCore();
		}
		return AccessText.RemoveAccessKeyMarker(text) ?? string.Empty;
	}
}
