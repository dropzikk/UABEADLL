using System;
using System.Collections.Specialized;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace Avalonia.Controls;

public class DataGridCheckBoxColumn : DataGridBoundColumn
{
	private CheckBox _currentCheckBox;

	private DataGrid _owningGrid;

	public static readonly StyledProperty<bool> IsThreeStateProperty = ToggleButton.IsThreeStateProperty.AddOwner<DataGridCheckBoxColumn>();

	public bool IsThreeState
	{
		get
		{
			return GetValue(IsThreeStateProperty);
		}
		set
		{
			SetValue(IsThreeStateProperty, value);
		}
	}

	public DataGridCheckBoxColumn()
	{
		base.BindingTarget = ToggleButton.IsCheckedProperty;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == IsThreeStateProperty)
		{
			NotifyPropertyChanged(change.Property.Name);
		}
	}

	protected override void CancelCellEdit(Control editingElement, object uneditedValue)
	{
		if (editingElement is CheckBox checkBox)
		{
			checkBox.IsChecked = (bool?)uneditedValue;
		}
	}

	protected override Control GenerateEditingElementDirect(DataGridCell cell, object dataItem)
	{
		CheckBox checkBox = new CheckBox
		{
			Margin = new Thickness(0.0)
		};
		ConfigureCheckBox(checkBox);
		return checkBox;
	}

	protected override Control GenerateElement(DataGridCell cell, object dataItem)
	{
		bool isEnabled = false;
		CheckBox checkBox = new CheckBox();
		if (EnsureOwningGrid() && cell.RowIndex != -1 && cell.ColumnIndex != -1 && cell.OwningRow != null && cell.OwningRow.Slot == base.OwningGrid.CurrentSlot && cell.ColumnIndex == base.OwningGrid.CurrentColumnIndex)
		{
			isEnabled = true;
			if (_currentCheckBox != null)
			{
				_currentCheckBox.IsEnabled = false;
			}
			_currentCheckBox = checkBox;
		}
		checkBox.IsEnabled = isEnabled;
		checkBox.IsHitTestVisible = false;
		ConfigureCheckBox(checkBox);
		if (Binding != null)
		{
			checkBox.Bind(base.BindingTarget, Binding);
		}
		return checkBox;
	}

	protected override object PrepareCellForEdit(Control editingElement, RoutedEventArgs editingEventArgs)
	{
		CheckBox editingCheckBox = editingElement as CheckBox;
		PointerPressedEventArgs args;
		if (editingCheckBox != null)
		{
			bool? isChecked = editingCheckBox.IsChecked;
			args = editingEventArgs as PointerPressedEventArgs;
			if (args != null)
			{
				if (editingCheckBox.Bounds.Width == 0.0 && editingCheckBox.Bounds.Height == 0.0)
				{
					editingCheckBox.LayoutUpdated += OnLayoutUpdated;
				}
				else
				{
					ProcessPointerArgs();
				}
			}
			return isChecked;
		}
		return false;
		void EditValue()
		{
			if (editingCheckBox.IsThreeState)
			{
				bool? isChecked2 = editingCheckBox.IsChecked;
				if (isChecked2.HasValue)
				{
					if (isChecked2 != true)
					{
						editingCheckBox.IsChecked = true;
					}
					else
					{
						editingCheckBox.IsChecked = null;
					}
				}
				else
				{
					editingCheckBox.IsChecked = false;
				}
			}
			else
			{
				editingCheckBox.IsChecked = !editingCheckBox.IsChecked;
			}
		}
		void OnLayoutUpdated(object sender, EventArgs e)
		{
			if (editingCheckBox.Bounds.Width != 0.0 || editingCheckBox.Bounds.Height != 0.0)
			{
				editingCheckBox.LayoutUpdated -= OnLayoutUpdated;
				ProcessPointerArgs();
			}
		}
		void ProcessPointerArgs()
		{
			Point position = args.GetPosition(editingCheckBox);
			Rect rect = new Rect(0.0, 0.0, editingCheckBox.Bounds.Width, editingCheckBox.Bounds.Height);
			if (rect.Contains(position))
			{
				EditValue();
			}
		}
	}

	protected internal override void RefreshCellContent(Control element, string propertyName)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		if (element is CheckBox content)
		{
			DataGridHelper.SyncColumnProperty(this, content, IsThreeStateProperty);
			return;
		}
		throw DataGridError.DataGrid.ValueIsNotAnInstanceOf("element", typeof(CheckBox));
	}

	private void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Contains(this) && _owningGrid != null)
		{
			_owningGrid.Columns.CollectionChanged -= Columns_CollectionChanged;
			_owningGrid.CurrentCellChanged -= OwningGrid_CurrentCellChanged;
			_owningGrid.KeyDown -= OwningGrid_KeyDown;
			_owningGrid.LoadingRow -= OwningGrid_LoadingRow;
			_owningGrid = null;
		}
	}

	private void ConfigureCheckBox(CheckBox checkBox)
	{
		checkBox.HorizontalAlignment = HorizontalAlignment.Center;
		checkBox.VerticalAlignment = VerticalAlignment.Center;
		DataGridHelper.SyncColumnProperty(this, checkBox, IsThreeStateProperty);
	}

	private bool EnsureOwningGrid()
	{
		if (base.OwningGrid != null)
		{
			if (base.OwningGrid != _owningGrid)
			{
				_owningGrid = base.OwningGrid;
				_owningGrid.Columns.CollectionChanged += Columns_CollectionChanged;
				_owningGrid.CurrentCellChanged += OwningGrid_CurrentCellChanged;
				_owningGrid.KeyDown += OwningGrid_KeyDown;
				_owningGrid.LoadingRow += OwningGrid_LoadingRow;
			}
			return true;
		}
		return false;
	}

	private void OwningGrid_CurrentCellChanged(object sender, EventArgs e)
	{
		if (_currentCheckBox != null)
		{
			_currentCheckBox.IsEnabled = false;
		}
		if (base.OwningGrid != null && base.OwningGrid.CurrentColumn == this && base.OwningGrid.IsSlotVisible(base.OwningGrid.CurrentSlot) && base.OwningGrid.DisplayData.GetDisplayedElement(base.OwningGrid.CurrentSlot) is DataGridRow dataGridRow)
		{
			CheckBox checkBox = GetCellContent(dataGridRow) as CheckBox;
			if (checkBox != null)
			{
				checkBox.IsEnabled = true;
			}
			_currentCheckBox = checkBox;
		}
	}

	private void OwningGrid_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Space && base.OwningGrid != null && base.OwningGrid.CurrentColumn == this && base.OwningGrid.DisplayData.GetDisplayedElement(base.OwningGrid.CurrentSlot) is DataGridRow dataGridRow && GetCellContent(dataGridRow) as CheckBox == _currentCheckBox)
		{
			base.OwningGrid.BeginEdit();
		}
	}

	private void OwningGrid_LoadingRow(object sender, DataGridRowEventArgs e)
	{
		if (base.OwningGrid == null || !(GetCellContent(e.Row) is CheckBox checkBox))
		{
			return;
		}
		if (base.OwningGrid.CurrentColumnIndex == base.Index && base.OwningGrid.CurrentSlot == e.Row.Slot)
		{
			if (_currentCheckBox != null)
			{
				_currentCheckBox.IsEnabled = false;
			}
			checkBox.IsEnabled = true;
			_currentCheckBox = checkBox;
		}
		else
		{
			checkBox.IsEnabled = false;
		}
	}
}
