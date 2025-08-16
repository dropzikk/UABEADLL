using Avalonia.Automation.Provider;
using Avalonia.Controls;

namespace Avalonia.Automation.Peers;

public class ButtonAutomationPeer : ContentControlAutomationPeer, IInvokeProvider
{
	public new Button Owner => (Button)base.Owner;

	public ButtonAutomationPeer(Button owner)
		: base(owner)
	{
	}

	public void Invoke()
	{
		EnsureEnabled();
		Owner?.PerformClick();
	}

	protected override string? GetAcceleratorKeyCore()
	{
		string text = base.GetAcceleratorKeyCore();
		if (string.IsNullOrWhiteSpace(text))
		{
			text = Owner.HotKey?.ToString();
		}
		return text;
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.Button;
	}

	protected override bool IsContentElementCore()
	{
		return true;
	}

	protected override bool IsControlElementCore()
	{
		return true;
	}
}
