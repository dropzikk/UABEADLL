using System;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls.Primitives;

namespace Avalonia.Controls.Automation.Peers;

public class RadioButtonAutomationPeer : ToggleButtonAutomationPeer, ISelectionItemProvider
{
	public bool IsSelected => ((RadioButton)base.Owner).IsChecked == true;

	public ISelectionProvider? SelectionContainer => null;

	public RadioButtonAutomationPeer(RadioButton owner)
		: base(owner)
	{
		owner.PropertyChanged += delegate(object? a, AvaloniaPropertyChangedEventArgs e)
		{
			if (e.Property == ToggleButton.IsCheckedProperty)
			{
				RaiseToggleStatePropertyChangedEvent((bool?)e.OldValue, (bool?)e.NewValue);
			}
		};
	}

	protected override string GetClassNameCore()
	{
		return "RadioButton";
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.RadioButton;
	}

	public void AddToSelection()
	{
		if (((RadioButton)base.Owner).IsChecked != true)
		{
			throw new InvalidOperationException("Operation cannot be performed");
		}
	}

	public void RemoveFromSelection()
	{
		if (((RadioButton)base.Owner).IsChecked == true)
		{
			throw new InvalidOperationException("Operation cannot be performed");
		}
	}

	public void Select()
	{
		if (!IsEnabled())
		{
			throw new InvalidOperationException("Element is disabled thus it cannot be selected");
		}
		((RadioButton)base.Owner).IsChecked = true;
	}

	internal virtual void RaiseToggleStatePropertyChangedEvent(bool? oldValue, bool? newValue)
	{
		RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, oldValue == true, newValue == true);
	}
}
