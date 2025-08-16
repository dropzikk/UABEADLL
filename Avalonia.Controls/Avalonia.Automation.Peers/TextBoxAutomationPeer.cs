using Avalonia.Automation.Provider;
using Avalonia.Controls;

namespace Avalonia.Automation.Peers;

public class TextBoxAutomationPeer : ControlAutomationPeer, IValueProvider
{
	public new TextBox Owner => (TextBox)base.Owner;

	public bool IsReadOnly => Owner.IsReadOnly;

	public string? Value => Owner.Text;

	public TextBoxAutomationPeer(TextBox owner)
		: base(owner)
	{
	}

	public void SetValue(string? value)
	{
		Owner.Text = value;
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.Edit;
	}
}
