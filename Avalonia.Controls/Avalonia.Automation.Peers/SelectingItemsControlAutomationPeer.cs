using System;
using System.Collections.Generic;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Selection;

namespace Avalonia.Automation.Peers;

public abstract class SelectingItemsControlAutomationPeer : ItemsControlAutomationPeer, ISelectionProvider
{
	private ISelectionModel _selection;

	public bool CanSelectMultiple => GetSelectionModeCore().HasAllFlags(SelectionMode.Multiple);

	public bool IsSelectionRequired => GetSelectionModeCore().HasAllFlags(SelectionMode.AlwaysSelected);

	protected SelectingItemsControlAutomationPeer(SelectingItemsControl owner)
		: base(owner)
	{
		_selection = owner.GetValue(ListBox.SelectionProperty);
		_selection.SelectionChanged += OwnerSelectionChanged;
		owner.PropertyChanged += OwnerPropertyChanged;
	}

	public IReadOnlyList<AutomationPeer> GetSelection()
	{
		return GetSelectionCore() ?? Array.Empty<AutomationPeer>();
	}

	protected virtual IReadOnlyList<AutomationPeer>? GetSelectionCore()
	{
		List<AutomationPeer> list = null;
		if (base.Owner is SelectingItemsControl selectingItemsControl)
		{
			{
				foreach (int selectedIndex in base.Owner.GetValue(ListBox.SelectionProperty).SelectedIndexes)
				{
					Control control = selectingItemsControl.ContainerFromIndex(selectedIndex);
					if (control == null)
					{
						continue;
					}
					Control control2 = control;
					if (!control2.IsAttachedToVisualTree)
					{
						continue;
					}
					AutomationPeer orCreate = GetOrCreate(control2);
					if (orCreate != null)
					{
						if (list == null)
						{
							list = new List<AutomationPeer>();
						}
						list.Add(orCreate);
					}
				}
				return list;
			}
		}
		return list;
	}

	protected virtual SelectionMode GetSelectionModeCore()
	{
		return (base.Owner as SelectingItemsControl)?.GetValue(ListBox.SelectionModeProperty) ?? SelectionMode.Single;
	}

	protected virtual void OwnerPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == ListBox.SelectionProperty)
		{
			_selection.SelectionChanged -= OwnerSelectionChanged;
			_selection = base.Owner.GetValue(ListBox.SelectionProperty);
			_selection.SelectionChanged += OwnerSelectionChanged;
			RaiseSelectionChanged();
		}
	}

	protected virtual void OwnerSelectionChanged(object? sender, SelectionModelSelectionChangedEventArgs e)
	{
		RaiseSelectionChanged();
	}

	private void RaiseSelectionChanged()
	{
		RaisePropertyChangedEvent(SelectionPatternIdentifiers.SelectionProperty, null, null);
	}
}
