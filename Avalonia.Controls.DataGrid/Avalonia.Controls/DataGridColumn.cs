using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls.Templates;
using Avalonia.Controls.Utils;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace Avalonia.Controls;

public abstract class DataGridColumn : AvaloniaObject
{
	internal const int DATAGRIDCOLUMN_maximumWidth = 65536;

	private const bool DATAGRIDCOLUMN_defaultIsReadOnly = false;

	private bool? _isReadOnly;

	private double? _maxWidth;

	private double? _minWidth;

	private bool _settingWidthInternally;

	private int _displayIndexWithFiller;

	private object _header;

	private IDataTemplate _headerTemplate;

	private DataGridColumnHeader _headerCell;

	private Control _editingElement;

	private ICellEditBinding _editBinding;

	private IBinding _clipboardContentBinding;

	private ControlTheme _cellTheme;

	private Classes _cellStyleClasses;

	private bool _setWidthInternalNoCallback;

	public static readonly StyledProperty<bool> IsVisibleProperty = Visual.IsVisibleProperty.AddOwner<DataGridColumn>();

	public static readonly DirectProperty<DataGridColumn, ControlTheme> CellThemeProperty = AvaloniaProperty.RegisterDirect("CellTheme", (DataGridColumn o) => o.CellTheme, delegate(DataGridColumn o, ControlTheme v)
	{
		o.CellTheme = v;
	});

	public static readonly DirectProperty<DataGridColumn, object> HeaderProperty = AvaloniaProperty.RegisterDirect("Header", (DataGridColumn o) => o.Header, delegate(DataGridColumn o, object v)
	{
		o.Header = v;
	});

	public static readonly DirectProperty<DataGridColumn, IDataTemplate> HeaderTemplateProperty = AvaloniaProperty.RegisterDirect("HeaderTemplate", (DataGridColumn o) => o.HeaderTemplate, delegate(DataGridColumn o, IDataTemplate v)
	{
		o.HeaderTemplate = v;
	});

	public static readonly StyledProperty<DataGridLength> WidthProperty;

	protected internal DataGrid OwningGrid { get; internal set; }

	internal int Index { get; set; }

	internal bool? CanUserReorderInternal { get; set; }

	internal bool? CanUserResizeInternal { get; set; }

	internal bool? CanUserSortInternal { get; set; }

	internal bool ActualCanUserResize
	{
		get
		{
			if (OwningGrid == null || !OwningGrid.CanUserResizeColumns || this is DataGridFillerColumn)
			{
				return false;
			}
			return CanUserResizeInternal ?? true;
		}
	}

	internal double ActualMaxWidth => _maxWidth ?? OwningGrid?.MaxColumnWidth ?? double.PositiveInfinity;

	internal double ActualMinWidth
	{
		get
		{
			double num = _minWidth ?? OwningGrid?.MinColumnWidth ?? 0.0;
			if (Width.IsStar)
			{
				return Math.Max(0.001, num);
			}
			return num;
		}
	}

	internal bool DisplayIndexHasChanged { get; set; }

	internal int DisplayIndexWithFiller
	{
		get
		{
			return _displayIndexWithFiller;
		}
		set
		{
			_displayIndexWithFiller = value;
		}
	}

	internal bool HasHeaderCell => _headerCell != null;

	internal DataGridColumnHeader HeaderCell
	{
		get
		{
			if (_headerCell == null)
			{
				_headerCell = CreateHeader();
			}
			return _headerCell;
		}
	}

	internal bool InheritsWidth { get; private set; }

	internal bool IsInitialDesiredWidthDetermined { get; set; }

	internal double LayoutRoundedWidth { get; private set; }

	internal ICellEditBinding CellEditBinding => _editBinding;

	public bool IsVisible
	{
		get
		{
			return GetValue(IsVisibleProperty);
		}
		set
		{
			SetValue(IsVisibleProperty, value);
		}
	}

	public double ActualWidth
	{
		get
		{
			if (OwningGrid == null || double.IsNaN(Width.DisplayValue))
			{
				return ActualMinWidth;
			}
			return Width.DisplayValue;
		}
	}

	public bool CanUserReorder
	{
		get
		{
			return CanUserReorderInternal ?? OwningGrid?.CanUserReorderColumns ?? true;
		}
		set
		{
			CanUserReorderInternal = value;
		}
	}

	public bool CanUserResize
	{
		get
		{
			return CanUserResizeInternal ?? OwningGrid?.CanUserResizeColumns ?? true;
		}
		set
		{
			CanUserResizeInternal = value;
			OwningGrid?.OnColumnCanUserResizeChanged(this);
		}
	}

	public bool CanUserSort
	{
		get
		{
			if (CanUserSortInternal.HasValue)
			{
				return CanUserSortInternal.Value;
			}
			if (OwningGrid != null)
			{
				string sortPropertyName = GetSortPropertyName();
				Type type = OwningGrid.DataConnection.DataType.GetNestedPropertyType(sortPropertyName);
				if (type.IsNullableType())
				{
					type = type.GetNonNullableType();
				}
				if (!typeof(IComparable).IsAssignableFrom(type))
				{
					return false;
				}
				return true;
			}
			return true;
		}
		set
		{
			CanUserSortInternal = value;
		}
	}

	public int DisplayIndex
	{
		get
		{
			if (OwningGrid != null && OwningGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented)
			{
				return _displayIndexWithFiller - 1;
			}
			return _displayIndexWithFiller;
		}
		set
		{
			if (value == int.MaxValue)
			{
				throw DataGridError.DataGrid.ValueMustBeLessThan("value", "DisplayIndex", int.MaxValue);
			}
			if (OwningGrid != null)
			{
				if (OwningGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented)
				{
					value++;
				}
				if (_displayIndexWithFiller != value)
				{
					if (value < 0 || value >= OwningGrid.ColumnsItemsInternal.Count)
					{
						throw DataGridError.DataGrid.ValueMustBeBetween("value", "DisplayIndex", 0, lowInclusive: true, OwningGrid.Columns.Count, highInclusive: false);
					}
					OwningGrid.OnColumnDisplayIndexChanging(this, value);
					_displayIndexWithFiller = value;
					try
					{
						OwningGrid.InDisplayIndexAdjustments = true;
						OwningGrid.OnColumnDisplayIndexChanged(this);
						OwningGrid.OnColumnDisplayIndexChanged_PostNotification();
					}
					finally
					{
						OwningGrid.InDisplayIndexAdjustments = false;
					}
				}
			}
			else
			{
				if (value < -1)
				{
					throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "DisplayIndex", -1);
				}
				_displayIndexWithFiller = value;
			}
		}
	}

	public Classes CellStyleClasses => _cellStyleClasses ?? (_cellStyleClasses = new Classes());

	public ControlTheme CellTheme
	{
		get
		{
			return _cellTheme;
		}
		set
		{
			SetAndRaise(CellThemeProperty, ref _cellTheme, value);
		}
	}

	public object Header
	{
		get
		{
			return _header;
		}
		set
		{
			SetAndRaise(HeaderProperty, ref _header, value);
		}
	}

	public IDataTemplate HeaderTemplate
	{
		get
		{
			return _headerTemplate;
		}
		set
		{
			SetAndRaise(HeaderTemplateProperty, ref _headerTemplate, value);
		}
	}

	public bool IsAutoGenerated { get; internal set; }

	public bool IsFrozen { get; internal set; }

	public virtual bool IsReadOnly
	{
		get
		{
			if (OwningGrid == null)
			{
				return _isReadOnly == true;
			}
			if (_isReadOnly.HasValue)
			{
				if (!_isReadOnly.Value)
				{
					return OwningGrid.IsReadOnly;
				}
				return true;
			}
			return OwningGrid.GetColumnReadOnlyState(this, isReadOnly: false);
		}
		set
		{
			if (value != _isReadOnly)
			{
				OwningGrid?.OnColumnReadOnlyStateChanging(this, value);
				_isReadOnly = value;
			}
		}
	}

	public double MaxWidth
	{
		get
		{
			return _maxWidth ?? double.PositiveInfinity;
		}
		set
		{
			if (value < 0.0)
			{
				throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MaxWidth", 0);
			}
			if (value < ActualMinWidth)
			{
				throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MaxWidth", "MinWidth");
			}
			if (!_maxWidth.HasValue || _maxWidth.Value != value)
			{
				double actualMaxWidth = ActualMaxWidth;
				_maxWidth = value;
				if (OwningGrid != null && OwningGrid.ColumnsInternal != null)
				{
					OwningGrid.OnColumnMaxWidthChanged(this, actualMaxWidth);
				}
			}
		}
	}

	public double MinWidth
	{
		get
		{
			return _minWidth.GetValueOrDefault();
		}
		set
		{
			if (double.IsNaN(value))
			{
				throw DataGridError.DataGrid.ValueCannotBeSetToNAN("MinWidth");
			}
			if (value < 0.0)
			{
				throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MinWidth", 0);
			}
			if (double.IsPositiveInfinity(value))
			{
				throw DataGridError.DataGrid.ValueCannotBeSetToInfinity("MinWidth");
			}
			if (value > ActualMaxWidth)
			{
				throw DataGridError.DataGrid.ValueMustBeLessThanOrEqualTo("value", "MinWidth", "MaxWidth");
			}
			if (!_minWidth.HasValue || _minWidth.Value != value)
			{
				double actualMinWidth = ActualMinWidth;
				_minWidth = value;
				if (OwningGrid != null && OwningGrid.ColumnsInternal != null)
				{
					OwningGrid.OnColumnMinWidthChanged(this, actualMinWidth);
				}
			}
		}
	}

	public DataGridLength Width
	{
		get
		{
			return GetValue(WidthProperty);
		}
		set
		{
			SetValue(WidthProperty, value);
		}
	}

	public virtual IBinding ClipboardContentBinding
	{
		get
		{
			return _clipboardContentBinding;
		}
		set
		{
			_clipboardContentBinding = value;
		}
	}

	public string SortMemberPath { get; set; }

	public object Tag { get; set; }

	public IComparer CustomSortComparer { get; set; }

	protected internal DataGridColumn()
	{
		_displayIndexWithFiller = -1;
		IsInitialDesiredWidthDetermined = false;
		InheritsWidth = true;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == IsVisibleProperty)
		{
			OwningGrid?.OnColumnVisibleStateChanging(this);
			bool newValue = change.GetNewValue<bool>();
			if (_headerCell != null)
			{
				_headerCell.IsVisible = newValue;
			}
			OwningGrid?.OnColumnVisibleStateChanged(this);
			NotifyPropertyChanged(change.Property.Name);
		}
		else
		{
			if (!(change.Property == WidthProperty))
			{
				return;
			}
			if (!_settingWidthInternally)
			{
				InheritsWidth = false;
			}
			if (_setWidthInternalNoCallback)
			{
				return;
			}
			DataGrid owningGrid = OwningGrid;
			DataGridLength value = (change as AvaloniaPropertyChangedEventArgs<DataGridLength>).NewValue.Value;
			if (owningGrid != null)
			{
				DataGridLength value2 = (change as AvaloniaPropertyChangedEventArgs<DataGridLength>).OldValue.Value;
				if (value.IsStar != value2.IsStar)
				{
					SetWidthInternalNoCallback(value);
					IsInitialDesiredWidthDetermined = false;
					owningGrid.OnColumnWidthChanged(this);
				}
				else
				{
					Resize(value2, value, userInitiated: false);
				}
			}
			else
			{
				SetWidthInternalNoCallback(value);
			}
		}
	}

	internal object GetCellValue(object item, IBinding binding)
	{
		object result = null;
		if (binding != null)
		{
			OwningGrid.ClipboardContentControl.DataContext = item;
			IDisposable disposable = OwningGrid.ClipboardContentControl.Bind(ContentControl.ContentProperty, binding);
			result = OwningGrid.ClipboardContentControl.GetValue(ContentControl.ContentProperty);
			disposable.Dispose();
		}
		return result;
	}

	public Control GetCellContent(DataGridRow dataGridRow)
	{
		dataGridRow = dataGridRow ?? throw new ArgumentNullException("dataGridRow");
		if (OwningGrid == null)
		{
			throw DataGridError.DataGrid.NoOwningGrid(GetType());
		}
		if (dataGridRow.OwningGrid == OwningGrid)
		{
			DataGridCell dataGridCell = dataGridRow.Cells[Index];
			if (dataGridCell != null)
			{
				return dataGridCell.Content as Control;
			}
		}
		return null;
	}

	public Control GetCellContent(object dataItem)
	{
		dataItem = dataItem ?? throw new ArgumentNullException("dataItem");
		if (OwningGrid == null)
		{
			throw DataGridError.DataGrid.NoOwningGrid(GetType());
		}
		DataGridRow rowFromItem = OwningGrid.GetRowFromItem(dataItem);
		if (rowFromItem == null)
		{
			return null;
		}
		return GetCellContent(rowFromItem);
	}

	public static DataGridColumn GetColumnContainingElement(Control element)
	{
		for (Visual visual = element; visual != null; visual = visual.GetVisualParent())
		{
			if (visual is DataGridCell dataGridCell)
			{
				return dataGridCell.OwningColumn;
			}
			if (visual is DataGridColumnHeader dataGridColumnHeader)
			{
				return dataGridColumnHeader.OwningColumn;
			}
		}
		return null;
	}

	public void ClearSort()
	{
		_headerCell?.InvokeProcessSort(KeyboardHelper.GetPlatformCtrlOrCmdKeyModifier(OwningGrid), null);
	}

	public void Sort()
	{
		_headerCell?.InvokeProcessSort(KeyModifiers.None, null);
	}

	public void Sort(ListSortDirection direction)
	{
		_headerCell?.InvokeProcessSort(KeyModifiers.None, direction);
	}

	protected virtual void CancelCellEdit(Control editingElement, object uneditedValue)
	{
	}

	protected abstract Control GenerateEditingElement(DataGridCell cell, object dataItem, out ICellEditBinding binding);

	protected abstract Control GenerateElement(DataGridCell cell, object dataItem);

	protected void NotifyPropertyChanged(string propertyName)
	{
		OwningGrid?.RefreshColumnElements(this, propertyName);
	}

	protected abstract object PrepareCellForEdit(Control editingElement, RoutedEventArgs editingEventArgs);

	protected internal virtual void RefreshCellContent(Control element, string propertyName)
	{
	}

	protected virtual void EndCellEdit()
	{
	}

	internal void CancelCellEditInternal(Control editingElement, object uneditedValue)
	{
		CancelCellEdit(editingElement, uneditedValue);
	}

	internal void EndCellEditInternal()
	{
		EndCellEdit();
	}

	private static DataGridLength CoerceWidth(AvaloniaObject source, DataGridLength width)
	{
		DataGridColumn target = (DataGridColumn)source;
		if (target._setWidthInternalNoCallback)
		{
			return width;
		}
		if (!target.IsSet(WidthProperty))
		{
			return target.OwningGrid?.ColumnWidth ?? DataGridLength.Auto;
		}
		double num = width.DesiredValue;
		if (double.IsNaN(num))
		{
			if (!width.IsStar || target.OwningGrid == null || target.OwningGrid.ColumnsInternal == null)
			{
				num = ((!width.IsAbsolute) ? target.ActualMinWidth : width.Value);
			}
			else
			{
				double num2 = 0.0;
				double num3 = 0.0;
				double num4 = 0.0;
				foreach (DataGridColumn displayedColumn in target.OwningGrid.ColumnsInternal.GetDisplayedColumns((DataGridColumn c) => c.IsVisible && c != target && !double.IsNaN(c.Width.DesiredValue)))
				{
					if (displayedColumn.Width.IsStar)
					{
						num2 += displayedColumn.Width.Value;
						num3 += displayedColumn.Width.DesiredValue;
					}
					else
					{
						num4 += displayedColumn.ActualWidth;
					}
				}
				num = ((num2 != 0.0) ? (num3 * width.Value / num2) : Math.Max(target.ActualMinWidth, target.OwningGrid.CellsWidth - num4));
			}
		}
		double num5 = width.DisplayValue;
		if (double.IsNaN(num5))
		{
			num5 = num;
		}
		num5 = Math.Max(target.ActualMinWidth, Math.Min(target.ActualMaxWidth, num5));
		return new DataGridLength(width.Value, width.UnitType, num, num5);
	}

	internal void ComputeLayoutRoundedWidth(double leftEdge)
	{
		if (OwningGrid != null && OwningGrid.UseLayoutRounding)
		{
			double layoutScale = LayoutHelper.GetLayoutScale(HeaderCell);
			LayoutRoundedWidth = LayoutHelper.RoundLayoutSizeUp(new Size(leftEdge + ActualWidth, 1.0), layoutScale, layoutScale).Width - leftEdge;
		}
		else
		{
			LayoutRoundedWidth = ActualWidth;
		}
	}

	internal virtual DataGridColumnHeader CreateHeader()
	{
		DataGridColumnHeader dataGridColumnHeader = new DataGridColumnHeader
		{
			OwningColumn = this
		};
		dataGridColumnHeader[!ContentControl.ContentProperty] = base[!HeaderProperty];
		dataGridColumnHeader[!ContentControl.ContentTemplateProperty] = base[!HeaderTemplateProperty];
		ControlTheme columnHeaderTheme = OwningGrid.ColumnHeaderTheme;
		if (columnHeaderTheme != null)
		{
			dataGridColumnHeader.SetValue(StyledElement.ThemeProperty, columnHeaderTheme, BindingPriority.Template);
		}
		return dataGridColumnHeader;
	}

	internal void EnsureWidth()
	{
		SetWidthInternalNoCallback(CoerceWidth(this, Width));
	}

	internal Control GenerateElementInternal(DataGridCell cell, object dataItem)
	{
		return GenerateElement(cell, dataItem);
	}

	internal object PrepareCellForEditInternal(Control editingElement, RoutedEventArgs editingEventArgs)
	{
		object result = PrepareCellForEdit(editingElement, editingEventArgs);
		editingElement.Focus();
		return result;
	}

	internal void Resize(DataGridLength oldWidth, DataGridLength newWidth, bool userInitiated)
	{
		double val = newWidth.Value;
		double desiredValue = newWidth.DesiredValue;
		double num = Math.Max(ActualMinWidth, Math.Min(ActualMaxWidth, newWidth.DisplayValue));
		DataGridLengthUnitType dataGridLengthUnitType = newWidth.UnitType;
		int num2 = 0;
		double num3 = 0.0;
		foreach (DataGridColumn visibleColumn in OwningGrid.ColumnsInternal.GetVisibleColumns())
		{
			visibleColumn.EnsureWidth();
			num3 += visibleColumn.ActualWidth;
			num2 += ((visibleColumn != this && visibleColumn.Width.IsStar) ? 1 : 0);
		}
		bool flag = !OwningGrid.RowsPresenterAvailableSize.HasValue || double.IsPositiveInfinity(OwningGrid.RowsPresenterAvailableSize.Value.Width);
		if (!flag && (num2 > 0 || (dataGridLengthUnitType == DataGridLengthUnitType.Star && newWidth.IsStar && userInitiated)))
		{
			double displayValue = oldWidth.DisplayValue;
			double num4 = Math.Max(0.0, OwningGrid.CellsWidth - num3);
			double num5 = num - oldWidth.DisplayValue;
			if (!(num5 > num4))
			{
				displayValue = ((!(num5 > 0.0)) ? (displayValue + (num5 + OwningGrid.IncreaseColumnWidths(DisplayIndex + 1, 0.0 - num5, userInitiated))) : (displayValue + num5));
			}
			else
			{
				num5 -= num4;
				double num6 = num5 + OwningGrid.DecreaseColumnWidths(DisplayIndex + 1, 0.0 - num5, userInitiated);
				displayValue += num4 + num6;
			}
			if (ActualCanUserResize || (oldWidth.IsStar && !userInitiated))
			{
				num = displayValue;
			}
		}
		if (userInitiated)
		{
			desiredValue = num;
			if (!Width.IsStar)
			{
				InheritsWidth = false;
				val = num;
				dataGridLengthUnitType = DataGridLengthUnitType.Pixel;
			}
			else if (num2 > 0 && !flag)
			{
				InheritsWidth = false;
				val = Width.Value * num / ActualWidth;
			}
		}
		num = Math.Min(double.MaxValue, val);
		newWidth = new DataGridLength(num, dataGridLengthUnitType, desiredValue, num);
		SetWidthInternalNoCallback(newWidth);
		if (newWidth != oldWidth)
		{
			OwningGrid.OnColumnWidthChanged(this);
		}
	}

	internal void SetWidthDesiredValue(double desiredValue)
	{
		SetWidthInternalNoCallback(new DataGridLength(Width.Value, Width.UnitType, desiredValue, Width.DisplayValue));
	}

	internal void SetWidthDisplayValue(double displayValue)
	{
		SetWidthInternalNoCallback(new DataGridLength(Width.Value, Width.UnitType, Width.DesiredValue, displayValue));
	}

	internal void SetWidthInternal(DataGridLength width)
	{
		bool settingWidthInternally = _settingWidthInternally;
		_settingWidthInternally = true;
		try
		{
			Width = width;
		}
		finally
		{
			_settingWidthInternally = settingWidthInternally;
		}
	}

	internal void SetWidthInternalNoCallback(DataGridLength width)
	{
		bool setWidthInternalNoCallback = _setWidthInternalNoCallback;
		_setWidthInternalNoCallback = true;
		try
		{
			Width = width;
		}
		finally
		{
			_setWidthInternalNoCallback = setWidthInternalNoCallback;
		}
	}

	internal void SetWidthStarValue(double value)
	{
		InheritsWidth = false;
		SetWidthInternalNoCallback(new DataGridLength(value, Width.UnitType, Width.DesiredValue, Width.DisplayValue));
	}

	internal Control GenerateEditingElementInternal(DataGridCell cell, object dataItem)
	{
		if (_editingElement == null)
		{
			_editingElement = GenerateEditingElement(cell, dataItem, out _editBinding);
		}
		return _editingElement;
	}

	internal void RemoveEditingElement()
	{
		_editingElement = null;
	}

	internal DataGridSortDescription GetSortDescription()
	{
		if (OwningGrid != null && OwningGrid.DataConnection != null && OwningGrid.DataConnection.SortDescriptions != null)
		{
			if (CustomSortComparer != null)
			{
				return OwningGrid.DataConnection.SortDescriptions.OfType<DataGridComparerSortDescription>().FirstOrDefault((DataGridComparerSortDescription s) => s.SourceComparer == CustomSortComparer);
			}
			string propertyName = GetSortPropertyName();
			return OwningGrid.DataConnection.SortDescriptions.FirstOrDefault((DataGridSortDescription s) => s.HasPropertyPath && s.PropertyPath == propertyName);
		}
		return null;
	}

	internal string GetSortPropertyName()
	{
		string text = SortMemberPath;
		if (string.IsNullOrEmpty(text) && this is DataGridBoundColumn dataGridBoundColumn)
		{
			if (dataGridBoundColumn.Binding is Binding binding)
			{
				text = binding.Path;
			}
			else if (dataGridBoundColumn.Binding is CompiledBindingExtension compiledBindingExtension)
			{
				text = compiledBindingExtension.Path.ToString();
			}
		}
		return text;
	}

	static DataGridColumn()
	{
		Func<AvaloniaObject, DataGridLength, DataGridLength> coerce = CoerceWidth;
		WidthProperty = AvaloniaProperty.Register<DataGridColumn, DataGridLength>("Width", default(DataGridLength), inherits: false, BindingMode.OneWay, null, coerce);
	}
}
