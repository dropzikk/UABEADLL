using Avalonia.Automation.Peers;

namespace Avalonia.Controls.Automation.Peers;

public class SliderAutomationPeer : RangeBaseAutomationPeer
{
	public SliderAutomationPeer(Slider owner)
		: base(owner)
	{
	}

	protected override string GetClassNameCore()
	{
		return "Slider";
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.Slider;
	}
}
