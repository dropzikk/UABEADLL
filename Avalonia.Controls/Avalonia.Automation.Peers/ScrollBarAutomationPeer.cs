using Avalonia.Controls.Primitives;

namespace Avalonia.Automation.Peers;

public class ScrollBarAutomationPeer : RangeBaseAutomationPeer
{
	public ScrollBarAutomationPeer(ScrollBar owner)
		: base(owner)
	{
	}

	protected override string GetClassNameCore()
	{
		return "ScrollBar";
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.ScrollBar;
	}

	protected override bool IsContentElementCore()
	{
		return false;
	}
}
