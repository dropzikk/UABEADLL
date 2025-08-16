using System;
using System.Collections.Generic;
using Avalonia.Automation.Provider;
using Avalonia.Controls;

namespace Avalonia.Automation.Peers;

public class ComboBoxAutomationPeer : SelectingItemsControlAutomationPeer, IExpandCollapseProvider, IValueProvider
{
	private class UnrealizedSelectionPeer : UnrealizedElementAutomationPeer
	{
		private readonly ComboBoxAutomationPeer _owner;

		private object? _item;

		public object? Item
		{
			get
			{
				return _item;
			}
			set
			{
				if (_item != value)
				{
					string nameCore = GetNameCore();
					_item = value;
					RaisePropertyChangedEvent(AutomationElementIdentifiers.NameProperty, nameCore, GetNameCore());
				}
			}
		}

		public UnrealizedSelectionPeer(ComboBoxAutomationPeer owner)
		{
			_owner = owner;
		}

		protected override string? GetAcceleratorKeyCore()
		{
			return null;
		}

		protected override string? GetAccessKeyCore()
		{
			return null;
		}

		protected override string? GetAutomationIdCore()
		{
			return null;
		}

		protected override string GetClassNameCore()
		{
			return typeof(ComboBoxItem).Name;
		}

		protected override AutomationPeer? GetLabeledByCore()
		{
			return null;
		}

		protected override AutomationPeer? GetParentCore()
		{
			return _owner;
		}

		protected override AutomationControlType GetAutomationControlTypeCore()
		{
			return AutomationControlType.ListItem;
		}

		protected override string? GetNameCore()
		{
			if (_item is Control control)
			{
				string text = AutomationProperties.GetName(control);
				if (text == null && control is ContentControl contentControl && contentControl.Presenter?.Child is TextBlock textBlock)
				{
					text = textBlock.Text;
				}
				if (text == null)
				{
					text = control.GetValue(ContentControl.ContentProperty)?.ToString();
				}
				return text;
			}
			return _item?.ToString();
		}
	}

	private UnrealizedSelectionPeer[]? _selection;

	public new ComboBox Owner => (ComboBox)base.Owner;

	public ExpandCollapseState ExpandCollapseState => ToState(Owner.IsDropDownOpen);

	public bool ShowsMenu => true;

	bool IValueProvider.IsReadOnly => true;

	string? IValueProvider.Value
	{
		get
		{
			IReadOnlyList<AutomationPeer> selection = GetSelection();
			if (selection.Count != 1)
			{
				return null;
			}
			return selection[0].GetName();
		}
	}

	public ComboBoxAutomationPeer(ComboBox owner)
		: base(owner)
	{
	}

	public void Collapse()
	{
		Owner.IsDropDownOpen = false;
	}

	public void Expand()
	{
		Owner.IsDropDownOpen = true;
	}

	void IValueProvider.SetValue(string? value)
	{
		throw new NotSupportedException();
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.ComboBox;
	}

	protected override IReadOnlyList<AutomationPeer>? GetSelectionCore()
	{
		if (ExpandCollapseState == ExpandCollapseState.Expanded)
		{
			return base.GetSelectionCore();
		}
		object selectedItem = Owner.SelectedItem;
		if (selectedItem != null)
		{
			if (_selection == null)
			{
				_selection = new UnrealizedSelectionPeer[1]
				{
					new UnrealizedSelectionPeer(this)
				};
			}
			_selection[0].Item = selectedItem;
			return _selection;
		}
		return null;
	}

	protected override void OwnerPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		base.OwnerPropertyChanged(sender, e);
		if (e.Property == ComboBox.IsDropDownOpenProperty)
		{
			RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, ToState((bool)e.OldValue), ToState((bool)e.NewValue));
		}
	}

	private static ExpandCollapseState ToState(bool value)
	{
		if (!value)
		{
			return ExpandCollapseState.Collapsed;
		}
		return ExpandCollapseState.Expanded;
	}
}
