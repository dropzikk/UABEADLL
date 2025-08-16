using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Input;
using Avalonia.Reactive;

namespace Avalonia.Controls;

public class ComboBoxItem : ListBoxItem
{
	public ComboBoxItem()
	{
		this.GetObservable(InputElement.IsFocusedProperty).Subscribe(delegate(bool focused)
		{
			if (focused)
			{
				(base.Parent as ComboBox)?.ItemFocused(this);
			}
		});
	}

	static ComboBoxItem()
	{
		AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<ComboBoxItem>(AutomationControlType.ComboBoxItem);
	}
}
