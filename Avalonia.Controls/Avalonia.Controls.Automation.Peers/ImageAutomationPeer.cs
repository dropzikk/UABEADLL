using Avalonia.Automation.Peers;

namespace Avalonia.Controls.Automation.Peers;

public class ImageAutomationPeer : ControlAutomationPeer
{
	public ImageAutomationPeer(Control owner)
		: base(owner)
	{
	}

	protected override string GetClassNameCore()
	{
		return "Image";
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.Image;
	}
}
