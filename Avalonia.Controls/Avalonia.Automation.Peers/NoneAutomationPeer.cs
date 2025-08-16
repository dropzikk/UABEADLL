using Avalonia.Controls;

namespace Avalonia.Automation.Peers;

public class NoneAutomationPeer : ControlAutomationPeer
{
	public NoneAutomationPeer(Control owner)
		: base(owner)
	{
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.None;
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
