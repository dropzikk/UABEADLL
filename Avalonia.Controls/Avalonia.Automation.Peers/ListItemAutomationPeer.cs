using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Selection;

namespace Avalonia.Automation.Peers;

public class ListItemAutomationPeer : ContentControlAutomationPeer, ISelectionItemProvider
{
	public bool IsSelected => base.Owner.GetValue(ListBoxItem.IsSelectedProperty);

	public ISelectionProvider? SelectionContainer
	{
		get
		{
			if (base.Owner.Parent is Control element)
			{
				return GetOrCreate(element) as ISelectionProvider;
			}
			return null;
		}
	}

	public ListItemAutomationPeer(ContentControl owner)
		: base(owner)
	{
	}

	public void Select()
	{
		EnsureEnabled();
		if (base.Owner.Parent is SelectingItemsControl selectingItemsControl)
		{
			int num = selectingItemsControl.IndexFromContainer(base.Owner);
			if (num != -1)
			{
				selectingItemsControl.SelectedIndex = num;
			}
		}
	}

	void ISelectionItemProvider.AddToSelection()
	{
		EnsureEnabled();
		if (!(base.Owner.Parent is ItemsControl itemsControl))
		{
			return;
		}
		ISelectionModel value = itemsControl.GetValue(ListBox.SelectionProperty);
		if (value != null)
		{
			int num = itemsControl.IndexFromContainer(base.Owner);
			if (num != -1)
			{
				value.Select(num);
			}
		}
	}

	void ISelectionItemProvider.RemoveFromSelection()
	{
		EnsureEnabled();
		if (!(base.Owner.Parent is ItemsControl itemsControl))
		{
			return;
		}
		ISelectionModel value = itemsControl.GetValue(ListBox.SelectionProperty);
		if (value != null)
		{
			int num = itemsControl.IndexFromContainer(base.Owner);
			if (num != -1)
			{
				value.Deselect(num);
			}
		}
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.ListItem;
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
