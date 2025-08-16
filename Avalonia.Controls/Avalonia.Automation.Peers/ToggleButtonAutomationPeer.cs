using Avalonia.Automation.Provider;
using Avalonia.Controls.Primitives;

namespace Avalonia.Automation.Peers;

public class ToggleButtonAutomationPeer : ContentControlAutomationPeer, IToggleProvider
{
	public new ToggleButton Owner => (ToggleButton)base.Owner;

	ToggleState IToggleProvider.ToggleState
	{
		get
		{
			bool? isChecked = Owner.IsChecked;
			if (isChecked.HasValue)
			{
				if (isChecked == true)
				{
					return ToggleState.On;
				}
				return ToggleState.Off;
			}
			return ToggleState.Indeterminate;
		}
	}

	public ToggleButtonAutomationPeer(ToggleButton owner)
		: base(owner)
	{
	}

	void IToggleProvider.Toggle()
	{
		EnsureEnabled();
		Owner.PerformClick();
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
