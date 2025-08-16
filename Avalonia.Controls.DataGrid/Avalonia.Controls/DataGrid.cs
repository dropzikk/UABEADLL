using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using Avalonia.Collections;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Controls.Utils;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Reactive;
using Avalonia.Styling;
using Avalonia.Utilities;
using Avalonia.VisualTree;

namespace Avalonia.Controls;

[TemplatePart("PART_BottomRightCorner", typeof(Visual))]
[TemplatePart("PART_ColumnHeadersPresenter", typeof(DataGridColumnHeadersPresenter))]
[TemplatePart("PART_FrozenColumnScrollBarSpacer", typeof(Control))]
[TemplatePart("PART_HorizontalScrollbar", typeof(ScrollBar))]
[TemplatePart("PART_RowsPresenter", typeof(DataGridRowsPresenter))]
[TemplatePart("PART_TopLeftCornerHeader", typeof(ContentControl))]
[TemplatePart("PART_TopRightCornerHeader", typeof(ContentControl))]
[TemplatePart("PART_VerticalScrollbar", typeof(ScrollBar))]
[PseudoClasses(new string[] { ":invalid", ":empty-rows", ":empty-columns" })]
public class DataGrid : TemplatedControl
{
	public class DisplayIndexComparer : IComparer<DataGridColumn>
	{
		int IComparer<DataGridColumn>.Compare(DataGridColumn x, DataGridColumn y)
		{
			if (x.DisplayIndexWithFiller >= y.DisplayIndexWithFiller)
			{
				return 1;
			}
			return -1;
		}
	}

	private const string DATAGRID_elementRowsPresenterName = "PART_RowsPresenter";

	private const string DATAGRID_elementColumnHeadersPresenterName = "PART_ColumnHeadersPresenter";

	private const string DATAGRID_elementFrozenColumnScrollBarSpacerName = "PART_FrozenColumnScrollBarSpacer";

	private const string DATAGRID_elementHorizontalScrollbarName = "PART_HorizontalScrollbar";

	private const string DATAGRID_elementTopLeftCornerHeaderName = "PART_TopLeftCornerHeader";

	private const string DATAGRID_elementTopRightCornerHeaderName = "PART_TopRightCornerHeader";

	private const string DATAGRID_elementBottomRightCornerHeaderName = "PART_BottomRightCorner";

	private const string DATAGRID_elementVerticalScrollbarName = "PART_VerticalScrollbar";

	internal const bool DATAGRID_defaultCanUserReorderColumns = true;

	internal const bool DATAGRID_defaultCanUserResizeColumns = true;

	internal const bool DATAGRID_defaultCanUserSortColumns = true;

	private const int DATAGRID_defaultColumnDisplayOrder = 10000;

	private const double DATAGRID_horizontalGridLinesThickness = 1.0;

	private const double DATAGRID_minimumRowHeaderWidth = 4.0;

	private const double DATAGRID_minimumColumnHeaderHeight = 4.0;

	internal const double DATAGRID_maximumStarColumnWidth = 10000.0;

	internal const double DATAGRID_minimumStarColumnWidth = 0.001;

	private const double DATAGRID_mouseWheelDelta = 50.0;

	private const double DATAGRID_maxHeadersThickness = 32768.0;

	private const double DATAGRID_defaultRowHeight = 22.0;

	internal const double DATAGRID_defaultRowGroupSublevelIndent = 20.0;

	private const double DATAGRID_defaultMinColumnWidth = 20.0;

	private const double DATAGRID_defaultMaxColumnWidth = double.PositiveInfinity;

	private List<Exception> _bindingValidationErrors;

	private IDisposable _validationSubscription;

	private INotifyCollectionChanged _topLevelGroup;

	private ContentControl _clipboardContentControl;

	private Visual _bottomRightCorner;

	private DataGridColumnHeadersPresenter _columnHeadersPresenter;

	private DataGridRowsPresenter _rowsPresenter;

	private ScrollBar _vScrollBar;

	private ScrollBar _hScrollBar;

	private ContentControl _topLeftCornerHeader;

	private ContentControl _topRightCornerHeader;

	private Control _frozenColumnScrollBarSpacer;

	private double _horizontalOffset;

	private double _negHorizontalOffset;

	private byte _autoGeneratingColumnOperationCount;

	private bool _areHandlersSuspended;

	private bool _autoSizingColumns;

	private IndexToValueTable<bool> _collapsedSlotsTable;

	private Control _clickedElement;

	private int _desiredCurrentColumnIndex;

	private int _editingColumnIndex;

	private RoutedEventArgs _editingEventArgs;

	private bool _executingLostFocusActions;

	private bool _flushCurrentCellChanged;

	private bool _focusEditingControl;

	private Visual _focusedObject;

	private byte _horizontalScrollChangesIgnored;

	private DataGridRow _focusedRow;

	private bool _ignoreNextScrollBarsLayout;

	private int _lastEstimatedRow;

	private List<DataGridRow> _loadedRows;

	private Queue<Action> _lostFocusActions;

	private int _noSelectionChangeCount;

	private int _noCurrentCellChangeCount;

	private bool _makeFirstDisplayedCellCurrentCellPending;

	private bool _measured;

	private int? _mouseOverRowIndex;

	private DataGridColumn _previousCurrentColumn;

	private object _previousCurrentItem;

	private double[] _rowGroupHeightsByLevel;

	private double _rowHeaderDesiredWidth;

	private Size? _rowsPresenterAvailableSize;

	private bool _scrollingByHeight;

	private IndexToValueTable<bool> _showDetailsTable;

	private bool _successfullyUpdatedSelection;

	private DataGridSelectedItemsCollection _selectedItems;

	private bool _temporarilyResetCurrentCell;

	private object _uneditedValue;

	private double _verticalOffset;

	private byte _verticalScrollChangesIgnored;

	public static readonly StyledProperty<bool> CanUserReorderColumnsProperty;

	public static readonly StyledProperty<bool> CanUserResizeColumnsProperty;

	public static readonly StyledProperty<bool> CanUserSortColumnsProperty;

	public static readonly StyledProperty<double> ColumnHeaderHeightProperty;

	public static readonly StyledProperty<DataGridLength> ColumnWidthProperty;

	public static readonly StyledProperty<ControlTheme> RowThemeProperty;

	public static readonly StyledProperty<ControlTheme> CellThemeProperty;

	public static readonly StyledProperty<ControlTheme> ColumnHeaderThemeProperty;

	public static readonly StyledProperty<ControlTheme> RowGroupThemeProperty;

	public static readonly StyledProperty<int> FrozenColumnCountProperty;

	public static readonly StyledProperty<DataGridGridLinesVisibility> GridLinesVisibilityProperty;

	public static readonly StyledProperty<DataGridHeadersVisibility> HeadersVisibilityProperty;

	public static readonly StyledProperty<IBrush> HorizontalGridLinesBrushProperty;

	public static readonly StyledProperty<ScrollBarVisibility> HorizontalScrollBarVisibilityProperty;

	public static readonly StyledProperty<bool> IsReadOnlyProperty;

	public static readonly StyledProperty<bool> AreRowGroupHeadersFrozenProperty;

	private bool _isValid = true;

	public static readonly DirectProperty<DataGrid, bool> IsValidProperty;

	public static readonly StyledProperty<double> MaxColumnWidthProperty;

	public static readonly StyledProperty<double> MinColumnWidthProperty;

	public static readonly StyledProperty<IBrush> RowBackgroundProperty;

	public static readonly StyledProperty<double> RowHeightProperty;

	public static readonly StyledProperty<double> RowHeaderWidthProperty;

	public static readonly StyledProperty<DataGridSelectionMode> SelectionModeProperty;

	public static readonly StyledProperty<IBrush> VerticalGridLinesBrushProperty;

	public static readonly StyledProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty;

	public static readonly StyledProperty<ITemplate<Control>> DropLocationIndicatorTemplateProperty;

	private int _selectedIndex = -1;

	private object _selectedItem;

	public static readonly DirectProperty<DataGrid, int> SelectedIndexProperty;

	public static readonly DirectProperty<DataGrid, object> SelectedItemProperty;

	public static readonly StyledProperty<DataGridClipboardCopyMode> ClipboardCopyModeProperty;

	public static readonly StyledProperty<bool> AutoGenerateColumnsProperty;

	public static readonly StyledProperty<IEnumerable> ItemsSourceProperty;

	public static readonly StyledProperty<bool> AreRowDetailsFrozenProperty;

	public static readonly StyledProperty<IDataTemplate> RowDetailsTemplateProperty;

	public static readonly StyledProperty<DataGridRowDetailsVisibilityMode> RowDetailsVisibilityModeProperty;

	public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent;

	public bool CanUserReorderColumns
	{
		get
		{
			return GetValue(CanUserReorderColumnsProperty);
		}
		set
		{
			SetValue(CanUserReorderColumnsProperty, value);
		}
	}

	public bool CanUserResizeColumns
	{
		get
		{
			return GetValue(CanUserResizeColumnsProperty);
		}
		set
		{
			SetValue(CanUserResizeColumnsProperty, value);
		}
	}

	public bool CanUserSortColumns
	{
		get
		{
			return GetValue(CanUserSortColumnsProperty);
		}
		set
		{
			SetValue(CanUserSortColumnsProperty, value);
		}
	}

	public double ColumnHeaderHeight
	{
		get
		{
			return GetValue(ColumnHeaderHeightProperty);
		}
		set
		{
			SetValue(ColumnHeaderHeightProperty, value);
		}
	}

	public ControlTheme RowTheme
	{
		get
		{
			return GetValue(RowThemeProperty);
		}
		set
		{
			SetValue(RowThemeProperty, value);
		}
	}

	public ControlTheme CellTheme
	{
		get
		{
			return GetValue(CellThemeProperty);
		}
		set
		{
			SetValue(CellThemeProperty, value);
		}
	}

	public ControlTheme ColumnHeaderTheme
	{
		get
		{
			return GetValue(ColumnHeaderThemeProperty);
		}
		set
		{
			SetValue(ColumnHeaderThemeProperty, value);
		}
	}

	public ControlTheme RowGroupTheme
	{
		get
		{
			return GetValue(RowGroupThemeProperty);
		}
		set
		{
			SetValue(RowGroupThemeProperty, value);
		}
	}

	public DataGridLength ColumnWidth
	{
		get
		{
			return GetValue(ColumnWidthProperty);
		}
		set
		{
			SetValue(ColumnWidthProperty, value);
		}
	}

	public int FrozenColumnCount
	{
		get
		{
			return GetValue(FrozenColumnCountProperty);
		}
		set
		{
			SetValue(FrozenColumnCountProperty, value);
		}
	}

	public DataGridGridLinesVisibility GridLinesVisibility
	{
		get
		{
			return GetValue(GridLinesVisibilityProperty);
		}
		set
		{
			SetValue(GridLinesVisibilityProperty, value);
		}
	}

	public DataGridHeadersVisibility HeadersVisibility
	{
		get
		{
			return GetValue(HeadersVisibilityProperty);
		}
		set
		{
			SetValue(HeadersVisibilityProperty, value);
		}
	}

	public IBrush HorizontalGridLinesBrush
	{
		get
		{
			return GetValue(HorizontalGridLinesBrushProperty);
		}
		set
		{
			SetValue(HorizontalGridLinesBrushProperty, value);
		}
	}

	public ScrollBarVisibility HorizontalScrollBarVisibility
	{
		get
		{
			return GetValue(HorizontalScrollBarVisibilityProperty);
		}
		set
		{
			SetValue(HorizontalScrollBarVisibilityProperty, value);
		}
	}

	public bool IsReadOnly
	{
		get
		{
			return GetValue(IsReadOnlyProperty);
		}
		set
		{
			SetValue(IsReadOnlyProperty, value);
		}
	}

	public bool AreRowGroupHeadersFrozen
	{
		get
		{
			return GetValue(AreRowGroupHeadersFrozenProperty);
		}
		set
		{
			SetValue(AreRowGroupHeadersFrozenProperty, value);
		}
	}

	public bool IsValid
	{
		get
		{
			return _isValid;
		}
		internal set
		{
			SetAndRaise(IsValidProperty, ref _isValid, value);
			base.PseudoClasses.Set(":invalid", !value);
		}
	}

	public double MaxColumnWidth
	{
		get
		{
			return GetValue(MaxColumnWidthProperty);
		}
		set
		{
			SetValue(MaxColumnWidthProperty, value);
		}
	}

	public double MinColumnWidth
	{
		get
		{
			return GetValue(MinColumnWidthProperty);
		}
		set
		{
			SetValue(MinColumnWidthProperty, value);
		}
	}

	public IBrush RowBackground
	{
		get
		{
			return GetValue(RowBackgroundProperty);
		}
		set
		{
			SetValue(RowBackgroundProperty, value);
		}
	}

	public double RowHeight
	{
		get
		{
			return GetValue(RowHeightProperty);
		}
		set
		{
			SetValue(RowHeightProperty, value);
		}
	}

	public double RowHeaderWidth
	{
		get
		{
			return GetValue(RowHeaderWidthProperty);
		}
		set
		{
			SetValue(RowHeaderWidthProperty, value);
		}
	}

	public DataGridSelectionMode SelectionMode
	{
		get
		{
			return GetValue(SelectionModeProperty);
		}
		set
		{
			SetValue(SelectionModeProperty, value);
		}
	}

	public IBrush VerticalGridLinesBrush
	{
		get
		{
			return GetValue(VerticalGridLinesBrushProperty);
		}
		set
		{
			SetValue(VerticalGridLinesBrushProperty, value);
		}
	}

	public ScrollBarVisibility VerticalScrollBarVisibility
	{
		get
		{
			return GetValue(VerticalScrollBarVisibilityProperty);
		}
		set
		{
			SetValue(VerticalScrollBarVisibilityProperty, value);
		}
	}

	public ITemplate<Control> DropLocationIndicatorTemplate
	{
		get
		{
			return GetValue(DropLocationIndicatorTemplateProperty);
		}
		set
		{
			SetValue(DropLocationIndicatorTemplateProperty, value);
		}
	}

	public int SelectedIndex
	{
		get
		{
			return _selectedIndex;
		}
		set
		{
			SetAndRaise(SelectedIndexProperty, ref _selectedIndex, value);
		}
	}

	public object SelectedItem
	{
		get
		{
			return _selectedItem;
		}
		set
		{
			SetAndRaise(SelectedItemProperty, ref _selectedItem, value);
		}
	}

	public DataGridClipboardCopyMode ClipboardCopyMode
	{
		get
		{
			return GetValue(ClipboardCopyModeProperty);
		}
		set
		{
			SetValue(ClipboardCopyModeProperty, value);
		}
	}

	public bool AutoGenerateColumns
	{
		get
		{
			return GetValue(AutoGenerateColumnsProperty);
		}
		set
		{
			SetValue(AutoGenerateColumnsProperty, value);
		}
	}

	public IEnumerable ItemsSource
	{
		get
		{
			return GetValue(ItemsSourceProperty);
		}
		set
		{
			SetValue(ItemsSourceProperty, value);
		}
	}

	public bool AreRowDetailsFrozen
	{
		get
		{
			return GetValue(AreRowDetailsFrozenProperty);
		}
		set
		{
			SetValue(AreRowDetailsFrozenProperty, value);
		}
	}

	public IDataTemplate RowDetailsTemplate
	{
		get
		{
			return GetValue(RowDetailsTemplateProperty);
		}
		set
		{
			SetValue(RowDetailsTemplateProperty, value);
		}
	}

	public DataGridRowDetailsVisibilityMode RowDetailsVisibilityMode
	{
		get
		{
			return GetValue(RowDetailsVisibilityModeProperty);
		}
		set
		{
			SetValue(RowDetailsVisibilityModeProperty, value);
		}
	}

	public ObservableCollection<DataGridColumn> Columns => ColumnsInternal;

	public DataGridColumn CurrentColumn
	{
		get
		{
			if (CurrentColumnIndex == -1)
			{
				return null;
			}
			return ColumnsItemsInternal[CurrentColumnIndex];
		}
		set
		{
			if (value == null)
			{
				throw DataGridError.DataGrid.ValueCannotBeSetToNull("value", "CurrentColumn");
			}
			if (CurrentColumn == value)
			{
				return;
			}
			if (value.OwningGrid != this)
			{
				throw DataGridError.DataGrid.ColumnNotInThisDataGrid();
			}
			if (!value.IsVisible)
			{
				throw DataGridError.DataGrid.ColumnCannotBeCollapsed();
			}
			if (CurrentSlot == -1)
			{
				throw DataGridError.DataGrid.NoCurrentRow();
			}
			bool flag = _editingColumnIndex != -1;
			if (EndCellEdit(DataGridEditAction.Commit, exitEditingMode: true, ContainsFocus, raiseEvents: true))
			{
				UpdateSelectionAndCurrency(value.Index, CurrentSlot, DataGridSelectionAction.None, scrollIntoView: false);
				if (flag && _editingColumnIndex == -1 && CurrentSlot != -1 && CurrentColumnIndex != -1 && CurrentColumnIndex == value.Index && value.OwningGrid == this && !GetColumnEffectiveReadOnlyState(value))
				{
					BeginCellEdit(new RoutedEventArgs());
				}
			}
		}
	}

	public IList SelectedItems => _selectedItems;

	internal DataGridColumnCollection ColumnsInternal { get; }

	internal int AnchorSlot { get; private set; }

	internal double ActualRowHeaderWidth
	{
		get
		{
			if (!AreRowHeadersVisible)
			{
				return 0.0;
			}
			if (double.IsNaN(RowHeaderWidth))
			{
				return RowHeadersDesiredWidth;
			}
			return RowHeaderWidth;
		}
	}

	internal double ActualRowsPresenterHeight
	{
		get
		{
			if (_rowsPresenter != null)
			{
				return _rowsPresenter.Bounds.Height;
			}
			return 0.0;
		}
	}

	internal bool AreColumnHeadersVisible => (HeadersVisibility & DataGridHeadersVisibility.Column) == DataGridHeadersVisibility.Column;

	internal bool AreRowHeadersVisible => (HeadersVisibility & DataGridHeadersVisibility.Row) == DataGridHeadersVisibility.Row;

	internal bool AutoSizingColumns
	{
		get
		{
			return _autoSizingColumns;
		}
		set
		{
			if (_autoSizingColumns && !value && ColumnsInternal != null)
			{
				double amount = CellsWidth - ColumnsInternal.VisibleEdgedColumnsWidth;
				AdjustColumnWidths(0, amount, userInitiated: false);
				foreach (DataGridColumn visibleColumn in ColumnsInternal.GetVisibleColumns())
				{
					visibleColumn.IsInitialDesiredWidthDetermined = true;
				}
				ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
				ComputeScrollBarsLayout();
				InvalidateColumnHeadersMeasure();
				InvalidateRowsMeasure(invalidateIndividualElements: true);
			}
			_autoSizingColumns = value;
		}
	}

	internal double AvailableSlotElementRoom { get; set; }

	internal double CellsHeight => RowsPresenterEstimatedAvailableHeight.GetValueOrDefault();

	internal double CellsWidth
	{
		get
		{
			double num = double.PositiveInfinity;
			if (RowsPresenterAvailableSize.HasValue)
			{
				num = Math.Max(0.0, RowsPresenterAvailableSize.Value.Width - ActualRowHeaderWidth);
			}
			if (!double.IsPositiveInfinity(num))
			{
				return num;
			}
			return ColumnsInternal.VisibleEdgedColumnsWidth;
		}
	}

	internal DataGridColumnHeadersPresenter ColumnHeaders => _columnHeadersPresenter;

	internal List<DataGridColumn> ColumnsItemsInternal => ColumnsInternal.ItemsInternal;

	internal bool ContainsFocus { get; private set; }

	internal int CurrentColumnIndex
	{
		get
		{
			return CurrentCellCoordinates.ColumnIndex;
		}
		private set
		{
			CurrentCellCoordinates.ColumnIndex = value;
		}
	}

	internal int CurrentSlot
	{
		get
		{
			return CurrentCellCoordinates.Slot;
		}
		private set
		{
			CurrentCellCoordinates.Slot = value;
		}
	}

	internal DataGridDataConnection DataConnection { get; private set; }

	internal DataGridDisplayData DisplayData { get; private set; }

	internal int EditingColumnIndex { get; private set; }

	internal DataGridRow EditingRow { get; private set; }

	internal double FirstDisplayedScrollingColumnHiddenWidth => _negHorizontalOffset;

	internal double HorizontalAdjustment { get; private set; }

	internal static double HorizontalGridLinesThickness => 1.0;

	internal double HorizontalOffset
	{
		get
		{
			return _horizontalOffset;
		}
		set
		{
			if (value < 0.0)
			{
				value = 0.0;
			}
			double num = Math.Max(0.0, ColumnsInternal.VisibleEdgedColumnsWidth - CellsWidth);
			if (value > num)
			{
				value = num;
			}
			if (value != _horizontalOffset)
			{
				if (_hScrollBar != null && value != _hScrollBar.Value)
				{
					_hScrollBar.Value = value;
				}
				_horizontalOffset = value;
				DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
				ComputeDisplayedColumns();
			}
		}
	}

	internal ScrollBar HorizontalScrollBar => _hScrollBar;

	internal IndexToValueTable<DataGridRowGroupInfo> RowGroupHeadersTable { get; private set; }

	internal bool LoadingOrUnloadingRow { get; private set; }

	internal bool InDisplayIndexAdjustments { get; set; }

	internal int? MouseOverRowIndex
	{
		get
		{
			return _mouseOverRowIndex;
		}
		set
		{
			if (_mouseOverRowIndex == value)
			{
				return;
			}
			DataGridRow dataGridRow = null;
			if (_mouseOverRowIndex.HasValue)
			{
				int slot = SlotFromRowIndex(_mouseOverRowIndex.Value);
				if (IsSlotVisible(slot))
				{
					dataGridRow = DisplayData.GetDisplayedElement(slot) as DataGridRow;
				}
			}
			_mouseOverRowIndex = value;
			dataGridRow?.UpdatePseudoClasses();
			if (_mouseOverRowIndex.HasValue)
			{
				int slot2 = SlotFromRowIndex(_mouseOverRowIndex.Value);
				if (IsSlotVisible(slot2) && DisplayData.GetDisplayedElement(slot2) is DataGridRow dataGridRow2)
				{
					dataGridRow2.UpdatePseudoClasses();
				}
			}
		}
	}

	internal double NegVerticalOffset { get; private set; }

	internal int NoCurrentCellChangeCount
	{
		get
		{
			return _noCurrentCellChangeCount;
		}
		set
		{
			_noCurrentCellChangeCount = value;
			if (value == 0)
			{
				FlushCurrentCellChanged();
			}
		}
	}

	internal double RowDetailsHeightEstimate { get; private set; }

	internal double RowHeadersDesiredWidth
	{
		get
		{
			return _rowHeaderDesiredWidth;
		}
		set
		{
			if (_rowHeaderDesiredWidth < value)
			{
				double actualRowHeaderWidth = ActualRowHeaderWidth;
				_rowHeaderDesiredWidth = value;
				if (actualRowHeaderWidth != ActualRowHeaderWidth)
				{
					EnsureRowHeaderWidth();
				}
			}
		}
	}

	internal double RowGroupHeaderHeightEstimate { get; private set; }

	internal double RowHeightEstimate { get; private set; }

	internal Size? RowsPresenterAvailableSize
	{
		get
		{
			return _rowsPresenterAvailableSize;
		}
		set
		{
			if (_rowsPresenterAvailableSize.HasValue && value.HasValue && value.Value.Width > RowsPresenterAvailableSize.Value.Width)
			{
				double val = _horizontalOffset + value.Value.Width - ColumnsInternal.VisibleEdgedColumnsWidth;
				HorizontalAdjustment = Math.Min(HorizontalOffset, Math.Max(0.0, val));
			}
			else
			{
				HorizontalAdjustment = 0.0;
			}
			_rowsPresenterAvailableSize = value;
		}
	}

	internal double? RowsPresenterEstimatedAvailableHeight { get; set; }

	internal double[] RowGroupSublevelIndents { get; private set; }

	internal bool SelectionHasChanged { get; set; }

	internal int SlotCount { get; private set; }

	internal bool UpdatedStateOnMouseLeftButtonDown { get; set; }

	internal bool UsesStarSizing
	{
		get
		{
			if (ColumnsInternal != null)
			{
				if (ColumnsInternal.VisibleStarColumnCount > 0)
				{
					if (RowsPresenterAvailableSize.HasValue)
					{
						return !double.IsPositiveInfinity(RowsPresenterAvailableSize.Value.Width);
					}
					return true;
				}
				return false;
			}
			return false;
		}
	}

	internal ScrollBar VerticalScrollBar => _vScrollBar;

	internal int VisibleSlotCount { get; set; }

	protected object CurrentItem
	{
		get
		{
			if (CurrentSlot == -1 || ItemsSource == null || RowGroupHeadersTable.Contains(CurrentSlot))
			{
				return null;
			}
			return DataConnection.GetDataItem(RowIndexFromSlot(CurrentSlot));
		}
	}

	private DataGridCellCoordinates CurrentCellCoordinates { get; set; }

	private int FirstDisplayedNonFillerColumnIndex
	{
		get
		{
			DataGridColumn firstVisibleNonFillerColumn = ColumnsInternal.FirstVisibleNonFillerColumn;
			if (firstVisibleNonFillerColumn != null)
			{
				if (firstVisibleNonFillerColumn.IsFrozen)
				{
					return firstVisibleNonFillerColumn.Index;
				}
				if (DisplayData.FirstDisplayedScrollingCol >= firstVisibleNonFillerColumn.Index)
				{
					return DisplayData.FirstDisplayedScrollingCol;
				}
				return firstVisibleNonFillerColumn.Index;
			}
			return -1;
		}
	}

	private bool IsHorizontalScrollBarOverCells
	{
		get
		{
			if (_columnHeadersPresenter != null)
			{
				return Grid.GetColumnSpan(_columnHeadersPresenter) == 2;
			}
			return false;
		}
	}

	private bool IsVerticalScrollBarOverCells
	{
		get
		{
			if (_rowsPresenter != null)
			{
				return Grid.GetRowSpan(_rowsPresenter) == 2;
			}
			return false;
		}
	}

	private int NoSelectionChangeCount
	{
		get
		{
			return _noSelectionChangeCount;
		}
		set
		{
			_noSelectionChangeCount = value;
			if (value == 0)
			{
				FlushSelectionChanged();
			}
		}
	}

	internal ContentControl ClipboardContentControl
	{
		get
		{
			if (_clipboardContentControl == null)
			{
				_clipboardContentControl = new ContentControl();
			}
			return _clipboardContentControl;
		}
	}

	internal bool AreRowBottomGridLinesRequired
	{
		get
		{
			if (GridLinesVisibility == DataGridGridLinesVisibility.Horizontal || GridLinesVisibility == DataGridGridLinesVisibility.All)
			{
				return HorizontalGridLinesBrush != null;
			}
			return false;
		}
	}

	internal int FirstVisibleSlot
	{
		get
		{
			if (SlotCount <= 0)
			{
				return -1;
			}
			return GetNextVisibleSlot(-1);
		}
	}

	internal int FrozenColumnCountWithFiller
	{
		get
		{
			int num = FrozenColumnCount;
			if (ColumnsInternal.RowGroupSpacerColumn.IsRepresented && (AreRowGroupHeadersFrozen || num > 0))
			{
				num++;
			}
			return num;
		}
	}

	internal int LastVisibleSlot
	{
		get
		{
			if (SlotCount <= 0)
			{
				return -1;
			}
			return GetPreviousVisibleSlot(SlotCount);
		}
	}

	private double EdgedRowsHeightCalculated
	{
		get
		{
			if (DisplayData.LastScrollingSlot == -1 || double.IsPositiveInfinity(AvailableSlotElementRoom))
			{
				return 0.0;
			}
			double num = _verticalOffset - NegVerticalOffset;
			foreach (Control scrollingElement in DisplayData.GetScrollingElements())
			{
				num = ((!(scrollingElement is DataGridRow dataGridRow)) ? (num + scrollingElement.DesiredSize.Height) : (num + dataGridRow.TargetHeight));
			}
			int num2 = GetDetailsCountInclusive(0, DisplayData.LastScrollingSlot);
			num -= (double)num2 * RowDetailsHeightEstimate;
			if (DisplayData.LastScrollingSlot >= _lastEstimatedRow)
			{
				_lastEstimatedRow = DisplayData.LastScrollingSlot;
				RowHeightEstimate = num / (double)(_lastEstimatedRow + 1 - _collapsedSlotsTable.GetIndexCount(0, _lastEstimatedRow));
			}
			if (VisibleSlotCount > DisplayData.NumDisplayedScrollingElements)
			{
				int num3 = SlotCount - DisplayData.LastScrollingSlot - _collapsedSlotsTable.GetIndexCount(DisplayData.LastScrollingSlot, SlotCount - 1) - 1;
				num += RowHeightEstimate * (double)num3;
				num2 += GetDetailsCountInclusive(DisplayData.LastScrollingSlot + 1, SlotCount - 1);
			}
			double num4 = (double)num2 * RowDetailsHeightEstimate;
			return num + num4;
		}
	}

	public event EventHandler<ScrollEventArgs> HorizontalScroll;

	public event EventHandler<ScrollEventArgs> VerticalScroll;

	public event EventHandler<DataGridAutoGeneratingColumnEventArgs> AutoGeneratingColumn;

	public event EventHandler<DataGridBeginningEditEventArgs> BeginningEdit;

	public event EventHandler<DataGridCellEditEndedEventArgs> CellEditEnded;

	public event EventHandler<DataGridCellEditEndingEventArgs> CellEditEnding;

	public event EventHandler<DataGridCellPointerPressedEventArgs> CellPointerPressed;

	public event EventHandler<DataGridColumnEventArgs> ColumnDisplayIndexChanged;

	public event EventHandler<DataGridColumnEventArgs> ColumnReordered;

	public event EventHandler<DataGridColumnReorderingEventArgs> ColumnReordering;

	public event EventHandler<EventArgs> CurrentCellChanged;

	public event EventHandler<DataGridRowEventArgs> LoadingRow;

	public event EventHandler<DataGridPreparingCellForEditEventArgs> PreparingCellForEdit;

	public event EventHandler<DataGridRowEditEndedEventArgs> RowEditEnded;

	public event EventHandler<DataGridRowEditEndingEventArgs> RowEditEnding;

	public event EventHandler<SelectionChangedEventArgs> SelectionChanged
	{
		add
		{
			AddHandler(SelectionChangedEvent, value);
		}
		remove
		{
			RemoveHandler(SelectionChangedEvent, value);
		}
	}

	public event EventHandler<DataGridColumnEventArgs> Sorting;

	public event EventHandler<DataGridRowEventArgs> UnloadingRow;

	public event EventHandler<DataGridRowDetailsEventArgs> LoadingRowDetails;

	public event EventHandler<DataGridRowDetailsEventArgs> RowDetailsVisibilityChanged;

	public event EventHandler<DataGridRowDetailsEventArgs> UnloadingRowDetails;

	public event EventHandler<DataGridRowGroupHeaderEventArgs> LoadingRowGroup;

	public event EventHandler<DataGridRowGroupHeaderEventArgs> UnloadingRowGroup;

	public event EventHandler<DataGridRowClipboardEventArgs> CopyingRowClipboardContent;

	private static bool IsValidColumnHeaderHeight(double value)
	{
		if (!double.IsNaN(value))
		{
			if (value >= 4.0)
			{
				return value <= 32768.0;
			}
			return false;
		}
		return true;
	}

	private static bool ValidateFrozenColumnCount(int value)
	{
		return value >= 0;
	}

	private void OnAreRowGroupHeadersFrozenChanged(AvaloniaPropertyChangedEventArgs e)
	{
		bool num = (bool)e.NewValue;
		ProcessFrozenColumnCount();
		if (!num || _rowsPresenter == null)
		{
			return;
		}
		foreach (Control child in _rowsPresenter.Children)
		{
			if (child is DataGridRowGroupHeader dataGridRowGroupHeader)
			{
				dataGridRowGroupHeader.ClearFrozenStates();
			}
		}
	}

	private static bool IsValidColumnWidth(double value)
	{
		if (!double.IsNaN(value))
		{
			return value > 0.0;
		}
		return false;
	}

	private static bool IsValidMinColumnWidth(double value)
	{
		if (!double.IsNaN(value) && !double.IsPositiveInfinity(value))
		{
			return value >= 0.0;
		}
		return false;
	}

	private static bool IsValidRowHeight(double value)
	{
		if (!double.IsNaN(value))
		{
			if (value >= 0.0)
			{
				return value <= 65536.0;
			}
			return false;
		}
		return true;
	}

	private static bool IsValidRowHeaderWidth(double value)
	{
		if (!double.IsNaN(value))
		{
			if (value >= 4.0)
			{
				return value <= 32768.0;
			}
			return false;
		}
		return true;
	}

	private void OnAutoGenerateColumnsChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if ((bool)e.NewValue)
		{
			InitializeElements(recycleRows: false);
		}
		else
		{
			RemoveAutoGeneratedColumns();
		}
	}

	static DataGrid()
	{
		CanUserReorderColumnsProperty = AvaloniaProperty.Register<DataGrid, bool>("CanUserReorderColumns", defaultValue: false);
		CanUserResizeColumnsProperty = AvaloniaProperty.Register<DataGrid, bool>("CanUserResizeColumns", defaultValue: false);
		CanUserSortColumnsProperty = AvaloniaProperty.Register<DataGrid, bool>("CanUserSortColumns", defaultValue: true);
		ColumnHeaderHeightProperty = AvaloniaProperty.Register<DataGrid, double>("ColumnHeaderHeight", double.NaN, inherits: false, BindingMode.OneWay, IsValidColumnHeaderHeight);
		ColumnWidthProperty = AvaloniaProperty.Register<DataGrid, DataGridLength>("ColumnWidth", DataGridLength.Auto);
		RowThemeProperty = AvaloniaProperty.Register<DataGrid, ControlTheme>("RowTheme");
		CellThemeProperty = AvaloniaProperty.Register<DataGrid, ControlTheme>("CellTheme");
		ColumnHeaderThemeProperty = AvaloniaProperty.Register<DataGrid, ControlTheme>("ColumnHeaderTheme");
		RowGroupThemeProperty = AvaloniaProperty.Register<DataGrid, ControlTheme>("RowGroupTheme");
		FrozenColumnCountProperty = AvaloniaProperty.Register<DataGrid, int>("FrozenColumnCount", 0, inherits: false, BindingMode.OneWay, ValidateFrozenColumnCount);
		GridLinesVisibilityProperty = AvaloniaProperty.Register<DataGrid, DataGridGridLinesVisibility>("GridLinesVisibility", DataGridGridLinesVisibility.None);
		HeadersVisibilityProperty = AvaloniaProperty.Register<DataGrid, DataGridHeadersVisibility>("HeadersVisibility", DataGridHeadersVisibility.None);
		HorizontalGridLinesBrushProperty = AvaloniaProperty.Register<DataGrid, IBrush>("HorizontalGridLinesBrush");
		HorizontalScrollBarVisibilityProperty = AvaloniaProperty.Register<DataGrid, ScrollBarVisibility>("HorizontalScrollBarVisibility", ScrollBarVisibility.Disabled);
		IsReadOnlyProperty = AvaloniaProperty.Register<DataGrid, bool>("IsReadOnly", defaultValue: false);
		AreRowGroupHeadersFrozenProperty = AvaloniaProperty.Register<DataGrid, bool>("AreRowGroupHeadersFrozen", defaultValue: true);
		IsValidProperty = AvaloniaProperty.RegisterDirect("IsValid", (DataGrid o) => o.IsValid, null, unsetValue: false);
		MaxColumnWidthProperty = AvaloniaProperty.Register<DataGrid, double>("MaxColumnWidth", double.PositiveInfinity, inherits: false, BindingMode.OneWay, IsValidColumnWidth);
		MinColumnWidthProperty = AvaloniaProperty.Register<DataGrid, double>("MinColumnWidth", 20.0, inherits: false, BindingMode.OneWay, IsValidMinColumnWidth);
		RowBackgroundProperty = AvaloniaProperty.Register<DataGrid, IBrush>("RowBackground");
		RowHeightProperty = AvaloniaProperty.Register<DataGrid, double>("RowHeight", double.NaN, inherits: false, BindingMode.OneWay, IsValidRowHeight);
		RowHeaderWidthProperty = AvaloniaProperty.Register<DataGrid, double>("RowHeaderWidth", double.NaN, inherits: false, BindingMode.OneWay, IsValidRowHeaderWidth);
		SelectionModeProperty = AvaloniaProperty.Register<DataGrid, DataGridSelectionMode>("SelectionMode", DataGridSelectionMode.Extended);
		VerticalGridLinesBrushProperty = AvaloniaProperty.Register<DataGrid, IBrush>("VerticalGridLinesBrush");
		VerticalScrollBarVisibilityProperty = AvaloniaProperty.Register<DataGrid, ScrollBarVisibility>("VerticalScrollBarVisibility", ScrollBarVisibility.Disabled);
		DropLocationIndicatorTemplateProperty = AvaloniaProperty.Register<DataGrid, ITemplate<Control>>("DropLocationIndicatorTemplate");
		SelectedIndexProperty = AvaloniaProperty.RegisterDirect("SelectedIndex", (DataGrid o) => o.SelectedIndex, delegate(DataGrid o, int v)
		{
			o.SelectedIndex = v;
		}, 0, BindingMode.TwoWay);
		SelectedItemProperty = AvaloniaProperty.RegisterDirect("SelectedItem", (DataGrid o) => o.SelectedItem, delegate(DataGrid o, object v)
		{
			o.SelectedItem = v;
		}, null, BindingMode.TwoWay);
		ClipboardCopyModeProperty = AvaloniaProperty.Register<DataGrid, DataGridClipboardCopyMode>("ClipboardCopyMode", DataGridClipboardCopyMode.ExcludeHeader);
		AutoGenerateColumnsProperty = AvaloniaProperty.Register<DataGrid, bool>("AutoGenerateColumns", defaultValue: false);
		ItemsSourceProperty = AvaloniaProperty.Register<DataGrid, IEnumerable>("ItemsSource");
		AreRowDetailsFrozenProperty = AvaloniaProperty.Register<DataGrid, bool>("AreRowDetailsFrozen", defaultValue: false);
		RowDetailsTemplateProperty = AvaloniaProperty.Register<DataGrid, IDataTemplate>("RowDetailsTemplate");
		RowDetailsVisibilityModeProperty = AvaloniaProperty.Register<DataGrid, DataGridRowDetailsVisibilityMode>("RowDetailsVisibilityMode", DataGridRowDetailsVisibilityMode.VisibleWhenSelected);
		SelectionChangedEvent = RoutedEvent.Register<DataGrid, SelectionChangedEventArgs>("SelectionChanged", RoutingStrategies.Bubble);
		Layoutable.AffectsMeasure<DataGrid>(new AvaloniaProperty[3] { ColumnHeaderHeightProperty, HorizontalScrollBarVisibilityProperty, VerticalScrollBarVisibilityProperty });
		ItemsSourceProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnItemsSourcePropertyChanged(e);
		});
		CanUserResizeColumnsProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnCanUserResizeColumnsChanged(e);
		});
		ColumnWidthProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnColumnWidthChanged(e);
		});
		FrozenColumnCountProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnFrozenColumnCountChanged(e);
		});
		GridLinesVisibilityProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnGridLinesVisibilityChanged(e);
		});
		HeadersVisibilityProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnHeadersVisibilityChanged(e);
		});
		HorizontalGridLinesBrushProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnHorizontalGridLinesBrushChanged(e);
		});
		IsReadOnlyProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnIsReadOnlyChanged(e);
		});
		MaxColumnWidthProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnMaxColumnWidthChanged(e);
		});
		MinColumnWidthProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnMinColumnWidthChanged(e);
		});
		RowHeightProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnRowHeightChanged(e);
		});
		RowHeaderWidthProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnRowHeaderWidthChanged(e);
		});
		SelectionModeProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnSelectionModeChanged(e);
		});
		VerticalGridLinesBrushProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnVerticalGridLinesBrushChanged(e);
		});
		SelectedIndexProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnSelectedIndexChanged(e);
		});
		SelectedItemProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnSelectedItemChanged(e);
		});
		InputElement.IsEnabledProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.DataGrid_IsEnabledChanged(e);
		});
		AreRowGroupHeadersFrozenProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnAreRowGroupHeadersFrozenChanged(e);
		});
		RowDetailsTemplateProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnRowDetailsTemplateChanged(e);
		});
		RowDetailsVisibilityModeProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnRowDetailsVisibilityModeChanged(e);
		});
		AutoGenerateColumnsProperty.Changed.AddClassHandler(delegate(DataGrid x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnAutoGenerateColumnsChanged(e);
		});
		InputElement.FocusableProperty.OverrideDefaultValue<DataGrid>(defaultValue: true);
	}

	public DataGrid()
	{
		base.KeyDown += DataGrid_KeyDown;
		base.KeyUp += DataGrid_KeyUp;
		base.GotFocus += DataGrid_GotFocus;
		base.LostFocus += DataGrid_LostFocus;
		_loadedRows = new List<DataGridRow>();
		_lostFocusActions = new Queue<Action>();
		_selectedItems = new DataGridSelectedItemsCollection(this);
		RowGroupHeadersTable = new IndexToValueTable<DataGridRowGroupInfo>();
		_bindingValidationErrors = new List<Exception>();
		DisplayData = new DataGridDisplayData(this);
		ColumnsInternal = CreateColumnsInstance();
		ColumnsInternal.CollectionChanged += ColumnsInternal_CollectionChanged;
		RowHeightEstimate = 22.0;
		RowDetailsHeightEstimate = 0.0;
		_rowHeaderDesiredWidth = 0.0;
		DataConnection = new DataGridDataConnection(this);
		_showDetailsTable = new IndexToValueTable<bool>();
		_collapsedSlotsTable = new IndexToValueTable<bool>();
		AnchorSlot = -1;
		_lastEstimatedRow = -1;
		_editingColumnIndex = -1;
		_mouseOverRowIndex = null;
		CurrentCellCoordinates = new DataGridCellCoordinates(-1, -1);
		RowGroupHeaderHeightEstimate = 22.0;
		UpdatePseudoClasses();
	}

	private void SetValueNoCallback<T>(AvaloniaProperty<T> property, T value, BindingPriority priority = BindingPriority.LocalValue)
	{
		_areHandlersSuspended = true;
		try
		{
			SetValue(property, value, priority);
		}
		finally
		{
			_areHandlersSuspended = false;
		}
	}

	private void OnRowDetailsVisibilityModeChanged(AvaloniaPropertyChangedEventArgs e)
	{
		UpdateRowDetailsVisibilityMode((DataGridRowDetailsVisibilityMode)e.NewValue);
	}

	private void OnRowDetailsTemplateChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (_rowsPresenter != null)
		{
			foreach (DataGridRow allRow in GetAllRows())
			{
				if (GetRowDetailsVisibility(allRow.Index))
				{
					allRow.ApplyDetailsTemplate(initializeDetailsPreferredHeight: false);
				}
			}
		}
		UpdateRowDetailsHeightEstimate();
		InvalidateMeasure();
	}

	private void OnItemsSourcePropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (_areHandlersSuspended)
		{
			return;
		}
		IEnumerable value = (IEnumerable)e.OldValue;
		IEnumerable enumerable = (IEnumerable)e.NewValue;
		if (LoadingOrUnloadingRow)
		{
			SetValueNoCallback(ItemsSourceProperty, value);
			throw DataGridError.DataGrid.CannotChangeItemsWhenLoadingRows();
		}
		if (!CommitEdit())
		{
			CancelEdit(DataGridEditingUnit.Row, raiseEvents: false);
		}
		DataConnection.UnWireEvents(DataConnection.DataSource);
		DataConnection.ClearDataProperties();
		ClearRowGroupHeadersTable();
		DataConnection.DataSource = null;
		_selectedItems.UpdateIndexes();
		CoerceSelectedItem();
		bool flag = false;
		if (enumerable != null && !(enumerable is IDataGridCollectionView))
		{
			DataConnection.DataSource = DataGridDataConnection.CreateView(enumerable);
		}
		else
		{
			DataConnection.DataSource = enumerable;
			flag = true;
		}
		if (DataConnection.DataSource != null)
		{
			if (DataConnection.DataType != null)
			{
				foreach (DataGridColumn displayedColumn in ColumnsInternal.GetDisplayedColumns())
				{
					if (displayedColumn is DataGridBoundColumn dataGridBoundColumn)
					{
						dataGridBoundColumn.SetHeaderFromBinding();
					}
				}
			}
			DataConnection.WireEvents(DataConnection.DataSource);
		}
		_makeFirstDisplayedCellCurrentCellPending = true;
		ClearRows(recycle: false);
		RemoveAutoGeneratedColumns();
		PopulateRowGroupHeadersTable();
		SelectedItem = null;
		if (DataConnection.CollectionView != null && flag)
		{
			SelectedItem = DataConnection.CollectionView.CurrentItem;
		}
		_measured = false;
		InvalidateMeasure();
		UpdatePseudoClasses();
	}

	private void ColumnsInternal_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
		{
			UpdatePseudoClasses();
		}
	}

	internal void UpdatePseudoClasses()
	{
		base.PseudoClasses.Set(":empty-columns", !ColumnsInternal.GetVisibleColumns().Any());
		base.PseudoClasses.Set(":empty-rows", !DataConnection.Any());
	}

	private void OnSelectedIndexChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (!_areHandlersSuspended)
		{
			int num = (int)e.NewValue;
			object obj2 = (SelectedItem = ((num < 0) ? null : DataConnection.GetDataItem(num)));
			if (SelectedItem != obj2)
			{
				SetValueNoCallback(SelectedIndexProperty, (int)e.OldValue);
			}
		}
	}

	private void OnSelectedItemChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (_areHandlersSuspended)
		{
			return;
		}
		int num = ((e.NewValue == null) ? (-1) : DataConnection.IndexOf(e.NewValue));
		if (num == -1)
		{
			if (!CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true))
			{
				SetValueNoCallback(SelectedItemProperty, e.OldValue);
				return;
			}
			ClearRowSelection(resetAnchorSlot: true);
			if (DataConnection.CollectionView != null)
			{
				DataConnection.CollectionView.MoveCurrentTo(null);
			}
			return;
		}
		int num2 = SlotFromRowIndex(num);
		if (num2 != CurrentSlot)
		{
			if (!CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true))
			{
				SetValueNoCallback(SelectedItemProperty, e.OldValue);
				return;
			}
			if ((num2 >= SlotCount || num2 < -1) && DataConnection.CollectionView != null)
			{
				DataConnection.CollectionView.MoveCurrentToPosition(num);
			}
		}
		int selectedIndex = SelectedIndex;
		SetValueNoCallback(SelectedIndexProperty, num);
		try
		{
			_noSelectionChangeCount++;
			int num3 = CurrentColumnIndex;
			if (num3 == -1)
			{
				num3 = FirstDisplayedNonFillerColumnIndex;
			}
			if (IsSlotOutOfSelectionBounds(num2))
			{
				ClearRowSelection(num2, setAnchorSlot: true);
				return;
			}
			UpdateSelectionAndCurrency(num3, num2, DataGridSelectionAction.SelectCurrent, scrollIntoView: false);
		}
		finally
		{
			NoSelectionChangeCount--;
		}
		if (!_successfullyUpdatedSelection)
		{
			SetValueNoCallback(SelectedIndexProperty, selectedIndex);
			SetValueNoCallback(SelectedItemProperty, e.OldValue);
		}
	}

	private void OnVerticalGridLinesBrushChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (_rowsPresenter == null)
		{
			return;
		}
		foreach (DataGridRow allRow in GetAllRows())
		{
			allRow.EnsureGridLines();
		}
	}

	private void OnSelectionModeChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (!_areHandlersSuspended)
		{
			ClearRowSelection(resetAnchorSlot: true);
		}
	}

	private void OnRowHeaderWidthChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (!_areHandlersSuspended)
		{
			EnsureRowHeaderWidth();
		}
	}

	private void OnRowHeightChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (!_areHandlersSuspended)
		{
			InvalidateRowHeightEstimate();
			InvalidateRowsMeasure(invalidateIndividualElements: true);
			InvalidateMeasure();
		}
	}

	private void OnMinColumnWidthChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (_areHandlersSuspended)
		{
			return;
		}
		double val = (double)e.OldValue;
		foreach (DataGridColumn displayedColumn in ColumnsInternal.GetDisplayedColumns())
		{
			OnColumnMinWidthChanged(displayedColumn, Math.Max(displayedColumn.MinWidth, val));
		}
	}

	private void OnMaxColumnWidthChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (_areHandlersSuspended)
		{
			return;
		}
		double val = (double)e.OldValue;
		foreach (DataGridColumn displayedColumn in ColumnsInternal.GetDisplayedColumns())
		{
			OnColumnMaxWidthChanged(displayedColumn, Math.Min(displayedColumn.MaxWidth, val));
		}
	}

	private void OnIsReadOnlyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (!_areHandlersSuspended && (bool)e.NewValue && !CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true))
		{
			CancelEdit(DataGridEditingUnit.Row, raiseEvents: false);
		}
	}

	private void OnHorizontalGridLinesBrushChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (_areHandlersSuspended || _rowsPresenter == null)
		{
			return;
		}
		foreach (DataGridRow allRow in GetAllRows())
		{
			allRow.EnsureGridLines();
		}
	}

	private void OnHeadersVisibilityChanged(AvaloniaPropertyChangedEventArgs e)
	{
		DataGridHeadersVisibility value2 = (DataGridHeadersVisibility)e.OldValue;
		DataGridHeadersVisibility value3 = (DataGridHeadersVisibility)e.NewValue;
		bool flag = hasFlags(value3, DataGridHeadersVisibility.Column);
		bool flag2 = hasFlags(value3, DataGridHeadersVisibility.Row);
		bool flag3 = hasFlags(value2, DataGridHeadersVisibility.Column);
		bool flag4 = hasFlags(value2, DataGridHeadersVisibility.Row);
		if (flag != flag3 && _columnHeadersPresenter != null)
		{
			EnsureColumnHeadersVisibility();
			if (!flag)
			{
				_columnHeadersPresenter.Measure(default(Size));
			}
			else
			{
				EnsureVerticalGridLines();
			}
			InvalidateMeasure();
		}
		if (flag2 != flag4 && _rowsPresenter != null)
		{
			foreach (Control child in _rowsPresenter.Children)
			{
				if (child is DataGridRow dataGridRow)
				{
					dataGridRow.EnsureHeaderStyleAndVisibility(null);
					if (flag2)
					{
						dataGridRow.UpdatePseudoClasses();
						dataGridRow.EnsureHeaderVisibility();
					}
				}
				else if (child is DataGridRowGroupHeader dataGridRowGroupHeader)
				{
					dataGridRowGroupHeader.EnsureHeaderVisibility();
				}
			}
			InvalidateRowHeightEstimate();
			InvalidateRowsMeasure(invalidateIndividualElements: true);
		}
		if (_topLeftCornerHeader != null)
		{
			_topLeftCornerHeader.IsVisible = flag2 && flag;
			if (_topLeftCornerHeader.IsVisible)
			{
				_topLeftCornerHeader.Measure(default(Size));
			}
		}
		static bool hasFlags(DataGridHeadersVisibility value, DataGridHeadersVisibility flags)
		{
			return (value & flags) == flags;
		}
	}

	private void OnGridLinesVisibilityChanged(AvaloniaPropertyChangedEventArgs e)
	{
		foreach (DataGridRow allRow in GetAllRows())
		{
			allRow.EnsureGridLines();
			allRow.InvalidateHorizontalArrange();
		}
	}

	private void OnFrozenColumnCountChanged(AvaloniaPropertyChangedEventArgs e)
	{
		ProcessFrozenColumnCount();
	}

	private void ProcessFrozenColumnCount()
	{
		CorrectColumnFrozenStates();
		ComputeScrollBarsLayout();
		InvalidateColumnHeadersArrange();
		InvalidateCellsArrange();
	}

	private void OnColumnWidthChanged(AvaloniaPropertyChangedEventArgs e)
	{
		DataGridLength widthInternalNoCallback = (DataGridLength)e.NewValue;
		foreach (DataGridColumn displayedColumn in ColumnsInternal.GetDisplayedColumns())
		{
			if (displayedColumn.InheritsWidth)
			{
				displayedColumn.SetWidthInternalNoCallback(widthInternalNoCallback);
			}
		}
		EnsureHorizontalLayout();
	}

	private void OnCanUserResizeColumnsChanged(AvaloniaPropertyChangedEventArgs e)
	{
		EnsureHorizontalLayout();
	}

	public bool BeginEdit()
	{
		return BeginEdit(null);
	}

	public bool BeginEdit(RoutedEventArgs editingEventArgs)
	{
		if (CurrentColumnIndex == -1 || !GetRowSelection(CurrentSlot))
		{
			return false;
		}
		if (GetColumnEffectiveReadOnlyState(CurrentColumn))
		{
			return false;
		}
		return BeginCellEdit(editingEventArgs);
	}

	public bool CancelEdit()
	{
		return CancelEdit(DataGridEditingUnit.Row);
	}

	public bool CancelEdit(DataGridEditingUnit editingUnit)
	{
		return CancelEdit(editingUnit, raiseEvents: true);
	}

	public bool CommitEdit()
	{
		return CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true);
	}

	public bool CommitEdit(DataGridEditingUnit editingUnit, bool exitEditingMode)
	{
		if (!EndCellEdit(DataGridEditAction.Commit, editingUnit != 0 || exitEditingMode, ContainsFocus, raiseEvents: true))
		{
			return false;
		}
		if (editingUnit == DataGridEditingUnit.Row)
		{
			return EndRowEdit(DataGridEditAction.Commit, exitEditingMode, raiseEvents: true);
		}
		return true;
	}

	public void ScrollIntoView(object item, DataGridColumn column)
	{
		if ((column == null && (item == null || FirstDisplayedNonFillerColumnIndex == -1)) || (column != null && column.OwningGrid != this))
		{
			return;
		}
		if (item == null)
		{
			ScrollSlotIntoView(column.Index, DisplayData.FirstScrollingSlot, forCurrentCellChange: false, forceHorizontalScroll: true);
			return;
		}
		int num = -1;
		DataGridRowGroupInfo dataGridRowGroupInfo = null;
		if (item is DataGridCollectionViewGroup collectionViewGroup)
		{
			dataGridRowGroupInfo = RowGroupInfoFromCollectionViewGroup(collectionViewGroup);
			if (dataGridRowGroupInfo == null)
			{
				return;
			}
			num = dataGridRowGroupInfo.Slot;
		}
		else
		{
			int num2 = DataConnection.IndexOf(item);
			if (num2 == -1)
			{
				return;
			}
			num = SlotFromRowIndex(num2);
		}
		int columnIndex = column?.Index ?? FirstDisplayedNonFillerColumnIndex;
		if (_collapsedSlotsTable.Contains(num))
		{
			if (dataGridRowGroupInfo != null)
			{
				ExpandRowGroupParentChain(dataGridRowGroupInfo.Level - 1, dataGridRowGroupInfo.Slot);
			}
			else
			{
				dataGridRowGroupInfo = RowGroupHeadersTable.GetValueAt(RowGroupHeadersTable.GetPreviousIndex(num));
				if (dataGridRowGroupInfo != null)
				{
					ExpandRowGroupParentChain(dataGridRowGroupInfo.Level, dataGridRowGroupInfo.Slot);
				}
			}
			NegVerticalOffset = 0.0;
			SetVerticalOffset(0.0);
			ResetDisplayedRows();
			DisplayData.FirstScrollingSlot = 0;
			ComputeScrollBarsLayout();
		}
		ScrollSlotIntoView(columnIndex, num, forCurrentCellChange: true, forceHorizontalScroll: true);
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		if (DataConnection.DataSource != null && !DataConnection.EventsWired)
		{
			DataConnection.WireEvents(DataConnection.DataSource);
			InitializeElements(recycleRows: false);
		}
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		if (DataConnection.DataSource != null && DataConnection.EventsWired)
		{
			DataConnection.UnWireEvents(DataConnection.DataSource);
		}
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		if (_makeFirstDisplayedCellCurrentCellPending)
		{
			MakeFirstDisplayedCellCurrentCell();
		}
		if (base.Bounds.Width != finalSize.Width)
		{
			InvalidateColumnHeadersArrange();
			InvalidateCellsArrange();
		}
		return base.ArrangeOverride(finalSize);
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		if (!_measured)
		{
			_measured = true;
			RefreshRowsAndColumns(clearRows: false);
			UpdateRowDetailsHeightEstimate();
			if (FrozenColumnCountWithFiller > 0)
			{
				ProcessFrozenColumnCount();
			}
		}
		Size result;
		if (ColumnsInternal.VisibleEdgedColumnsWidth == 0.0)
		{
			if (_hScrollBar != null && _hScrollBar.IsVisible)
			{
				_hScrollBar.IsVisible = false;
			}
			if (_vScrollBar != null && _vScrollBar.IsVisible)
			{
				_vScrollBar.IsVisible = false;
			}
			result = base.MeasureOverride(availableSize);
		}
		else
		{
			if (_rowsPresenter != null)
			{
				_rowsPresenter.InvalidateMeasure();
			}
			InvalidateColumnHeadersMeasure();
			result = base.MeasureOverride(availableSize);
			ComputeScrollBarsLayout();
		}
		return result;
	}

	protected override void OnDataContextBeginUpdate()
	{
		base.OnDataContextBeginUpdate();
		NotifyDataContextPropertyForAllRowCells(GetAllRows(), arg2: true);
	}

	protected override void OnDataContextEndUpdate()
	{
		base.OnDataContextEndUpdate();
		NotifyDataContextPropertyForAllRowCells(GetAllRows(), arg2: false);
	}

	protected virtual void OnBeginningEdit(DataGridBeginningEditEventArgs e)
	{
		this.BeginningEdit?.Invoke(this, e);
	}

	protected virtual void OnCellEditEnded(DataGridCellEditEndedEventArgs e)
	{
		this.CellEditEnded?.Invoke(this, e);
	}

	protected virtual void OnCellEditEnding(DataGridCellEditEndingEventArgs e)
	{
		this.CellEditEnding?.Invoke(this, e);
	}

	internal virtual void OnCellPointerPressed(DataGridCellPointerPressedEventArgs e)
	{
		this.CellPointerPressed?.Invoke(this, e);
	}

	protected virtual void OnCurrentCellChanged(EventArgs e)
	{
		this.CurrentCellChanged?.Invoke(this, e);
	}

	protected virtual void OnLoadingRow(DataGridRowEventArgs e)
	{
		EventHandler<DataGridRowEventArgs> eventHandler = this.LoadingRow;
		if (eventHandler != null)
		{
			_loadedRows.Add(e.Row);
			LoadingOrUnloadingRow = true;
			eventHandler(this, e);
			LoadingOrUnloadingRow = false;
			_loadedRows.Remove(e.Row);
		}
	}

	protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
	{
		if (UpdateScroll(e.Delta * 50.0))
		{
			e.Handled = true;
		}
		else
		{
			e.Handled = e.Handled || !ScrollViewer.GetIsScrollChainingEnabled(this);
		}
	}

	internal bool UpdateScroll(Vector delta)
	{
		if (base.IsEnabled && DisplayData.NumDisplayedScrollingElements > 0)
		{
			bool flag = false;
			bool flag2 = false;
			double num = 0.0;
			if (delta.Y > 0.0)
			{
				num = Math.Max(0.0 - _verticalOffset, 0.0 - delta.Y);
			}
			else if (delta.Y < 0.0)
			{
				if (_vScrollBar != null && VerticalScrollBarVisibility == ScrollBarVisibility.Visible)
				{
					num = Math.Min(Math.Max(0.0, _vScrollBar.Maximum - _verticalOffset), 0.0 - delta.Y);
				}
				else
				{
					double num2 = EdgedRowsHeightCalculated - CellsHeight;
					num = Math.Min(Math.Max(0.0, num2 - _verticalOffset), 0.0 - delta.Y);
				}
			}
			if (num != 0.0)
			{
				DisplayData.PendingVerticalScrollHeight = num;
				flag = true;
			}
			if (delta.X != 0.0)
			{
				double num3 = HorizontalOffset - delta.X;
				double num4 = Math.Max(0.0, ColumnsInternal.VisibleEdgedColumnsWidth - CellsWidth);
				if (num3 < 0.0)
				{
					num3 = 0.0;
				}
				if (num3 > num4)
				{
					num3 = num4;
				}
				if (UpdateHorizontalOffset(num3))
				{
					flag2 = true;
					flag = true;
				}
			}
			if (flag)
			{
				if (!flag2)
				{
					InvalidateRowsMeasure(invalidateIndividualElements: false);
				}
				return true;
			}
		}
		return false;
	}

	protected virtual void OnPreparingCellForEdit(DataGridPreparingCellForEditEventArgs e)
	{
		this.PreparingCellForEdit?.Invoke(this, e);
	}

	protected virtual void OnRowEditEnded(DataGridRowEditEndedEventArgs e)
	{
		this.RowEditEnded?.Invoke(this, e);
	}

	protected virtual void OnRowEditEnding(DataGridRowEditEndingEventArgs e)
	{
		this.RowEditEnding?.Invoke(this, e);
	}

	protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
	{
		RaiseEvent(e);
	}

	protected virtual void OnUnloadingRow(DataGridRowEventArgs e)
	{
		EventHandler<DataGridRowEventArgs> eventHandler = this.UnloadingRow;
		if (eventHandler != null)
		{
			LoadingOrUnloadingRow = true;
			eventHandler(this, e);
			LoadingOrUnloadingRow = false;
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		_measured = false;
		if (_columnHeadersPresenter != null)
		{
			_columnHeadersPresenter.Children.Clear();
		}
		_columnHeadersPresenter = e.NameScope.Find<DataGridColumnHeadersPresenter>("PART_ColumnHeadersPresenter");
		if (_columnHeadersPresenter != null)
		{
			if (ColumnsInternal.FillerColumn != null)
			{
				ColumnsInternal.FillerColumn.IsRepresented = false;
			}
			_columnHeadersPresenter.OwningGrid = this;
			List<DataGridColumn> list = new List<DataGridColumn>(ColumnsItemsInternal);
			list.Sort(new DisplayIndexComparer());
			foreach (DataGridColumn item in list)
			{
				InsertDisplayedColumnHeader(item);
			}
		}
		if (_rowsPresenter != null)
		{
			UnloadElements(recycle: false);
		}
		_rowsPresenter = e.NameScope.Find<DataGridRowsPresenter>("PART_RowsPresenter");
		if (_rowsPresenter != null)
		{
			_rowsPresenter.OwningGrid = this;
			InvalidateRowHeightEstimate();
			UpdateRowDetailsHeightEstimate();
		}
		_frozenColumnScrollBarSpacer = e.NameScope.Find<Control>("PART_FrozenColumnScrollBarSpacer");
		if (_hScrollBar != null)
		{
			_hScrollBar.Scroll -= HorizontalScrollBar_Scroll;
		}
		_hScrollBar = e.NameScope.Find<ScrollBar>("PART_HorizontalScrollbar");
		if (_hScrollBar != null)
		{
			_hScrollBar.IsTabStop = false;
			_hScrollBar.Maximum = 0.0;
			_hScrollBar.Orientation = Orientation.Horizontal;
			_hScrollBar.IsVisible = false;
			_hScrollBar.Scroll += HorizontalScrollBar_Scroll;
		}
		if (_vScrollBar != null)
		{
			_vScrollBar.Scroll -= VerticalScrollBar_Scroll;
		}
		_vScrollBar = e.NameScope.Find<ScrollBar>("PART_VerticalScrollbar");
		if (_vScrollBar != null)
		{
			_vScrollBar.IsTabStop = false;
			_vScrollBar.Maximum = 0.0;
			_vScrollBar.Orientation = Orientation.Vertical;
			_vScrollBar.IsVisible = false;
			_vScrollBar.Scroll += VerticalScrollBar_Scroll;
		}
		_topLeftCornerHeader = e.NameScope.Find<ContentControl>("PART_TopLeftCornerHeader");
		EnsureTopLeftCornerHeader();
		_topRightCornerHeader = e.NameScope.Find<ContentControl>("PART_TopRightCornerHeader");
		_bottomRightCorner = e.NameScope.Find<Visual>("PART_BottomRightCorner");
	}

	internal bool CancelEdit(DataGridEditingUnit editingUnit, bool raiseEvents)
	{
		if (!EndCellEdit(DataGridEditAction.Cancel, exitEditingMode: true, ContainsFocus, raiseEvents))
		{
			return false;
		}
		if (editingUnit == DataGridEditingUnit.Row)
		{
			return EndRowEdit(DataGridEditAction.Cancel, exitEditingMode: true, raiseEvents);
		}
		return true;
	}

	internal void CoerceSelectedItem()
	{
		object obj = null;
		if (SelectionMode == DataGridSelectionMode.Extended && CurrentSlot != -1 && _selectedItems.ContainsSlot(CurrentSlot))
		{
			obj = CurrentItem;
		}
		else if (_selectedItems.Count > 0)
		{
			obj = _selectedItems[0];
		}
		SetValueNoCallback(SelectedItemProperty, obj);
		int value = -1;
		if (obj != null)
		{
			value = DataConnection.IndexOf(obj);
		}
		SetValueNoCallback(SelectedIndexProperty, value);
	}

	internal static DataGridCell GetOwningCell(Control element)
	{
		DataGridCell dataGridCell = element as DataGridCell;
		while (element != null && dataGridCell == null)
		{
			element = element.Parent as Control;
			dataGridCell = element as DataGridCell;
		}
		return dataGridCell;
	}

	internal IEnumerable<object> GetSelectionInclusive(int startRowIndex, int endRowIndex)
	{
		int endSlot = SlotFromRowIndex(endRowIndex);
		foreach (int slot in _selectedItems.GetSlots(SlotFromRowIndex(startRowIndex)))
		{
			if (slot > endSlot)
			{
				break;
			}
			yield return DataConnection.GetDataItem(RowIndexFromSlot(slot));
		}
	}

	internal void InitializeElements(bool recycleRows)
	{
		try
		{
			_noCurrentCellChangeCount++;
			CancelEdit(DataGridEditingUnit.Row, raiseEvents: false);
			List<object> selectedItemsCache = new List<object>(_selectedItems.SelectedItemsCache);
			if (recycleRows)
			{
				RefreshRows(recycleRows, clearRows: true);
			}
			else
			{
				RefreshRowsAndColumns(clearRows: true);
			}
			_selectedItems.SelectedItemsCache = selectedItemsCache;
			CoerceSelectedItem();
			if (RowDetailsVisibilityMode != DataGridRowDetailsVisibilityMode.Collapsed)
			{
				UpdateRowDetailsVisibilityMode(RowDetailsVisibilityMode);
			}
			ApplyDisplayedRowsState(DisplayData.FirstScrollingSlot, DisplayData.LastScrollingSlot);
		}
		finally
		{
			NoCurrentCellChangeCount--;
		}
	}

	internal bool IsDoubleClickRecordsClickOnCall(Control element)
	{
		if (_clickedElement == element)
		{
			_clickedElement = null;
			return true;
		}
		_clickedElement = element;
		return false;
	}

	internal object ItemFromSlot(int slot, ref int rowIndex)
	{
		if (RowGroupHeadersTable.Contains(slot))
		{
			return RowGroupHeadersTable.GetValueAt(slot)?.CollectionViewGroup;
		}
		rowIndex = RowIndexFromSlot(slot);
		return DataConnection.GetDataItem(rowIndex);
	}

	internal bool ProcessDownKey(KeyEventArgs e)
	{
		KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrlOrCmd, out var shift);
		return ProcessDownKeyInternal(shift, ctrlOrCmd);
	}

	internal bool ProcessEndKey(KeyEventArgs e)
	{
		KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrlOrCmd, out var shift);
		return ProcessEndKey(shift, ctrlOrCmd);
	}

	internal bool ProcessEnterKey(KeyEventArgs e)
	{
		KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrlOrCmd, out var shift);
		return ProcessEnterKey(shift, ctrlOrCmd);
	}

	internal bool ProcessHomeKey(KeyEventArgs e)
	{
		KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrlOrCmd, out var shift);
		return ProcessHomeKey(shift, ctrlOrCmd);
	}

	internal void ProcessHorizontalScroll(ScrollEventType scrollEventType)
	{
		if (_horizontalScrollChangesIgnored > 0)
		{
			return;
		}
		double num = 0.0;
		switch (scrollEventType)
		{
		case ScrollEventType.SmallIncrement:
			num = GetHorizontalSmallScrollIncrease();
			break;
		case ScrollEventType.SmallDecrement:
			num = 0.0 - GetHorizontalSmallScrollDecrease();
			break;
		}
		_horizontalScrollChangesIgnored++;
		try
		{
			if (num != 0.0)
			{
				_hScrollBar.Value = _horizontalOffset + num;
			}
			UpdateHorizontalOffset(_hScrollBar.Value);
		}
		finally
		{
			_horizontalScrollChangesIgnored--;
		}
	}

	internal bool ProcessLeftKey(KeyEventArgs e)
	{
		KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrlOrCmd, out var shift);
		return ProcessLeftKey(shift, ctrlOrCmd);
	}

	internal bool ProcessNextKey(KeyEventArgs e)
	{
		KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrlOrCmd, out var shift);
		return ProcessNextKey(shift, ctrlOrCmd);
	}

	internal bool ProcessPriorKey(KeyEventArgs e)
	{
		KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrlOrCmd, out var shift);
		return ProcessPriorKey(shift, ctrlOrCmd);
	}

	internal bool ProcessRightKey(KeyEventArgs e)
	{
		KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrlOrCmd, out var shift);
		return ProcessRightKey(shift, ctrlOrCmd);
	}

	internal void ProcessSelectionAndCurrency(int columnIndex, object item, int backupSlot, DataGridSelectionAction action, bool scrollIntoView)
	{
		_noSelectionChangeCount++;
		_noCurrentCellChangeCount++;
		try
		{
			int num = -1;
			if (item is DataGridCollectionViewGroup collectionViewGroup)
			{
				DataGridRowGroupInfo dataGridRowGroupInfo = RowGroupInfoFromCollectionViewGroup(collectionViewGroup);
				if (dataGridRowGroupInfo != null)
				{
					num = dataGridRowGroupInfo.Slot;
				}
			}
			else
			{
				num = SlotFromRowIndex(DataConnection.IndexOf(item));
			}
			if (num == -1)
			{
				num = backupSlot;
			}
			if (num < 0 || num > SlotCount)
			{
				return;
			}
			switch (action)
			{
			case DataGridSelectionAction.AddCurrentToSelection:
				SetRowSelection(num, isSelected: true, setAnchorSlot: true);
				break;
			case DataGridSelectionAction.RemoveCurrentFromSelection:
				SetRowSelection(num, isSelected: false, setAnchorSlot: false);
				break;
			case DataGridSelectionAction.SelectFromAnchorToCurrent:
				if (SelectionMode == DataGridSelectionMode.Extended && AnchorSlot != -1)
				{
					int anchorSlot = AnchorSlot;
					ClearRowSelection(num, setAnchorSlot: false);
					if (num <= anchorSlot)
					{
						SetRowsSelection(num, anchorSlot);
					}
					else
					{
						SetRowsSelection(anchorSlot, num);
					}
					break;
				}
				goto case DataGridSelectionAction.SelectCurrent;
			case DataGridSelectionAction.SelectCurrent:
				ClearRowSelection(num, setAnchorSlot: true);
				break;
			}
			if (CurrentSlot != num || (CurrentColumnIndex != columnIndex && columnIndex != -1))
			{
				if (columnIndex == -1)
				{
					if (CurrentColumnIndex != -1)
					{
						columnIndex = CurrentColumnIndex;
					}
					else
					{
						DataGridColumn firstVisibleNonFillerColumn = ColumnsInternal.FirstVisibleNonFillerColumn;
						if (firstVisibleNonFillerColumn != null)
						{
							columnIndex = firstVisibleNonFillerColumn.Index;
						}
					}
				}
				if (columnIndex != -1 && (!SetCurrentCellCore(columnIndex, num, commitEdit: true, SlotFromRowIndex(SelectedIndex) != num) || (scrollIntoView && !ScrollSlotIntoView(columnIndex, num, forCurrentCellChange: true, forceHorizontalScroll: false))))
				{
					return;
				}
			}
			_successfullyUpdatedSelection = true;
		}
		finally
		{
			NoCurrentCellChangeCount--;
			NoSelectionChangeCount--;
		}
	}

	internal bool ProcessUpKey(KeyEventArgs e)
	{
		KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrlOrCmd, out var shift);
		return ProcessUpKey(shift, ctrlOrCmd);
	}

	internal void ProcessVerticalScroll(ScrollEventType scrollEventType)
	{
		if (_verticalScrollChangesIgnored > 0)
		{
			return;
		}
		_verticalScrollChangesIgnored++;
		try
		{
			switch (scrollEventType)
			{
			case ScrollEventType.SmallIncrement:
			{
				DisplayData.PendingVerticalScrollHeight = GetVerticalSmallScrollIncrease();
				double num = _verticalOffset + DisplayData.PendingVerticalScrollHeight;
				if (num > _vScrollBar.Maximum)
				{
					DisplayData.PendingVerticalScrollHeight -= num - _vScrollBar.Maximum;
				}
				break;
			}
			case ScrollEventType.SmallDecrement:
			{
				if (MathUtilities.GreaterThan(NegVerticalOffset, 0.0))
				{
					DisplayData.PendingVerticalScrollHeight -= NegVerticalOffset;
					break;
				}
				int previousVisibleSlot = GetPreviousVisibleSlot(DisplayData.FirstScrollingSlot);
				if (previousVisibleSlot >= 0)
				{
					ScrollSlotIntoView(previousVisibleSlot, scrolledHorizontally: false);
				}
				return;
			}
			default:
				DisplayData.PendingVerticalScrollHeight = _vScrollBar.Value - _verticalOffset;
				break;
			}
			if (!MathUtilities.IsZero(DisplayData.PendingVerticalScrollHeight))
			{
				InvalidateRowsMeasure(invalidateIndividualElements: false);
			}
		}
		finally
		{
			_verticalScrollChangesIgnored--;
		}
	}

	internal void RefreshRowsAndColumns(bool clearRows)
	{
		if (_measured)
		{
			try
			{
				_noCurrentCellChangeCount++;
				if (clearRows)
				{
					ClearRows(recycle: false);
					ClearRowGroupHeadersTable();
					PopulateRowGroupHeadersTable();
				}
				if (AutoGenerateColumns)
				{
					AutoGenerateColumnsPrivate();
				}
				foreach (DataGridColumn item in ColumnsItemsInternal)
				{
					if (!item.IsAutoGenerated && item.HasHeaderCell)
					{
						item.HeaderCell.UpdatePseudoClasses();
					}
				}
				RefreshRows(recycleRows: false, clearRows: false);
				if (Columns.Count > 0 && CurrentColumnIndex == -1)
				{
					MakeFirstDisplayedCellCurrentCell();
				}
				else
				{
					_makeFirstDisplayedCellCurrentCellPending = false;
					_desiredCurrentColumnIndex = -1;
					FlushCurrentCellChanged();
				}
				return;
			}
			finally
			{
				NoCurrentCellChangeCount--;
			}
		}
		if (clearRows)
		{
			ClearRows(recycle: false);
		}
		ClearRowGroupHeadersTable();
		PopulateRowGroupHeadersTable();
	}

	internal bool ScrollSlotIntoView(int columnIndex, int slot, bool forCurrentCellChange, bool forceHorizontalScroll)
	{
		if (CurrentColumnIndex >= 0 && (CurrentColumnIndex != columnIndex || CurrentSlot != slot) && (!CommitEditForOperation(columnIndex, slot, forCurrentCellChange) || IsInnerCellOutOfBounds(columnIndex, slot)))
		{
			return false;
		}
		double horizontalOffset = HorizontalOffset;
		if ((forceHorizontalScroll || slot != -1) && !ScrollColumnIntoView(columnIndex))
		{
			return false;
		}
		if (!ScrollSlotIntoView(slot, horizontalOffset != HorizontalOffset))
		{
			return false;
		}
		return true;
	}

	internal bool SetCurrentCellCore(int columnIndex, int slot)
	{
		return SetCurrentCellCore(columnIndex, slot, commitEdit: true, endRowEdit: true);
	}

	internal bool UpdateHorizontalOffset(double newValue)
	{
		if (HorizontalOffset != newValue)
		{
			HorizontalOffset = newValue;
			InvalidateColumnHeadersMeasure();
			InvalidateRowsMeasure(invalidateIndividualElements: true);
			return true;
		}
		return false;
	}

	internal bool UpdateSelectionAndCurrency(int columnIndex, int slot, DataGridSelectionAction action, bool scrollIntoView)
	{
		_successfullyUpdatedSelection = false;
		_noSelectionChangeCount++;
		_noCurrentCellChangeCount++;
		try
		{
			if (ColumnsInternal.RowGroupSpacerColumn.IsRepresented && columnIndex == ColumnsInternal.RowGroupSpacerColumn.Index)
			{
				columnIndex = -1;
			}
			if (IsSlotOutOfSelectionBounds(slot) || (columnIndex != -1 && IsColumnOutOfBounds(columnIndex)))
			{
				return false;
			}
			int rowIndex = -1;
			object item = ItemFromSlot(slot, ref rowIndex);
			if (EditingRow != null && slot != EditingRow.Slot && !CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true))
			{
				return false;
			}
			if (DataConnection.CollectionView != null && DataConnection.CollectionView.CurrentPosition != rowIndex)
			{
				DataConnection.MoveCurrentTo(item, slot, columnIndex, action, scrollIntoView);
			}
			else
			{
				ProcessSelectionAndCurrency(columnIndex, item, slot, action, scrollIntoView);
			}
		}
		finally
		{
			NoCurrentCellChangeCount--;
			NoSelectionChangeCount--;
		}
		return _successfullyUpdatedSelection;
	}

	internal void UpdateStateOnCurrentChanged(object currentItem, int currentPosition)
	{
		if (currentItem == CurrentItem && currentItem == SelectedItem && currentPosition == SelectedIndex)
		{
			return;
		}
		int num = CurrentColumnIndex;
		if (num == -1)
		{
			num = ((!IsColumnOutOfBounds(_desiredCurrentColumnIndex) && (!ColumnsInternal.RowGroupSpacerColumn.IsRepresented || _desiredCurrentColumnIndex != ColumnsInternal.RowGroupSpacerColumn.Index)) ? _desiredCurrentColumnIndex : FirstDisplayedNonFillerColumnIndex);
		}
		_desiredCurrentColumnIndex = -1;
		try
		{
			_noSelectionChangeCount++;
			_noCurrentCellChangeCount++;
			if (!CommitEdit())
			{
				CancelEdit(DataGridEditingUnit.Row, raiseEvents: false);
			}
			ClearRowSelection(resetAnchorSlot: true);
			if (currentItem == null)
			{
				SetCurrentCellCore(-1, -1);
				return;
			}
			int backupSlot = SlotFromRowIndex(currentPosition);
			ProcessSelectionAndCurrency(num, currentItem, backupSlot, DataGridSelectionAction.SelectCurrent, scrollIntoView: false);
		}
		finally
		{
			NoCurrentCellChangeCount--;
			NoSelectionChangeCount--;
		}
	}

	internal bool UpdateStateOnMouseRightButtonDown(PointerPressedEventArgs pointerPressedEventArgs, int columnIndex, int slot, bool allowEdit)
	{
		KeyboardHelper.GetMetaKeyState(this, pointerPressedEventArgs.KeyModifiers, out var ctrlOrCmd, out var shift);
		return UpdateStateOnMouseRightButtonDown(pointerPressedEventArgs, columnIndex, slot, allowEdit, shift, ctrlOrCmd);
	}

	internal bool UpdateStateOnMouseLeftButtonDown(PointerPressedEventArgs pointerPressedEventArgs, int columnIndex, int slot, bool allowEdit)
	{
		KeyboardHelper.GetMetaKeyState(this, pointerPressedEventArgs.KeyModifiers, out var ctrlOrCmd, out var shift);
		return UpdateStateOnMouseLeftButtonDown(pointerPressedEventArgs, columnIndex, slot, allowEdit, shift, ctrlOrCmd);
	}

	internal void UpdateVerticalScrollBar()
	{
		if (_vScrollBar != null && _vScrollBar.IsVisible)
		{
			double cellsHeight = CellsHeight;
			double edgedRowsHeightCalculated = EdgedRowsHeightCalculated;
			UpdateVerticalScrollBar(edgedRowsHeightCalculated > cellsHeight, VerticalScrollBarVisibility == ScrollBarVisibility.Visible, edgedRowsHeightCalculated, cellsHeight);
		}
	}

	internal bool WaitForLostFocus(Action action)
	{
		if (EditingRow != null && EditingColumnIndex != -1 && !_executingLostFocusActions)
		{
			Control cellContent = ColumnsItemsInternal[EditingColumnIndex].GetCellContent(EditingRow);
			if (cellContent != null && cellContent.ContainsChild(_focusedObject))
			{
				_lostFocusActions.Enqueue(action);
				cellContent.LostFocus += EditingElement_LostFocus;
				Focus();
				return true;
			}
		}
		return false;
	}

	protected virtual void OnLoadingRowDetails(DataGridRowDetailsEventArgs e)
	{
		EventHandler<DataGridRowDetailsEventArgs> eventHandler = this.LoadingRowDetails;
		if (eventHandler != null)
		{
			LoadingOrUnloadingRow = true;
			eventHandler(this, e);
			LoadingOrUnloadingRow = false;
		}
	}

	protected virtual void OnUnloadingRowDetails(DataGridRowDetailsEventArgs e)
	{
		EventHandler<DataGridRowDetailsEventArgs> eventHandler = this.UnloadingRowDetails;
		if (eventHandler != null)
		{
			LoadingOrUnloadingRow = true;
			eventHandler(this, e);
			LoadingOrUnloadingRow = false;
		}
	}

	internal void OnRowDetailsChanged()
	{
		if (!_scrollingByHeight)
		{
			InvalidateMeasure();
		}
	}

	private static void NotifyDataContextPropertyForAllRowCells(IEnumerable<DataGridRow> rowSource, bool arg2)
	{
		foreach (DataGridRow item in rowSource)
		{
			foreach (DataGridCell cell in item.Cells)
			{
				if (cell.Content is StyledElement arg3)
				{
					StyledElement.DataContextProperty.Notifying?.Invoke(arg3, arg2);
				}
			}
		}
	}

	private void UpdateRowDetailsVisibilityMode(DataGridRowDetailsVisibilityMode newDetailsMode)
	{
		int count = DataConnection.Count;
		if (_rowsPresenter == null || count <= 0)
		{
			return;
		}
		bool flag = false;
		switch (newDetailsMode)
		{
		case DataGridRowDetailsVisibilityMode.Visible:
			flag = true;
			_showDetailsTable.AddValues(0, count, value: true);
			break;
		case DataGridRowDetailsVisibilityMode.Collapsed:
			flag = false;
			_showDetailsTable.AddValues(0, count, value: false);
			break;
		case DataGridRowDetailsVisibilityMode.VisibleWhenSelected:
			_showDetailsTable.Clear();
			break;
		}
		bool flag2 = false;
		foreach (DataGridRow allRow in GetAllRows())
		{
			if (allRow.IsVisible)
			{
				if (newDetailsMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
				{
					flag = _selectedItems.ContainsSlot(allRow.Slot);
				}
				if (allRow.AreDetailsVisible != flag)
				{
					flag2 = true;
					allRow.SetDetailsVisibilityInternal(flag, raiseNotification: true, animate: false);
				}
			}
		}
		if (flag2)
		{
			UpdateDisplayedRows(DisplayData.FirstScrollingSlot, CellsHeight);
			InvalidateRowsMeasure(invalidateIndividualElements: false);
		}
	}

	private void AddNewCellPrivate(DataGridRow row, DataGridColumn column)
	{
		DataGridCell dataGridCell = new DataGridCell();
		PopulateCellContent(isCellEdited: false, column, row, dataGridCell);
		if (row.OwningGrid != null)
		{
			dataGridCell.OwningColumn = column;
			dataGridCell.IsVisible = column.IsVisible;
			ControlTheme cellTheme = row.OwningGrid.CellTheme;
			if (cellTheme != null)
			{
				dataGridCell.SetValue(StyledElement.ThemeProperty, cellTheme, BindingPriority.Template);
			}
		}
		row.Cells.Insert(column.Index, dataGridCell);
	}

	private bool BeginCellEdit(RoutedEventArgs editingEventArgs)
	{
		if (CurrentColumnIndex == -1 || !GetRowSelection(CurrentSlot))
		{
			return false;
		}
		if (_editingColumnIndex != -1)
		{
			return true;
		}
		DataGridRow dataGridRow = EditingRow;
		if (dataGridRow == null)
		{
			dataGridRow = ((!IsSlotVisible(CurrentSlot)) ? GenerateRow(RowIndexFromSlot(CurrentSlot), CurrentSlot) : (DisplayData.GetDisplayedElement(CurrentSlot) as DataGridRow));
		}
		int currentSlot = CurrentSlot;
		int currentColumnIndex = CurrentColumnIndex;
		DataGridCell dataGridCell = dataGridRow.Cells[CurrentColumnIndex];
		DataGridBeginningEditEventArgs dataGridBeginningEditEventArgs = new DataGridBeginningEditEventArgs(CurrentColumn, dataGridRow, editingEventArgs);
		OnBeginningEdit(dataGridBeginningEditEventArgs);
		if (dataGridBeginningEditEventArgs.Cancel || currentSlot != CurrentSlot || currentColumnIndex != CurrentColumnIndex || !GetRowSelection(CurrentSlot) || (EditingRow == null && !BeginRowEdit(dataGridRow)))
		{
			return false;
		}
		_editingColumnIndex = CurrentColumnIndex;
		_editingEventArgs = editingEventArgs;
		EditingRow.Cells[CurrentColumnIndex].UpdatePseudoClasses();
		PopulateCellContent(isCellEdited: true, CurrentColumn, dataGridRow, dataGridCell);
		return true;
	}

	private bool BeginRowEdit(DataGridRow dataGridRow)
	{
		if (DataConnection.BeginEdit(dataGridRow.DataContext))
		{
			EditingRow = dataGridRow;
			GenerateEditingElements();
			return true;
		}
		return false;
	}

	private bool CancelRowEdit(bool exitEditingMode)
	{
		if (EditingRow == null)
		{
			return true;
		}
		object dataContext = EditingRow.DataContext;
		if (!DataConnection.CancelEdit(dataContext))
		{
			return false;
		}
		foreach (DataGridColumn column in Columns)
		{
			if (exitEditingMode || column.Index != _editingColumnIndex || !(column is DataGridBoundColumn))
			{
				PopulateCellContent(!exitEditingMode && column.Index == _editingColumnIndex, column, EditingRow, EditingRow.Cells[column.Index]);
			}
		}
		return true;
	}

	private bool CommitEditForOperation(int columnIndex, int slot, bool forCurrentCellChange)
	{
		if (forCurrentCellChange)
		{
			if (!EndCellEdit(DataGridEditAction.Commit, exitEditingMode: true, keepFocus: true, raiseEvents: true))
			{
				return false;
			}
			if (CurrentSlot != slot && !EndRowEdit(DataGridEditAction.Commit, exitEditingMode: true, raiseEvents: true))
			{
				return false;
			}
		}
		if (IsColumnOutOfBounds(columnIndex))
		{
			return false;
		}
		if (slot >= SlotCount)
		{
			int lastVisibleSlot = LastVisibleSlot;
			if (forCurrentCellChange && CurrentColumnIndex == -1 && lastVisibleSlot != -1)
			{
				SetAndSelectCurrentCell(columnIndex, lastVisibleSlot, forceCurrentCellSelection: false);
			}
			return false;
		}
		return true;
	}

	private bool CommitRowEdit(bool exitEditingMode)
	{
		if (EditingRow == null)
		{
			return true;
		}
		if (!EditingRow.IsValid)
		{
			return false;
		}
		DataConnection.EndEdit(EditingRow.DataContext);
		if (!exitEditingMode)
		{
			DataConnection.BeginEdit(EditingRow.DataContext);
		}
		return true;
	}

	private void CompleteCellsCollection(DataGridRow dataGridRow)
	{
		int count = dataGridRow.Cells.Count;
		if (ColumnsItemsInternal.Count > count)
		{
			for (int i = count; i < ColumnsItemsInternal.Count; i++)
			{
				AddNewCellPrivate(dataGridRow, ColumnsItemsInternal[i]);
			}
		}
	}

	private void ComputeScrollBarsLayout()
	{
		if (_ignoreNextScrollBarsLayout)
		{
			_ignoreNextScrollBarsLayout = false;
		}
		bool isHorizontalScrollBarOverCells = IsHorizontalScrollBarOverCells;
		bool isVerticalScrollBarOverCells = IsVerticalScrollBarOverCells;
		double num = CellsWidth;
		double num2 = CellsHeight;
		bool flag = false;
		bool flag2 = false;
		double num3 = 0.0;
		if (_hScrollBar != null)
		{
			flag2 = HorizontalScrollBarVisibility == ScrollBarVisibility.Visible;
			flag = flag2 || (ColumnsInternal.VisibleColumnCount > 0 && HorizontalScrollBarVisibility != 0 && HorizontalScrollBarVisibility != ScrollBarVisibility.Hidden);
			if (!flag2 && _hScrollBar.IsVisible && !isHorizontalScrollBarOverCells)
			{
				num2 += _hScrollBar.DesiredSize.Height;
			}
			if (!isHorizontalScrollBarOverCells)
			{
				num3 = _hScrollBar.Height + _hScrollBar.Margin.Top + _hScrollBar.Margin.Bottom;
			}
		}
		bool flag3 = false;
		bool flag4 = false;
		double num4 = 0.0;
		if (_vScrollBar != null)
		{
			flag4 = VerticalScrollBarVisibility == ScrollBarVisibility.Visible;
			flag3 = flag4 || (ColumnsItemsInternal.Count > 0 && VerticalScrollBarVisibility != 0 && VerticalScrollBarVisibility != ScrollBarVisibility.Hidden);
			if (!flag4 && _vScrollBar.IsVisible && !isVerticalScrollBarOverCells)
			{
				num += _vScrollBar.DesiredSize.Width;
			}
			if (!isVerticalScrollBarOverCells)
			{
				num4 = _vScrollBar.Width + _vScrollBar.Margin.Left + _vScrollBar.Margin.Right;
			}
		}
		bool flag5 = false;
		bool flag6 = false;
		double visibleEdgedColumnsWidth = ColumnsInternal.VisibleEdgedColumnsWidth;
		double visibleFrozenEdgedColumnsWidth = ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth();
		UpdateDisplayedRows(DisplayData.FirstScrollingSlot, CellsHeight);
		double edgedRowsHeightCalculated = EdgedRowsHeightCalculated;
		if (!flag2 && !flag4)
		{
			bool flag7 = false;
			if (flag && MathUtilities.GreaterThan(visibleEdgedColumnsWidth, num) && MathUtilities.LessThan(visibleFrozenEdgedColumnsWidth, num) && MathUtilities.LessThanOrClose(num3, num2))
			{
				double num5 = num2;
				num2 -= num3;
				flag7 = (flag5 = true);
				if (num4 > 0.0 && flag3 && (MathUtilities.LessThanOrClose(visibleEdgedColumnsWidth - num, num4) || MathUtilities.LessThanOrClose(num - visibleFrozenEdgedColumnsWidth, num4)))
				{
					UpdateDisplayedRows(DisplayData.FirstScrollingSlot, num2);
					if (DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount)
					{
						flag5 = MathUtilities.LessThan(visibleFrozenEdgedColumnsWidth, num - num4);
					}
				}
				if (!flag5)
				{
					num2 = num5;
				}
			}
			int firstScrollingSlot = DisplayData.FirstScrollingSlot;
			UpdateDisplayedRows(firstScrollingSlot, num2);
			if (flag3 && MathUtilities.GreaterThan(num2, 0.0) && MathUtilities.LessThanOrClose(num4, num) && DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount)
			{
				num -= num4;
				flag6 = true;
			}
			DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
			ComputeDisplayedColumns();
			if ((num4 > 0.0 || num3 > 0.0) && flag && flag6 && !flag5 && MathUtilities.GreaterThan(visibleEdgedColumnsWidth, num) && MathUtilities.LessThan(visibleFrozenEdgedColumnsWidth, num) && MathUtilities.LessThanOrClose(num3, num2))
			{
				num += num4;
				num2 -= num3;
				flag6 = false;
				UpdateDisplayedRows(firstScrollingSlot, num2);
				if (num2 > 0.0 && num4 <= num && DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount)
				{
					num -= num4;
					flag6 = true;
				}
				flag5 = flag6 || flag7;
			}
		}
		else if (flag2 && !flag4)
		{
			if (flag3)
			{
				if (num2 > 0.0 && MathUtilities.LessThanOrClose(num4, num) && DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount)
				{
					num -= num4;
					flag6 = true;
				}
				DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
				ComputeDisplayedColumns();
			}
			flag5 = visibleEdgedColumnsWidth > num && visibleFrozenEdgedColumnsWidth < num;
		}
		else if (!flag2 && flag4)
		{
			if (flag)
			{
				if (num > 0.0 && MathUtilities.LessThanOrClose(num3, num2) && MathUtilities.GreaterThan(visibleEdgedColumnsWidth, num) && MathUtilities.LessThan(visibleFrozenEdgedColumnsWidth, num))
				{
					num2 -= num3;
					flag5 = true;
					UpdateDisplayedRows(DisplayData.FirstScrollingSlot, num2);
				}
				DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
				ComputeDisplayedColumns();
			}
			flag6 = DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount;
		}
		else
		{
			DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
			ComputeDisplayedColumns();
			flag6 = DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount;
			flag5 = visibleEdgedColumnsWidth > num && visibleFrozenEdgedColumnsWidth < num;
		}
		UpdateHorizontalScrollBar(flag5, flag2, visibleEdgedColumnsWidth, visibleFrozenEdgedColumnsWidth, num);
		UpdateVerticalScrollBar(flag6, flag4, edgedRowsHeightCalculated, num2);
		if (_topRightCornerHeader != null)
		{
			if (AreColumnHeadersVisible && _vScrollBar != null && _vScrollBar.IsVisible)
			{
				_topRightCornerHeader.IsVisible = true;
			}
			else
			{
				_topRightCornerHeader.IsVisible = false;
			}
		}
		if (_bottomRightCorner != null)
		{
			_bottomRightCorner.IsVisible = _hScrollBar != null && _hScrollBar.IsVisible && _vScrollBar != null && _vScrollBar.IsVisible;
		}
		DisplayData.FullyRecycleElements();
	}

	private void EditingElement_LostFocus(object sender, RoutedEventArgs e)
	{
		if (!(sender is Control control))
		{
			return;
		}
		control.LostFocus -= EditingElement_LostFocus;
		if (EditingRow != null && _editingColumnIndex != -1)
		{
			FocusEditingCell(setFocus: true);
		}
		try
		{
			_executingLostFocusActions = true;
			while (_lostFocusActions.Count > 0)
			{
				_lostFocusActions.Dequeue()();
			}
		}
		finally
		{
			_executingLostFocusActions = false;
		}
	}

	private void EnsureHorizontalLayout()
	{
		ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
		InvalidateColumnHeadersMeasure();
		InvalidateRowsMeasure(invalidateIndividualElements: true);
		InvalidateMeasure();
	}

	private void EnsureRowHeaderWidth()
	{
		if (!AreRowHeadersVisible)
		{
			return;
		}
		if (AreColumnHeadersVisible)
		{
			EnsureTopLeftCornerHeader();
		}
		if (_rowsPresenter == null)
		{
			return;
		}
		bool flag = false;
		foreach (Control child in _rowsPresenter.Children)
		{
			if (child is DataGridRow dataGridRow)
			{
				if (dataGridRow.HeaderCell != null && dataGridRow.HeaderCell.DesiredSize.Width != ActualRowHeaderWidth)
				{
					dataGridRow.HeaderCell.InvalidateMeasure();
					flag = true;
				}
			}
			else if (child is DataGridRowGroupHeader { HeaderCell: not null } dataGridRowGroupHeader && dataGridRowGroupHeader.HeaderCell.DesiredSize.Width != ActualRowHeaderWidth)
			{
				dataGridRowGroupHeader.HeaderCell.InvalidateMeasure();
				flag = true;
			}
		}
		if (flag)
		{
			InvalidateMeasure();
		}
	}

	private void EnsureRowsPresenterVisibility()
	{
		if (_rowsPresenter != null)
		{
			_rowsPresenter.IsVisible = ColumnsInternal.FirstVisibleNonFillerColumn != null;
		}
	}

	private void EnsureTopLeftCornerHeader()
	{
		if (_topLeftCornerHeader == null)
		{
			return;
		}
		_topLeftCornerHeader.IsVisible = HeadersVisibility == DataGridHeadersVisibility.All;
		if (_topLeftCornerHeader.IsVisible)
		{
			if (!double.IsNaN(RowHeaderWidth))
			{
				_topLeftCornerHeader.Width = RowHeaderWidth;
			}
			else if (VisibleSlotCount > 0)
			{
				_topLeftCornerHeader.Width = RowHeadersDesiredWidth;
			}
		}
	}

	private void InvalidateCellsArrange()
	{
		foreach (DataGridRow allRow in GetAllRows())
		{
			allRow.InvalidateHorizontalArrange();
		}
	}

	private void InvalidateColumnHeadersArrange()
	{
		if (_columnHeadersPresenter != null)
		{
			_columnHeadersPresenter.InvalidateArrange();
		}
	}

	private void InvalidateColumnHeadersMeasure()
	{
		if (_columnHeadersPresenter != null)
		{
			EnsureColumnHeadersVisibility();
			_columnHeadersPresenter.InvalidateMeasure();
		}
	}

	private void InvalidateRowsArrange()
	{
		if (_rowsPresenter != null)
		{
			_rowsPresenter.InvalidateArrange();
		}
	}

	private void InvalidateRowsMeasure(bool invalidateIndividualElements)
	{
		if (_rowsPresenter == null)
		{
			return;
		}
		_rowsPresenter.InvalidateMeasure();
		if (!invalidateIndividualElements)
		{
			return;
		}
		foreach (Control child in _rowsPresenter.Children)
		{
			child.InvalidateMeasure();
		}
	}

	private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
	{
		if (!ContainsFocus)
		{
			ContainsFocus = true;
			ApplyDisplayedRowsState(DisplayData.FirstScrollingSlot, DisplayData.LastScrollingSlot);
			if (CurrentColumnIndex != -1 && IsSlotVisible(CurrentSlot) && DisplayData.GetDisplayedElement(CurrentSlot) is DataGridRow dataGridRow)
			{
				dataGridRow.Cells[CurrentColumnIndex].UpdatePseudoClasses();
			}
		}
		DataGridRow dataGridRow2 = null;
		for (Visual visual = (_focusedObject = e.Source as Visual); visual != null; visual = visual.GetVisualParent())
		{
			if (visual is DataGridRow dataGridRow3 && dataGridRow3.OwningGrid == this && _focusedRow != dataGridRow3)
			{
				ResetFocusedRow();
				_focusedRow = (dataGridRow3.IsVisible ? dataGridRow3 : null);
				break;
			}
		}
	}

	private void DataGrid_IsEnabledChanged(AvaloniaPropertyChangedEventArgs e)
	{
	}

	private void DataGrid_KeyDown(object sender, KeyEventArgs e)
	{
		if (!e.Handled)
		{
			e.Handled = ProcessDataGridKey(e);
		}
	}

	private void DataGrid_KeyUp(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Tab && CurrentColumnIndex != -1 && e.Source == this)
		{
			ScrollSlotIntoView(CurrentColumnIndex, CurrentSlot, forCurrentCellChange: false, forceHorizontalScroll: true);
			if (CurrentColumnIndex != -1 && SelectedItem == null)
			{
				SetRowSelection(CurrentSlot, isSelected: true, setAnchorSlot: true);
			}
		}
	}

	private void DataGrid_LostFocus(object sender, RoutedEventArgs e)
	{
		_focusedObject = null;
		if (!ContainsFocus)
		{
			return;
		}
		bool flag = true;
		bool flag2 = true;
		Visual visual = FocusManager.GetFocusManager(this)?.GetFocusedElement() as Visual;
		DataGridColumn dataGridColumn = null;
		while (visual != null)
		{
			if (visual == this)
			{
				flag = false;
				break;
			}
			Visual visual2 = visual.Parent as Visual;
			if (visual2 == null)
			{
				visual2 = visual.GetVisualParent();
			}
			else
			{
				flag2 = false;
			}
			visual = visual2;
		}
		if (EditingRow != null && EditingColumnIndex != -1)
		{
			dataGridColumn = ColumnsItemsInternal[EditingColumnIndex];
			if (flag && dataGridColumn is DataGridTemplateColumn)
			{
				flag2 = false;
			}
		}
		if (flag && !(dataGridColumn is DataGridTemplateColumn))
		{
			ContainsFocus = false;
			if (EditingRow != null)
			{
				CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true);
			}
			ResetFocusedRow();
			ApplyDisplayedRowsState(DisplayData.FirstScrollingSlot, DisplayData.LastScrollingSlot);
			if (CurrentColumnIndex != -1 && IsSlotVisible(CurrentSlot) && DisplayData.GetDisplayedElement(CurrentSlot) is DataGridRow dataGridRow)
			{
				dataGridRow.Cells[CurrentColumnIndex].UpdatePseudoClasses();
			}
		}
		else if (!flag2 && visual is Control control)
		{
			control.LostFocus += ExternalEditingElement_LostFocus;
		}
	}

	private void EditingElement_Initialized(object sender, EventArgs e)
	{
		Control control = sender as Control;
		if (control != null)
		{
			control.Initialized -= EditingElement_Initialized;
		}
		PreparingCellForEditPrivate(control);
	}

	private bool EndCellEdit(DataGridEditAction editAction, bool exitEditingMode, bool keepFocus, bool raiseEvents)
	{
		if (_editingColumnIndex == -1)
		{
			return true;
		}
		DataGridRow editingRow = EditingRow;
		if (editingRow == null)
		{
			return true;
		}
		int currentSlot = CurrentSlot;
		int currentColumnIndex = CurrentColumnIndex;
		DataGridCell editingCell = editingRow.Cells[_editingColumnIndex];
		Control editingElement = editingCell.Content as Control;
		if (editingElement == null)
		{
			return false;
		}
		if (raiseEvents)
		{
			DataGridCellEditEndingEventArgs dataGridCellEditEndingEventArgs = new DataGridCellEditEndingEventArgs(CurrentColumn, editingRow, editingElement, editAction);
			OnCellEditEnding(dataGridCellEditEndingEventArgs);
			if (dataGridCellEditEndingEventArgs.Cancel)
			{
				return false;
			}
			if (_editingColumnIndex == -1 || currentSlot != CurrentSlot || currentColumnIndex != CurrentColumnIndex)
			{
				return true;
			}
		}
		if (editAction == DataGridEditAction.Cancel)
		{
			CurrentColumn.CancelCellEditInternal(editingElement, _uneditedValue);
			if (_editingColumnIndex == -1 || currentSlot != CurrentSlot || currentColumnIndex != CurrentColumnIndex)
			{
				return true;
			}
		}
		if (editAction == DataGridEditAction.Commit)
		{
			ICellEditBinding editBinding = CurrentColumn?.CellEditBinding;
			if (editBinding != null && !editBinding.CommitEdit())
			{
				SetValidationStatus(editBinding);
				_validationSubscription?.Dispose();
				_validationSubscription = editBinding.ValidationChanged.Subscribe(delegate
				{
					SetValidationStatus(editBinding);
				});
				ScrollSlotIntoView(CurrentColumnIndex, CurrentSlot, forCurrentCellChange: false, forceHorizontalScroll: true);
				return false;
			}
		}
		ResetValidationStatus();
		if (exitEditingMode)
		{
			CurrentColumn.EndCellEditInternal();
			_editingColumnIndex = -1;
			editingCell.UpdatePseudoClasses();
			if (keepFocus && editingElement.ContainsFocusedElement())
			{
				Focus();
			}
			PopulateCellContent(!exitEditingMode, CurrentColumn, editingRow, editingCell);
			editingRow.InvalidateDesiredHeight();
			DataGridColumn owningColumn = editingCell.OwningColumn;
			if (owningColumn.Width.IsSizeToCells || owningColumn.Width.IsAuto)
			{
				owningColumn.SetWidthDesiredValue(0.0);
				editingRow.OwningGrid.AutoSizeColumn(owningColumn, editingCell.DesiredSize.Width);
			}
		}
		if (raiseEvents)
		{
			OnCellEditEnded(new DataGridCellEditEndedEventArgs(CurrentColumn, editingRow, editAction));
		}
		if (exitEditingMode)
		{
			return currentColumnIndex != _editingColumnIndex;
		}
		return true;
		void SetValidationStatus(ICellEditBinding binding)
		{
			if (binding.IsValid)
			{
				ResetValidationStatus();
				if (editingElement != null)
				{
					DataValidationErrors.ClearErrors(editingElement);
				}
			}
			else
			{
				if (editingRow != null)
				{
					if (editingCell.IsValid)
					{
						editingCell.IsValid = false;
						editingCell.UpdatePseudoClasses();
					}
					if (editingRow.IsValid)
					{
						editingRow.IsValid = false;
						editingRow.UpdatePseudoClasses();
					}
				}
				if (editingElement != null)
				{
					List<object> errors = binding.ValidationErrors.SelectMany(ValidationUtil.UnpackException).Select(ValidationUtil.UnpackDataValidationException).ToList();
					DataValidationErrors.SetErrors(editingElement, errors);
				}
			}
		}
	}

	private bool EndRowEdit(DataGridEditAction editAction, bool exitEditingMode, bool raiseEvents)
	{
		if (EditingRow == null || DataConnection.CommittingEdit)
		{
			return true;
		}
		if (_editingColumnIndex != -1 || (editAction == DataGridEditAction.Cancel && raiseEvents && (DataConnection.EditableCollectionView == null || !DataConnection.EditableCollectionView.CanCancelEdit) && !(EditingRow.DataContext is IEditableObject)))
		{
			return false;
		}
		DataGridRow editingRow = EditingRow;
		if (raiseEvents)
		{
			DataGridRowEditEndingEventArgs dataGridRowEditEndingEventArgs = new DataGridRowEditEndingEventArgs(EditingRow, editAction);
			OnRowEditEnding(dataGridRowEditEndingEventArgs);
			if (dataGridRowEditEndingEventArgs.Cancel)
			{
				return false;
			}
			if (_editingColumnIndex != -1)
			{
				return false;
			}
			if (editingRow != EditingRow)
			{
				return true;
			}
		}
		if (editAction == DataGridEditAction.Commit)
		{
			if (!CommitRowEdit(exitEditingMode))
			{
				return false;
			}
		}
		else if (!CancelRowEdit(exitEditingMode) && raiseEvents)
		{
			return false;
		}
		ResetValidationStatus();
		if (exitEditingMode && editingRow == EditingRow)
		{
			RemoveEditingElements();
			ResetEditingRow();
		}
		if (raiseEvents)
		{
			OnRowEditEnded(new DataGridRowEditEndedEventArgs(editingRow, editAction));
		}
		return true;
	}

	private void EnsureColumnHeadersVisibility()
	{
		if (_columnHeadersPresenter != null)
		{
			_columnHeadersPresenter.IsVisible = AreColumnHeadersVisible;
		}
	}

	private void EnsureVerticalGridLines()
	{
		if (AreColumnHeadersVisible)
		{
			double num = 0.0;
			foreach (DataGridColumn item in ColumnsInternal)
			{
				num += item.ActualWidth;
				item.HeaderCell.AreSeparatorsVisible = item != ColumnsInternal.LastVisibleColumn || num < CellsWidth;
			}
		}
		foreach (DataGridRow allRow in GetAllRows())
		{
			allRow.EnsureGridLines();
		}
	}

	private void ExitEdit(bool keepFocus)
	{
		if (EditingRow != null && !DataConnection.CommittingEdit)
		{
			if (_editingColumnIndex != -1)
			{
				_editingColumnIndex = -1;
				EditingRow.Cells[CurrentColumnIndex].UpdatePseudoClasses();
			}
			if (IsSlotVisible(EditingRow.Slot))
			{
				EditingRow.UpdatePseudoClasses();
			}
			ResetEditingRow();
			if (keepFocus)
			{
				Focus();
			}
		}
	}

	private void ExternalEditingElement_LostFocus(object sender, RoutedEventArgs e)
	{
		if (sender is Control control)
		{
			control.LostFocus -= ExternalEditingElement_LostFocus;
			DataGrid_LostFocus(sender, e);
		}
	}

	private void FlushCurrentCellChanged()
	{
		if (_makeFirstDisplayedCellCurrentCellPending)
		{
			return;
		}
		if (SelectionHasChanged)
		{
			_flushCurrentCellChanged = true;
			FlushSelectionChanged();
			return;
		}
		if (_collapsedSlotsTable.Contains(CurrentSlot))
		{
			DataGridRowGroupInfo valueAt = RowGroupHeadersTable.GetValueAt(RowGroupHeadersTable.GetPreviousIndex(CurrentSlot));
			if (valueAt != null)
			{
				ExpandRowGroupParentChain(valueAt.Level, valueAt.Slot);
			}
		}
		if (CurrentColumn != _previousCurrentColumn || CurrentItem != _previousCurrentItem)
		{
			CoerceSelectedItem();
			_previousCurrentColumn = CurrentColumn;
			_previousCurrentItem = CurrentItem;
			OnCurrentCellChanged(EventArgs.Empty);
		}
		_flushCurrentCellChanged = false;
	}

	private void FlushSelectionChanged()
	{
		if (!SelectionHasChanged || _noSelectionChangeCount != 0 || _makeFirstDisplayedCellCurrentCellPending)
		{
			return;
		}
		CoerceSelectedItem();
		if (NoCurrentCellChangeCount == 0)
		{
			SelectionHasChanged = false;
			if (_flushCurrentCellChanged)
			{
				FlushCurrentCellChanged();
			}
			SelectionChangedEventArgs selectionChangedEventArgs = _selectedItems.GetSelectionChangedEventArgs();
			if (selectionChangedEventArgs.AddedItems.Count > 0 || selectionChangedEventArgs.RemovedItems.Count > 0)
			{
				OnSelectionChanged(selectionChangedEventArgs);
			}
		}
	}

	private bool FocusEditingCell(bool setFocus)
	{
		_focusEditingControl = false;
		bool flag = false;
		DataGridCell dataGridCell = EditingRow.Cells[_editingColumnIndex];
		if (setFocus)
		{
			if (dataGridCell.ContainsFocusedElement())
			{
				flag = true;
			}
			else
			{
				dataGridCell.Focus();
				flag = dataGridCell.ContainsFocusedElement();
			}
			_focusEditingControl = !flag;
		}
		return flag;
	}

	private double GetHorizontalSmallScrollDecrease()
	{
		if (_negHorizontalOffset > 0.0)
		{
			return _negHorizontalOffset;
		}
		DataGridColumn previousVisibleScrollingColumn = ColumnsInternal.GetPreviousVisibleScrollingColumn(ColumnsItemsInternal[DisplayData.FirstDisplayedScrollingCol]);
		if (previousVisibleScrollingColumn != null)
		{
			return GetEdgedColumnWidth(previousVisibleScrollingColumn);
		}
		return 0.0;
	}

	private double GetHorizontalSmallScrollIncrease()
	{
		if (DisplayData.FirstDisplayedScrollingCol >= 0)
		{
			return GetEdgedColumnWidth(ColumnsItemsInternal[DisplayData.FirstDisplayedScrollingCol]) - _negHorizontalOffset;
		}
		return 0.0;
	}

	private double GetVerticalSmallScrollIncrease()
	{
		if (DisplayData.FirstScrollingSlot >= 0)
		{
			return GetExactSlotElementHeight(DisplayData.FirstScrollingSlot) - NegVerticalOffset;
		}
		return 0.0;
	}

	private void HorizontalScrollBar_Scroll(object sender, ScrollEventArgs e)
	{
		ProcessHorizontalScroll(e.ScrollEventType);
		this.HorizontalScroll?.Invoke(sender, e);
	}

	private bool IsColumnOutOfBounds(int columnIndex)
	{
		if (columnIndex < ColumnsItemsInternal.Count)
		{
			return columnIndex < 0;
		}
		return true;
	}

	private bool IsInnerCellOutOfBounds(int columnIndex, int slot)
	{
		if (!IsColumnOutOfBounds(columnIndex))
		{
			return IsSlotOutOfBounds(slot);
		}
		return true;
	}

	private bool IsInnerCellOutOfSelectionBounds(int columnIndex, int slot)
	{
		if (!IsColumnOutOfBounds(columnIndex))
		{
			return IsSlotOutOfSelectionBounds(slot);
		}
		return true;
	}

	private bool IsSlotOutOfBounds(int slot)
	{
		if (slot < SlotCount && slot >= -1)
		{
			return _collapsedSlotsTable.Contains(slot);
		}
		return true;
	}

	private bool IsSlotOutOfSelectionBounds(int slot)
	{
		if (RowGroupHeadersTable.Contains(slot))
		{
			return false;
		}
		int num = RowIndexFromSlot(slot);
		if (num >= 0)
		{
			return num >= DataConnection.Count;
		}
		return true;
	}

	private void MakeFirstDisplayedCellCurrentCell()
	{
		if (CurrentColumnIndex != -1)
		{
			_makeFirstDisplayedCellCurrentCellPending = false;
			_desiredCurrentColumnIndex = -1;
			FlushCurrentCellChanged();
			return;
		}
		if (SlotCount != SlotFromRowIndex(DataConnection.Count))
		{
			_makeFirstDisplayedCellCurrentCellPending = true;
			return;
		}
		int num = 0;
		if (DataConnection.CollectionView != null)
		{
			num = ((!DataConnection.CollectionView.IsCurrentBeforeFirst && !DataConnection.CollectionView.IsCurrentAfterLast) ? SlotFromRowIndex(DataConnection.CollectionView.CurrentPosition) : ((!RowGroupHeadersTable.Contains(0)) ? (-1) : 0));
		}
		else if (SelectedIndex == -1)
		{
			num = SlotFromRowIndex(0);
			if (!IsSlotVisible(num))
			{
				num = -1;
			}
		}
		else
		{
			num = SlotFromRowIndex(SelectedIndex);
		}
		int columnIndex = FirstDisplayedNonFillerColumnIndex;
		if (_desiredCurrentColumnIndex >= 0 && _desiredCurrentColumnIndex < ColumnsItemsInternal.Count)
		{
			columnIndex = _desiredCurrentColumnIndex;
		}
		SetAndSelectCurrentCell(columnIndex, num, forceCurrentCellSelection: false);
		AnchorSlot = num;
		_makeFirstDisplayedCellCurrentCellPending = false;
		_desiredCurrentColumnIndex = -1;
		FlushCurrentCellChanged();
	}

	private void PopulateCellContent(bool isCellEdited, DataGridColumn dataGridColumn, DataGridRow dataGridRow, DataGridCell dataGridCell)
	{
		Control control = null;
		if (isCellEdited)
		{
			control = dataGridColumn.GenerateEditingElementInternal(dataGridCell, dataGridRow.DataContext);
			if (control != null)
			{
				dataGridCell.Content = control;
				if (control.IsInitialized)
				{
					PreparingCellForEditPrivate(control);
				}
				else
				{
					control.Initialized += EditingElement_Initialized;
				}
			}
		}
		else
		{
			control = dataGridColumn.GenerateElementInternal(dataGridCell, dataGridRow.DataContext);
			dataGridCell.Content = control;
		}
	}

	private void PreparingCellForEditPrivate(Control editingElement)
	{
		if (_editingColumnIndex != -1 && CurrentColumnIndex != -1 && EditingRow.Cells[CurrentColumnIndex].Content == editingElement)
		{
			FocusEditingCell(ContainsFocus || _focusEditingControl);
			DataGridColumn currentColumn = CurrentColumn;
			_uneditedValue = currentColumn.PrepareCellForEditInternal(editingElement, _editingEventArgs);
			OnPreparingCellForEdit(new DataGridPreparingCellForEditEventArgs(currentColumn, EditingRow, _editingEventArgs, editingElement));
		}
	}

	private bool ProcessAKey(KeyEventArgs e)
	{
		KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrlOrCmd, out var shift, out var alt);
		if (ctrlOrCmd && !shift && !alt && SelectionMode == DataGridSelectionMode.Extended)
		{
			SelectAll();
			return true;
		}
		return false;
	}

	private bool ProcessDataGridKey(KeyEventArgs e)
	{
		bool flag = false;
		switch (e.Key)
		{
		case Key.Tab:
			return ProcessTabKey(e);
		case Key.Up:
			flag = ProcessUpKey(e);
			break;
		case Key.Down:
			flag = ProcessDownKey(e);
			break;
		case Key.PageDown:
			flag = ProcessNextKey(e);
			break;
		case Key.PageUp:
			flag = ProcessPriorKey(e);
			break;
		case Key.Left:
			flag = ProcessLeftKey(e);
			break;
		case Key.Right:
			flag = ProcessRightKey(e);
			break;
		case Key.F2:
			return ProcessF2Key(e);
		case Key.Home:
			flag = ProcessHomeKey(e);
			break;
		case Key.End:
			flag = ProcessEndKey(e);
			break;
		case Key.Return:
			flag = ProcessEnterKey(e);
			break;
		case Key.Escape:
			return ProcessEscapeKey();
		case Key.A:
			return ProcessAKey(e);
		case Key.C:
			return ProcessCopyKey(e.KeyModifiers);
		case Key.Insert:
			return ProcessCopyKey(e.KeyModifiers);
		}
		if (flag)
		{
			Focus();
		}
		return flag;
	}

	private bool ProcessDownKeyInternal(bool shift, bool ctrl)
	{
		int num = ColumnsInternal.FirstVisibleColumn?.Index ?? (-1);
		int lastVisibleSlot = LastVisibleSlot;
		if (num == -1 || lastVisibleSlot == -1)
		{
			return false;
		}
		if (WaitForLostFocus(delegate
		{
			ProcessDownKeyInternal(shift, ctrl);
		}))
		{
			return true;
		}
		int num2 = -1;
		if (CurrentSlot != -1)
		{
			num2 = GetNextVisibleSlot(CurrentSlot);
			if (num2 >= SlotCount)
			{
				num2 = -1;
			}
		}
		_noSelectionChangeCount++;
		try
		{
			int slot;
			int columnIndex;
			DataGridSelectionAction action;
			if (CurrentColumnIndex == -1)
			{
				slot = FirstVisibleSlot;
				columnIndex = num;
				action = DataGridSelectionAction.SelectCurrent;
			}
			else if (ctrl)
			{
				if (shift)
				{
					slot = lastVisibleSlot;
					columnIndex = CurrentColumnIndex;
					action = ((SelectionMode == DataGridSelectionMode.Extended) ? DataGridSelectionAction.SelectFromAnchorToCurrent : DataGridSelectionAction.SelectCurrent);
				}
				else
				{
					slot = lastVisibleSlot;
					columnIndex = CurrentColumnIndex;
					action = DataGridSelectionAction.SelectCurrent;
				}
			}
			else
			{
				if (num2 == -1)
				{
					return true;
				}
				if (shift)
				{
					slot = num2;
					columnIndex = CurrentColumnIndex;
					action = DataGridSelectionAction.SelectFromAnchorToCurrent;
				}
				else
				{
					slot = num2;
					columnIndex = CurrentColumnIndex;
					action = DataGridSelectionAction.SelectCurrent;
				}
			}
			UpdateSelectionAndCurrency(columnIndex, slot, action, scrollIntoView: true);
		}
		finally
		{
			NoSelectionChangeCount--;
		}
		return _successfullyUpdatedSelection;
	}

	private bool ProcessEndKey(bool shift, bool ctrl)
	{
		int num = ColumnsInternal.LastVisibleColumn?.Index ?? (-1);
		int firstVisibleSlot = FirstVisibleSlot;
		int lastVisibleSlot = LastVisibleSlot;
		if (num == -1 || firstVisibleSlot == -1)
		{
			return false;
		}
		if (WaitForLostFocus(delegate
		{
			ProcessEndKey(shift, ctrl);
		}))
		{
			return true;
		}
		_noSelectionChangeCount++;
		try
		{
			if (!ctrl)
			{
				return ProcessRightMost(num, firstVisibleSlot);
			}
			DataGridSelectionAction action = ((shift && SelectionMode == DataGridSelectionMode.Extended) ? DataGridSelectionAction.SelectFromAnchorToCurrent : DataGridSelectionAction.SelectCurrent);
			UpdateSelectionAndCurrency(num, lastVisibleSlot, action, scrollIntoView: true);
		}
		finally
		{
			NoSelectionChangeCount--;
		}
		return _successfullyUpdatedSelection;
	}

	private bool ProcessEnterKey(bool shift, bool ctrl)
	{
		int currentSlot = CurrentSlot;
		if (!ctrl)
		{
			if (FocusManager.GetFocusManager(this)?.GetFocusedElement() is TextBox { AcceptsReturn: not false })
			{
				return false;
			}
			if (WaitForLostFocus(delegate
			{
				ProcessEnterKey(shift, ctrl);
			}))
			{
				return true;
			}
			if (!ProcessDownKeyInternal(shift: false, ctrl))
			{
				return false;
			}
		}
		else if (WaitForLostFocus(delegate
		{
			ProcessEnterKey(shift, ctrl);
		}))
		{
			return true;
		}
		if (currentSlot == CurrentSlot && EndCellEdit(DataGridEditAction.Commit, exitEditingMode: true, keepFocus: true, raiseEvents: true) && EditingRow != null)
		{
			EndRowEdit(DataGridEditAction.Commit, exitEditingMode: true, raiseEvents: true);
			ScrollIntoView(CurrentItem, CurrentColumn);
		}
		return true;
	}

	private bool ProcessEscapeKey()
	{
		if (WaitForLostFocus(delegate
		{
			ProcessEscapeKey();
		}))
		{
			return true;
		}
		if (_editingColumnIndex != -1)
		{
			EndCellEdit(DataGridEditAction.Cancel, exitEditingMode: true, keepFocus: true, raiseEvents: true);
			return true;
		}
		if (EditingRow != null)
		{
			EndRowEdit(DataGridEditAction.Cancel, exitEditingMode: true, raiseEvents: true);
			return true;
		}
		return false;
	}

	private bool ProcessF2Key(KeyEventArgs e)
	{
		KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrlOrCmd, out var shift);
		if (!shift && !ctrlOrCmd && _editingColumnIndex == -1 && CurrentColumnIndex != -1 && GetRowSelection(CurrentSlot) && !GetColumnEffectiveReadOnlyState(CurrentColumn))
		{
			if (ScrollSlotIntoView(CurrentColumnIndex, CurrentSlot, forCurrentCellChange: false, forceHorizontalScroll: true))
			{
				BeginCellEdit(e);
			}
			return true;
		}
		return false;
	}

	private bool ProcessHomeKey(bool shift, bool ctrl)
	{
		int num = ColumnsInternal.FirstVisibleNonFillerColumn?.Index ?? (-1);
		int firstVisibleSlot = FirstVisibleSlot;
		if (num == -1 || firstVisibleSlot == -1)
		{
			return false;
		}
		if (WaitForLostFocus(delegate
		{
			ProcessHomeKey(shift, ctrl);
		}))
		{
			return true;
		}
		_noSelectionChangeCount++;
		try
		{
			if (!ctrl)
			{
				return ProcessLeftMost(num, firstVisibleSlot);
			}
			DataGridSelectionAction action = ((shift && SelectionMode == DataGridSelectionMode.Extended) ? DataGridSelectionAction.SelectFromAnchorToCurrent : DataGridSelectionAction.SelectCurrent);
			UpdateSelectionAndCurrency(num, firstVisibleSlot, action, scrollIntoView: true);
		}
		finally
		{
			NoSelectionChangeCount--;
		}
		return _successfullyUpdatedSelection;
	}

	private bool ProcessLeftKey(bool shift, bool ctrl)
	{
		int num = ColumnsInternal.FirstVisibleNonFillerColumn?.Index ?? (-1);
		int firstVisibleSlot = FirstVisibleSlot;
		if (num == -1 || firstVisibleSlot == -1)
		{
			return false;
		}
		if (WaitForLostFocus(delegate
		{
			ProcessLeftKey(shift, ctrl);
		}))
		{
			return true;
		}
		int num2 = -1;
		if (CurrentColumnIndex != -1)
		{
			DataGridColumn previousVisibleNonFillerColumn = ColumnsInternal.GetPreviousVisibleNonFillerColumn(ColumnsItemsInternal[CurrentColumnIndex]);
			if (previousVisibleNonFillerColumn != null)
			{
				num2 = previousVisibleNonFillerColumn.Index;
			}
		}
		_noSelectionChangeCount++;
		try
		{
			if (ctrl)
			{
				return ProcessLeftMost(num, firstVisibleSlot);
			}
			if (RowGroupHeadersTable.Contains(CurrentSlot))
			{
				CollapseRowGroup(RowGroupHeadersTable.GetValueAt(CurrentSlot).CollectionViewGroup, collapseAllSubgroups: false);
			}
			else if (CurrentColumnIndex == -1)
			{
				UpdateSelectionAndCurrency(num, firstVisibleSlot, DataGridSelectionAction.SelectCurrent, scrollIntoView: true);
			}
			else
			{
				if (num2 == -1)
				{
					return true;
				}
				UpdateSelectionAndCurrency(num2, CurrentSlot, DataGridSelectionAction.None, scrollIntoView: true);
			}
		}
		finally
		{
			NoSelectionChangeCount--;
		}
		return _successfullyUpdatedSelection;
	}

	private bool ProcessLeftMost(int firstVisibleColumnIndex, int firstVisibleSlot)
	{
		_noSelectionChangeCount++;
		try
		{
			int slot;
			DataGridSelectionAction action;
			if (CurrentColumnIndex == -1)
			{
				slot = firstVisibleSlot;
				action = DataGridSelectionAction.SelectCurrent;
			}
			else
			{
				slot = CurrentSlot;
				action = DataGridSelectionAction.None;
			}
			UpdateSelectionAndCurrency(firstVisibleColumnIndex, slot, action, scrollIntoView: true);
		}
		finally
		{
			NoSelectionChangeCount--;
		}
		return _successfullyUpdatedSelection;
	}

	private bool ProcessNextKey(bool shift, bool ctrl)
	{
		int num = ColumnsInternal.FirstVisibleNonFillerColumn?.Index ?? (-1);
		if (num == -1 || DisplayData.FirstScrollingSlot == -1)
		{
			return false;
		}
		if (WaitForLostFocus(delegate
		{
			ProcessNextKey(shift, ctrl);
		}))
		{
			return true;
		}
		int slot = ((CurrentSlot == -1) ? DisplayData.FirstScrollingSlot : CurrentSlot);
		int nextVisibleSlot = GetNextVisibleSlot(slot);
		int num2 = DisplayData.NumTotallyDisplayedScrollingElements;
		while (num2 > 0 && nextVisibleSlot < SlotCount)
		{
			slot = nextVisibleSlot;
			num2--;
			nextVisibleSlot = GetNextVisibleSlot(nextVisibleSlot);
		}
		_noSelectionChangeCount++;
		try
		{
			int columnIndex;
			DataGridSelectionAction action;
			if (CurrentColumnIndex == -1)
			{
				columnIndex = num;
				action = DataGridSelectionAction.SelectCurrent;
			}
			else
			{
				columnIndex = CurrentColumnIndex;
				action = ((!shift || SelectionMode != 0) ? (action = DataGridSelectionAction.SelectCurrent) : (action = DataGridSelectionAction.SelectFromAnchorToCurrent));
			}
			UpdateSelectionAndCurrency(columnIndex, slot, action, scrollIntoView: true);
		}
		finally
		{
			NoSelectionChangeCount--;
		}
		return _successfullyUpdatedSelection;
	}

	private bool ProcessPriorKey(bool shift, bool ctrl)
	{
		int num = ColumnsInternal.FirstVisibleNonFillerColumn?.Index ?? (-1);
		if (num == -1 || DisplayData.FirstScrollingSlot == -1)
		{
			return false;
		}
		if (WaitForLostFocus(delegate
		{
			ProcessPriorKey(shift, ctrl);
		}))
		{
			return true;
		}
		int slot = ((CurrentSlot == -1) ? DisplayData.FirstScrollingSlot : CurrentSlot);
		int num2 = DisplayData.NumTotallyDisplayedScrollingElements;
		int previousVisibleSlot = GetPreviousVisibleSlot(slot);
		while (num2 > 0 && previousVisibleSlot != -1)
		{
			slot = previousVisibleSlot;
			num2--;
			previousVisibleSlot = GetPreviousVisibleSlot(previousVisibleSlot);
		}
		_noSelectionChangeCount++;
		try
		{
			int columnIndex;
			DataGridSelectionAction action;
			if (CurrentColumnIndex == -1)
			{
				columnIndex = num;
				action = DataGridSelectionAction.SelectCurrent;
			}
			else
			{
				columnIndex = CurrentColumnIndex;
				action = ((shift && SelectionMode == DataGridSelectionMode.Extended) ? DataGridSelectionAction.SelectFromAnchorToCurrent : DataGridSelectionAction.SelectCurrent);
			}
			UpdateSelectionAndCurrency(columnIndex, slot, action, scrollIntoView: true);
		}
		finally
		{
			NoSelectionChangeCount--;
		}
		return _successfullyUpdatedSelection;
	}

	private bool ProcessRightKey(bool shift, bool ctrl)
	{
		int num = ColumnsInternal.LastVisibleColumn?.Index ?? (-1);
		int firstVisibleSlot = FirstVisibleSlot;
		if (num == -1 || firstVisibleSlot == -1)
		{
			return false;
		}
		if (WaitForLostFocus(delegate
		{
			ProcessRightKey(shift, ctrl);
		}))
		{
			return true;
		}
		int num2 = -1;
		if (CurrentColumnIndex != -1)
		{
			DataGridColumn nextVisibleColumn = ColumnsInternal.GetNextVisibleColumn(ColumnsItemsInternal[CurrentColumnIndex]);
			if (nextVisibleColumn != null)
			{
				num2 = nextVisibleColumn.Index;
			}
		}
		_noSelectionChangeCount++;
		try
		{
			if (ctrl)
			{
				return ProcessRightMost(num, firstVisibleSlot);
			}
			if (RowGroupHeadersTable.Contains(CurrentSlot))
			{
				ExpandRowGroup(RowGroupHeadersTable.GetValueAt(CurrentSlot).CollectionViewGroup, expandAllSubgroups: false);
			}
			else if (CurrentColumnIndex == -1)
			{
				int columnIndex = ((ColumnsInternal.FirstVisibleColumn == null) ? (-1) : ColumnsInternal.FirstVisibleColumn.Index);
				UpdateSelectionAndCurrency(columnIndex, firstVisibleSlot, DataGridSelectionAction.SelectCurrent, scrollIntoView: true);
			}
			else
			{
				if (num2 == -1)
				{
					return true;
				}
				UpdateSelectionAndCurrency(num2, CurrentSlot, DataGridSelectionAction.None, scrollIntoView: true);
			}
		}
		finally
		{
			NoSelectionChangeCount--;
		}
		return _successfullyUpdatedSelection;
	}

	private bool ProcessRightMost(int lastVisibleColumnIndex, int firstVisibleSlot)
	{
		_noSelectionChangeCount++;
		try
		{
			int slot;
			DataGridSelectionAction action;
			if (CurrentColumnIndex == -1)
			{
				slot = firstVisibleSlot;
				action = DataGridSelectionAction.SelectCurrent;
			}
			else
			{
				slot = CurrentSlot;
				action = DataGridSelectionAction.None;
			}
			UpdateSelectionAndCurrency(lastVisibleColumnIndex, slot, action, scrollIntoView: true);
		}
		finally
		{
			NoSelectionChangeCount--;
		}
		return _successfullyUpdatedSelection;
	}

	private bool ProcessTabKey(KeyEventArgs e)
	{
		KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrlOrCmd, out var shift);
		return ProcessTabKey(e, shift, ctrlOrCmd);
	}

	private bool ProcessTabKey(KeyEventArgs e, bool shift, bool ctrl)
	{
		if (ctrl || _editingColumnIndex == -1 || IsReadOnly)
		{
			return false;
		}
		DataGridColumn dataGridColumn;
		int num;
		if (shift)
		{
			dataGridColumn = ColumnsInternal.GetPreviousVisibleWritableColumn(ColumnsItemsInternal[CurrentColumnIndex]);
			num = GetPreviousVisibleSlot(CurrentSlot);
			if (EditingRow != null)
			{
				while (num != -1 && RowGroupHeadersTable.Contains(num))
				{
					num = GetPreviousVisibleSlot(num);
				}
			}
		}
		else
		{
			dataGridColumn = ColumnsInternal.GetNextVisibleWritableColumn(ColumnsItemsInternal[CurrentColumnIndex]);
			num = GetNextVisibleSlot(CurrentSlot);
			if (EditingRow != null)
			{
				while (num < SlotCount && RowGroupHeadersTable.Contains(num))
				{
					num = GetNextVisibleSlot(num);
				}
			}
		}
		int num2 = dataGridColumn?.Index ?? (-1);
		if (num2 == -1 && (num == -1 || num >= SlotCount))
		{
			return false;
		}
		if (WaitForLostFocus(delegate
		{
			ProcessTabKey(e, shift, ctrl);
		}))
		{
			return true;
		}
		int num3 = -1;
		int num4 = -1;
		_noSelectionChangeCount++;
		try
		{
			if (num2 == -1)
			{
				num3 = num;
				num4 = ((!shift) ? ColumnsInternal.FirstVisibleWritableColumn.Index : ColumnsInternal.LastVisibleWritableColumn.Index);
			}
			else
			{
				num3 = CurrentSlot;
				num4 = num2;
			}
			DataGridSelectionAction action;
			if (num3 != CurrentSlot || SelectionMode == DataGridSelectionMode.Extended)
			{
				if (IsSlotOutOfBounds(num3))
				{
					return true;
				}
				action = DataGridSelectionAction.SelectCurrent;
			}
			else
			{
				action = DataGridSelectionAction.None;
			}
			UpdateSelectionAndCurrency(num4, num3, action, scrollIntoView: true);
		}
		finally
		{
			NoSelectionChangeCount--;
		}
		if (_successfullyUpdatedSelection && !RowGroupHeadersTable.Contains(num3))
		{
			BeginCellEdit(e);
		}
		return true;
	}

	private bool ProcessUpKey(bool shift, bool ctrl)
	{
		int num = ColumnsInternal.FirstVisibleNonFillerColumn?.Index ?? (-1);
		int firstVisibleSlot = FirstVisibleSlot;
		if (num == -1 || firstVisibleSlot == -1)
		{
			return false;
		}
		if (WaitForLostFocus(delegate
		{
			ProcessUpKey(shift, ctrl);
		}))
		{
			return true;
		}
		int num2 = ((CurrentSlot != -1) ? GetPreviousVisibleSlot(CurrentSlot) : (-1));
		_noSelectionChangeCount++;
		try
		{
			int slot;
			int columnIndex;
			DataGridSelectionAction action;
			if (CurrentColumnIndex == -1)
			{
				slot = firstVisibleSlot;
				columnIndex = num;
				action = DataGridSelectionAction.SelectCurrent;
			}
			else if (ctrl)
			{
				if (shift)
				{
					slot = firstVisibleSlot;
					columnIndex = CurrentColumnIndex;
					action = ((SelectionMode == DataGridSelectionMode.Extended) ? DataGridSelectionAction.SelectFromAnchorToCurrent : DataGridSelectionAction.SelectCurrent);
				}
				else
				{
					slot = firstVisibleSlot;
					columnIndex = CurrentColumnIndex;
					action = DataGridSelectionAction.SelectCurrent;
				}
			}
			else
			{
				if (num2 == -1)
				{
					return true;
				}
				if (shift)
				{
					slot = num2;
					columnIndex = CurrentColumnIndex;
					action = DataGridSelectionAction.SelectFromAnchorToCurrent;
				}
				else
				{
					slot = num2;
					columnIndex = CurrentColumnIndex;
					action = DataGridSelectionAction.SelectCurrent;
				}
			}
			UpdateSelectionAndCurrency(columnIndex, slot, action, scrollIntoView: true);
		}
		finally
		{
			NoSelectionChangeCount--;
		}
		return _successfullyUpdatedSelection;
	}

	private void RemoveDisplayedColumnHeader(DataGridColumn dataGridColumn)
	{
		if (_columnHeadersPresenter != null)
		{
			_columnHeadersPresenter.Children.Remove(dataGridColumn.HeaderCell);
		}
	}

	private void RemoveDisplayedColumnHeaders()
	{
		if (_columnHeadersPresenter != null)
		{
			_columnHeadersPresenter.Children.Clear();
		}
		ColumnsInternal.FillerColumn.IsRepresented = false;
	}

	private bool ResetCurrentCellCore()
	{
		if (CurrentColumnIndex != -1)
		{
			return SetCurrentCellCore(-1, -1);
		}
		return true;
	}

	private void ResetEditingRow()
	{
		if (EditingRow != null && EditingRow != _focusedRow && !IsSlotVisible(EditingRow.Slot))
		{
			EditingRow.Clip = null;
			UnloadRow(EditingRow);
			DisplayData.FullyRecycleElements();
		}
		EditingRow = null;
	}

	private void ResetFocusedRow()
	{
		if (_focusedRow != null && _focusedRow != EditingRow && !IsSlotVisible(_focusedRow.Slot))
		{
			_focusedRow.Clip = null;
			UnloadRow(_focusedRow);
			DisplayData.FullyRecycleElements();
		}
		_focusedRow = null;
	}

	public void SelectAll()
	{
		SetRowsSelection(0, SlotCount - 1);
	}

	private void SetAndSelectCurrentCell(int columnIndex, int slot, bool forceCurrentCellSelection)
	{
		DataGridSelectionAction action = ((!forceCurrentCellSelection) ? DataGridSelectionAction.None : DataGridSelectionAction.SelectCurrent);
		UpdateSelectionAndCurrency(columnIndex, slot, action, scrollIntoView: false);
	}

	private bool SetCurrentCellCore(int columnIndex, int slot, bool commitEdit, bool endRowEdit)
	{
		if (columnIndex == CurrentColumnIndex && slot == CurrentSlot)
		{
			return true;
		}
		Control control = null;
		DataGridCellCoordinates dataGridCellCoordinates = new DataGridCellCoordinates(CurrentCellCoordinates);
		object obj = null;
		if (!RowGroupHeadersTable.Contains(slot))
		{
			int num = RowIndexFromSlot(slot);
			if (num >= 0 && num < DataConnection.Count)
			{
				obj = DataConnection.GetDataItem(num);
			}
		}
		if (CurrentColumnIndex > -1)
		{
			if (!IsInnerCellOutOfBounds(dataGridCellCoordinates.ColumnIndex, dataGridCellCoordinates.Slot) && IsSlotVisible(dataGridCellCoordinates.Slot))
			{
				control = DisplayData.GetDisplayedElement(dataGridCellCoordinates.Slot);
			}
			if (!RowGroupHeadersTable.Contains(dataGridCellCoordinates.Slot) && !_temporarilyResetCurrentCell)
			{
				bool containsFocus = ContainsFocus;
				if (commitEdit)
				{
					if (!EndCellEdit(DataGridEditAction.Commit, exitEditingMode: true, containsFocus, raiseEvents: true))
					{
						return false;
					}
					if ((columnIndex != -1 && slot != -1 && IsInnerCellOutOfSelectionBounds(columnIndex, slot)) || IsInnerCellOutOfSelectionBounds(dataGridCellCoordinates.ColumnIndex, dataGridCellCoordinates.Slot))
					{
						return false;
					}
					if (endRowEdit && !EndRowEdit(DataGridEditAction.Commit, exitEditingMode: true, raiseEvents: true))
					{
						return false;
					}
				}
				else
				{
					CancelEdit(DataGridEditingUnit.Row, raiseEvents: false);
					ExitEdit(containsFocus);
				}
			}
		}
		if (obj != null)
		{
			slot = SlotFromRowIndex(DataConnection.IndexOf(obj));
		}
		if (slot == -1 && columnIndex != -1)
		{
			return false;
		}
		CurrentColumnIndex = columnIndex;
		CurrentSlot = slot;
		if (_temporarilyResetCurrentCell && columnIndex != -1)
		{
			_temporarilyResetCurrentCell = false;
		}
		if (!_temporarilyResetCurrentCell && _editingColumnIndex != -1)
		{
			_editingColumnIndex = columnIndex;
		}
		if (control != null)
		{
			if (control is DataGridRow dataGridRow)
			{
				UpdateCurrentState(control, dataGridCellCoordinates.ColumnIndex, !_temporarilyResetCurrentCell || !dataGridRow.IsEditing || _editingColumnIndex != dataGridCellCoordinates.ColumnIndex);
			}
			else
			{
				UpdateCurrentState(control, dataGridCellCoordinates.ColumnIndex, applyCellState: false);
			}
		}
		if (CurrentColumnIndex > -1 && IsSlotVisible(CurrentSlot))
		{
			UpdateCurrentState(DisplayData.GetDisplayedElement(CurrentSlot), CurrentColumnIndex, applyCellState: true);
		}
		return true;
	}

	private void SetVerticalOffset(double newVerticalOffset)
	{
		_verticalOffset = newVerticalOffset;
		if (_vScrollBar != null && !MathUtilities.AreClose(newVerticalOffset, _vScrollBar.Value))
		{
			_vScrollBar.Value = _verticalOffset;
		}
	}

	private void UpdateCurrentState(Control displayedElement, int columnIndex, bool applyCellState)
	{
		if (displayedElement is DataGridRow dataGridRow)
		{
			if (AreRowHeadersVisible)
			{
				dataGridRow.ApplyHeaderStatus();
			}
			DataGridCell dataGridCell = dataGridRow.Cells[columnIndex];
			if (applyCellState)
			{
				dataGridCell.UpdatePseudoClasses();
			}
		}
		else if (displayedElement is DataGridRowGroupHeader dataGridRowGroupHeader)
		{
			dataGridRowGroupHeader.UpdatePseudoClasses();
			if (AreRowHeadersVisible)
			{
				dataGridRowGroupHeader.ApplyHeaderStatus();
			}
		}
	}

	private void UpdateHorizontalScrollBar(bool needHorizScrollbar, bool forceHorizScrollbar, double totalVisibleWidth, double totalVisibleFrozenWidth, double cellsWidth)
	{
		if (_hScrollBar == null)
		{
			return;
		}
		if (needHorizScrollbar || forceHorizScrollbar)
		{
			_hScrollBar.Minimum = 0.0;
			if (needHorizScrollbar)
			{
				_hScrollBar.Maximum = totalVisibleWidth - cellsWidth;
				if (_frozenColumnScrollBarSpacer != null)
				{
					_frozenColumnScrollBarSpacer.Width = totalVisibleFrozenWidth;
				}
				double num = Math.Max(0.0, cellsWidth - totalVisibleFrozenWidth);
				_hScrollBar.ViewportSize = num;
				_hScrollBar.LargeChange = num;
				if (_hScrollBar.Value != _horizontalOffset)
				{
					_hScrollBar.Value = _horizontalOffset;
				}
				_hScrollBar.IsEnabled = true;
			}
			else
			{
				_hScrollBar.Maximum = 0.0;
				_hScrollBar.ViewportSize = 0.0;
				_hScrollBar.IsEnabled = false;
			}
			if (!_hScrollBar.IsVisible)
			{
				_ignoreNextScrollBarsLayout = true;
				_hScrollBar.IsVisible = true;
				if (_hScrollBar.DesiredSize.Height == 0.0)
				{
					_hScrollBar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				}
			}
		}
		else
		{
			_hScrollBar.Maximum = 0.0;
			if (_hScrollBar.IsVisible)
			{
				_hScrollBar.IsVisible = false;
				_ignoreNextScrollBarsLayout = true;
			}
		}
	}

	private void UpdateVerticalScrollBar(bool needVertScrollbar, bool forceVertScrollbar, double totalVisibleHeight, double cellsHeight)
	{
		if (_vScrollBar == null)
		{
			return;
		}
		if (needVertScrollbar || forceVertScrollbar)
		{
			_vScrollBar.Minimum = 0.0;
			if (needVertScrollbar && !double.IsInfinity(cellsHeight))
			{
				_vScrollBar.Maximum = totalVisibleHeight - cellsHeight;
				_vScrollBar.ViewportSize = cellsHeight;
				_vScrollBar.IsEnabled = true;
			}
			else
			{
				_vScrollBar.Maximum = 0.0;
				_vScrollBar.ViewportSize = 0.0;
				_vScrollBar.IsEnabled = false;
			}
			if (!_vScrollBar.IsVisible)
			{
				_vScrollBar.IsVisible = true;
				if (_vScrollBar.DesiredSize.Width == 0.0)
				{
					_vScrollBar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				}
				_ignoreNextScrollBarsLayout = true;
			}
		}
		else
		{
			_vScrollBar.Maximum = 0.0;
			if (_vScrollBar.IsVisible)
			{
				_vScrollBar.IsVisible = false;
				_ignoreNextScrollBarsLayout = true;
			}
		}
	}

	private void VerticalScrollBar_Scroll(object sender, ScrollEventArgs e)
	{
		ProcessVerticalScroll(e.ScrollEventType);
		this.VerticalScroll?.Invoke(sender, e);
	}

	private bool UpdateStateOnMouseRightButtonDown(PointerPressedEventArgs pointerPressedEventArgs, int columnIndex, int slot, bool allowEdit, bool shift, bool ctrl)
	{
		if (shift || ctrl)
		{
			return true;
		}
		if (IsSlotOutOfBounds(slot))
		{
			return true;
		}
		if (GetRowSelection(slot))
		{
			return true;
		}
		_noSelectionChangeCount++;
		try
		{
			UpdateSelectionAndCurrency(columnIndex, slot, DataGridSelectionAction.SelectCurrent, scrollIntoView: false);
		}
		finally
		{
			NoSelectionChangeCount--;
		}
		return true;
	}

	private bool UpdateStateOnMouseLeftButtonDown(PointerPressedEventArgs pointerPressedEventArgs, int columnIndex, int slot, bool allowEdit, bool shift, bool ctrl)
	{
		bool flag = EditingColumnIndex != -1;
		if (IsSlotOutOfBounds(slot))
		{
			return true;
		}
		if (flag && (columnIndex != EditingColumnIndex || slot != CurrentSlot) && WaitForLostFocus(delegate
		{
			UpdateStateOnMouseLeftButtonDown(pointerPressedEventArgs, columnIndex, slot, allowEdit, shift, ctrl);
		}))
		{
			return true;
		}
		bool flag2;
		try
		{
			_noSelectionChangeCount++;
			flag2 = allowEdit && CurrentSlot == slot && columnIndex != -1 && (flag || CurrentColumnIndex == columnIndex) && !GetColumnEffectiveReadOnlyState(ColumnsItemsInternal[columnIndex]);
			UpdateSelectionAndCurrency(action: (SelectionMode == DataGridSelectionMode.Extended && shift) ? DataGridSelectionAction.SelectFromAnchorToCurrent : (GetRowSelection(slot) ? ((!ctrl && SelectionMode == DataGridSelectionMode.Extended && _selectedItems.Count != 0) ? DataGridSelectionAction.SelectCurrent : ((!ctrl || EditingRow != null) ? DataGridSelectionAction.None : DataGridSelectionAction.RemoveCurrentFromSelection)) : ((SelectionMode == DataGridSelectionMode.Single || !ctrl) ? DataGridSelectionAction.SelectCurrent : DataGridSelectionAction.AddCurrentToSelection)), columnIndex: columnIndex, slot: slot, scrollIntoView: false);
		}
		finally
		{
			NoSelectionChangeCount--;
		}
		if (_successfullyUpdatedSelection && flag2 && BeginCellEdit(pointerPressedEventArgs))
		{
			FocusEditingCell(setFocus: true);
		}
		return true;
	}

	public DataGridCollectionViewGroup GetGroupFromItem(object item, int groupLevel)
	{
		int num = DataConnection.IndexOf(item);
		if (num == -1)
		{
			return null;
		}
		int previousIndex = RowGroupHeadersTable.GetPreviousIndex(SlotFromRowIndex(num));
		DataGridRowGroupInfo valueAt = RowGroupHeadersTable.GetValueAt(previousIndex);
		while (valueAt != null && valueAt.Level != groupLevel)
		{
			previousIndex = RowGroupHeadersTable.GetPreviousIndex(valueAt.Slot);
			valueAt = RowGroupHeadersTable.GetValueAt(previousIndex);
		}
		return valueAt?.CollectionViewGroup;
	}

	protected virtual void OnLoadingRowGroup(DataGridRowGroupHeaderEventArgs e)
	{
		EventHandler<DataGridRowGroupHeaderEventArgs> eventHandler = this.LoadingRowGroup;
		if (eventHandler != null)
		{
			LoadingOrUnloadingRow = true;
			eventHandler(this, e);
			LoadingOrUnloadingRow = false;
		}
	}

	protected virtual void OnUnloadingRowGroup(DataGridRowGroupHeaderEventArgs e)
	{
		EventHandler<DataGridRowGroupHeaderEventArgs> eventHandler = this.UnloadingRowGroup;
		if (eventHandler != null)
		{
			LoadingOrUnloadingRow = true;
			eventHandler(this, e);
			LoadingOrUnloadingRow = false;
		}
	}

	private void ExpandRowGroupParentChain(int level, int slot)
	{
		if (level < 0)
		{
			return;
		}
		int previousIndex = RowGroupHeadersTable.GetPreviousIndex(slot + 1);
		DataGridRowGroupInfo dataGridRowGroupInfo = null;
		while (previousIndex >= 0)
		{
			dataGridRowGroupInfo = RowGroupHeadersTable.GetValueAt(previousIndex);
			if (level == dataGridRowGroupInfo.Level)
			{
				if (_collapsedSlotsTable.Contains(dataGridRowGroupInfo.Slot))
				{
					ExpandRowGroupParentChain(level - 1, dataGridRowGroupInfo.Slot - 1);
				}
				if (!dataGridRowGroupInfo.IsVisible)
				{
					EnsureRowGroupVisibility(dataGridRowGroupInfo, isVisible: true, setCurrent: false);
				}
				break;
			}
			previousIndex = RowGroupHeadersTable.GetPreviousIndex(previousIndex);
		}
	}

	protected virtual void OnCopyingRowClipboardContent(DataGridRowClipboardEventArgs e)
	{
		this.CopyingRowClipboardContent?.Invoke(this, e);
	}

	private string FormatClipboardContent(DataGridRowClipboardEventArgs e)
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire();
		List<DataGridClipboardCellContent> clipboardRowContent = e.ClipboardRowContent;
		int count = clipboardRowContent.Count;
		for (int i = 0; i < count; i++)
		{
			string value = (clipboardRowContent[i].Content?.ToString())?.Replace("\"", "\"\"");
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder2);
			handler.AppendLiteral("\"");
			handler.AppendFormatted(value);
			handler.AppendLiteral("\"");
			stringBuilder2.Append(ref handler);
			if (i < count - 1)
			{
				stringBuilder.Append('\t');
				continue;
			}
			stringBuilder.Append('\r');
			stringBuilder.Append('\n');
		}
		return StringBuilderCache.GetStringAndRelease(stringBuilder);
	}

	private bool ProcessCopyKey(KeyModifiers modifiers)
	{
		KeyboardHelper.GetMetaKeyState(this, modifiers, out var ctrlOrCmd, out var shift, out var alt);
		if (ctrlOrCmd && !shift && !alt && ClipboardCopyMode != 0 && SelectedItems.Count > 0)
		{
			StringBuilder stringBuilder = StringBuilderCache.Acquire();
			if (ClipboardCopyMode == DataGridClipboardCopyMode.IncludeHeader)
			{
				DataGridRowClipboardEventArgs dataGridRowClipboardEventArgs = new DataGridRowClipboardEventArgs(null, isColumnHeadersRow: true);
				foreach (DataGridColumn visibleColumn in ColumnsInternal.GetVisibleColumns())
				{
					dataGridRowClipboardEventArgs.ClipboardRowContent.Add(new DataGridClipboardCellContent(null, visibleColumn, visibleColumn.Header));
				}
				OnCopyingRowClipboardContent(dataGridRowClipboardEventArgs);
				stringBuilder.Append(FormatClipboardContent(dataGridRowClipboardEventArgs));
			}
			for (int i = 0; i < SelectedItems.Count; i++)
			{
				object item = SelectedItems[i];
				DataGridRowClipboardEventArgs dataGridRowClipboardEventArgs2 = new DataGridRowClipboardEventArgs(item, isColumnHeadersRow: false);
				foreach (DataGridColumn visibleColumn2 in ColumnsInternal.GetVisibleColumns())
				{
					object cellValue = visibleColumn2.GetCellValue(item, visibleColumn2.ClipboardContentBinding);
					dataGridRowClipboardEventArgs2.ClipboardRowContent.Add(new DataGridClipboardCellContent(item, visibleColumn2, cellValue));
				}
				OnCopyingRowClipboardContent(dataGridRowClipboardEventArgs2);
				stringBuilder.Append(FormatClipboardContent(dataGridRowClipboardEventArgs2));
			}
			string stringAndRelease = StringBuilderCache.GetStringAndRelease(stringBuilder);
			if (!string.IsNullOrEmpty(stringAndRelease))
			{
				CopyToClipboard(stringAndRelease);
				return true;
			}
		}
		return false;
	}

	private async void CopyToClipboard(string text)
	{
		IClipboard clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
		if (clipboard != null)
		{
			await clipboard.SetTextAsync(text);
		}
	}

	private void ResetValidationStatus()
	{
		if (EditingRow != null)
		{
			EditingRow.IsValid = true;
			if (EditingRow.Index != -1)
			{
				foreach (DataGridCell cell in EditingRow.Cells)
				{
					if (!cell.IsValid)
					{
						cell.IsValid = true;
						cell.UpdatePseudoClasses();
					}
				}
				EditingRow.UpdatePseudoClasses();
			}
		}
		IsValid = true;
		_validationSubscription?.Dispose();
		_validationSubscription = null;
	}

	protected virtual void OnAutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs e)
	{
		this.AutoGeneratingColumn?.Invoke(this, e);
	}

	protected virtual void OnColumnDisplayIndexChanged(DataGridColumnEventArgs e)
	{
		this.ColumnDisplayIndexChanged?.Invoke(this, e);
	}

	protected internal virtual void OnColumnReordered(DataGridColumnEventArgs e)
	{
		EnsureVerticalGridLines();
		this.ColumnReordered?.Invoke(this, e);
	}

	protected internal virtual void OnColumnReordering(DataGridColumnReorderingEventArgs e)
	{
		this.ColumnReordering?.Invoke(this, e);
	}

	protected internal virtual void OnColumnSorting(DataGridColumnEventArgs e)
	{
		this.Sorting?.Invoke(this, e);
	}

	internal double AdjustColumnWidths(int displayIndex, double amount, bool userInitiated)
	{
		if (!MathUtilities.IsZero(amount))
		{
			amount = ((!(amount < 0.0)) ? IncreaseColumnWidths(displayIndex, amount, userInitiated) : DecreaseColumnWidths(displayIndex, amount, userInitiated));
		}
		return amount;
	}

	internal void AutoSizeColumn(DataGridColumn column, double desiredWidth)
	{
		if (UsesStarSizing && !column.IsInitialDesiredWidthDetermined)
		{
			AutoSizingColumns = true;
		}
		if (desiredWidth > column.Width.DesiredValue || double.IsNaN(column.Width.DesiredValue))
		{
			if (UsesStarSizing && column.IsInitialDesiredWidthDetermined)
			{
				DataGridLength width = column.Width;
				column.Resize(width, new DataGridLength(column.Width.Value, column.Width.UnitType, desiredWidth, desiredWidth), userInitiated: false);
			}
			else
			{
				column.SetWidthInternalNoCallback(new DataGridLength(column.Width.Value, column.Width.UnitType, desiredWidth, desiredWidth));
				OnColumnWidthChanged(column);
			}
		}
	}

	internal bool ColumnRequiresRightGridLine(DataGridColumn dataGridColumn, bool includeLastRightGridLineWhenPresent)
	{
		if ((GridLinesVisibility == DataGridGridLinesVisibility.Vertical || GridLinesVisibility == DataGridGridLinesVisibility.All) && VerticalGridLinesBrush != null)
		{
			if (dataGridColumn == ColumnsInternal.LastVisibleColumn)
			{
				if (includeLastRightGridLineWhenPresent)
				{
					return ColumnsInternal.FillerColumn.IsActive;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	internal DataGridColumnCollection CreateColumnsInstance()
	{
		return new DataGridColumnCollection(this);
	}

	internal double DecreaseColumnWidths(int displayIndex, double amount, bool userInitiated)
	{
		amount = DecreaseNonStarColumnWidths(displayIndex, (DataGridColumn c) => c.Width.DesiredValue, amount, reverse: false, affectNewColumns: false);
		amount = AdjustStarColumnWidths(displayIndex, amount, userInitiated);
		amount = DecreaseNonStarColumnWidths(displayIndex, (DataGridColumn c) => c.ActualMinWidth, amount, reverse: true, affectNewColumns: false);
		amount = DecreaseNonStarColumnWidths(displayIndex, (DataGridColumn c) => c.ActualMinWidth, amount, reverse: true, affectNewColumns: true);
		return amount;
	}

	internal bool GetColumnReadOnlyState(DataGridColumn dataGridColumn, bool isReadOnly)
	{
		if (dataGridColumn is DataGridBoundColumn { Binding: BindingBase binding })
		{
			string text = (binding as Binding)?.Path ?? (binding as CompiledBindingExtension)?.Path.ToString();
			if (string.IsNullOrWhiteSpace(text))
			{
				return true;
			}
			return DataConnection.GetPropertyIsReadOnly(text) || isReadOnly;
		}
		return isReadOnly;
	}

	internal static double GetEdgedColumnWidth(DataGridColumn dataGridColumn)
	{
		return dataGridColumn.ActualWidth;
	}

	internal double IncreaseColumnWidths(int displayIndex, double amount, bool userInitiated)
	{
		amount = IncreaseNonStarColumnWidths(displayIndex, (DataGridColumn c) => c.Width.DesiredValue, amount, reverse: false, affectNewColumns: false);
		amount = AdjustStarColumnWidths(displayIndex, amount, userInitiated);
		amount = IncreaseNonStarColumnWidths(displayIndex, (DataGridColumn c) => c.ActualMaxWidth, amount, reverse: true, affectNewColumns: false);
		amount = IncreaseNonStarColumnWidths(displayIndex, (DataGridColumn c) => c.ActualMaxWidth, amount, reverse: true, affectNewColumns: false);
		return amount;
	}

	internal void OnClearingColumns()
	{
		ClearRows(recycle: false);
		RemoveDisplayedColumnHeaders();
		_horizontalOffset = (_negHorizontalOffset = 0.0);
		if (_hScrollBar != null && _hScrollBar.IsVisible)
		{
			_hScrollBar.Value = 0.0;
		}
	}

	internal void OnColumnCanUserResizeChanged(DataGridColumn column)
	{
		if (column.IsVisible)
		{
			EnsureHorizontalLayout();
		}
	}

	internal void OnColumnCollectionChanged_PostNotification(bool columnsGrew)
	{
		if (columnsGrew && CurrentColumnIndex == -1)
		{
			MakeFirstDisplayedCellCurrentCell();
		}
		if (_autoGeneratingColumnOperationCount == 0)
		{
			EnsureRowsPresenterVisibility();
			InvalidateRowHeightEstimate();
		}
	}

	internal void OnColumnCollectionChanged_PreNotification(bool columnsGrew)
	{
		if (columnsGrew && _autoGeneratingColumnOperationCount == 0 && ColumnsItemsInternal.Count == 1)
		{
			RefreshRows(recycleRows: false, clearRows: true);
		}
		else
		{
			InvalidateMeasure();
		}
	}

	internal void OnColumnDisplayIndexChanged(DataGridColumn dataGridColumn)
	{
		DataGridColumnEventArgs e = new DataGridColumnEventArgs(dataGridColumn);
		if (dataGridColumn != ColumnsInternal.RowGroupSpacerColumn)
		{
			OnColumnDisplayIndexChanged(e);
		}
	}

	internal void OnColumnDisplayIndexChanged_PostNotification()
	{
		FlushDisplayIndexChanged(raiseEvent: true);
		UpdateDisplayedColumns();
		CorrectColumnFrozenStates();
		EnsureHorizontalLayout();
	}

	internal void OnColumnDisplayIndexChanging(DataGridColumn targetColumn, int newDisplayIndex)
	{
		if (InDisplayIndexAdjustments)
		{
			throw DataGridError.DataGrid.CannotChangeColumnCollectionWhileAdjustingDisplayIndexes();
		}
		try
		{
			InDisplayIndexAdjustments = true;
			bool flag = targetColumn != ColumnsInternal.RowGroupSpacerColumn;
			if (newDisplayIndex < targetColumn.DisplayIndexWithFiller)
			{
				for (int i = newDisplayIndex; i < targetColumn.DisplayIndexWithFiller; i++)
				{
					DataGridColumn columnAtDisplayIndex = ColumnsInternal.GetColumnAtDisplayIndex(i);
					columnAtDisplayIndex.DisplayIndexWithFiller++;
					if (flag)
					{
						columnAtDisplayIndex.DisplayIndexHasChanged = true;
					}
				}
			}
			else
			{
				for (int num = newDisplayIndex; num > targetColumn.DisplayIndexWithFiller; num--)
				{
					DataGridColumn columnAtDisplayIndex = ColumnsInternal.GetColumnAtDisplayIndex(num);
					columnAtDisplayIndex.DisplayIndexWithFiller--;
					if (flag)
					{
						columnAtDisplayIndex.DisplayIndexHasChanged = true;
					}
				}
			}
			if (targetColumn.DisplayIndexWithFiller != -1)
			{
				ColumnsInternal.DisplayIndexMap.Remove(targetColumn.Index);
			}
			ColumnsInternal.DisplayIndexMap.Insert(newDisplayIndex, targetColumn.Index);
		}
		finally
		{
			InDisplayIndexAdjustments = false;
		}
	}

	internal void OnColumnBindingChanged(DataGridBoundColumn column)
	{
		if (_rowsPresenter == null)
		{
			return;
		}
		foreach (DataGridRow allRow in GetAllRows())
		{
			PopulateCellContent(isCellEdited: false, column, allRow, allRow.Cells[column.Index]);
		}
	}

	internal void OnColumnMaxWidthChanged(DataGridColumn column, double oldValue)
	{
		if (column.IsVisible && oldValue != column.ActualMaxWidth)
		{
			DataGridLength oldWidth = new DataGridLength(oldValue, column.Width.UnitType, column.Width.DesiredValue, column.Width.DesiredValue);
			if (column.ActualMaxWidth < column.Width.DisplayValue)
			{
				AdjustColumnWidths(column.DisplayIndex + 1, column.Width.DisplayValue - column.ActualMaxWidth, userInitiated: false);
				column.SetWidthDisplayValue(column.ActualMaxWidth);
			}
			else if (column.Width.DisplayValue == oldValue && column.Width.DesiredValue > column.Width.DisplayValue)
			{
				column.Resize(oldWidth, new DataGridLength(column.Width.Value, column.Width.UnitType, column.Width.DesiredValue, column.Width.DesiredValue), userInitiated: false);
			}
			OnColumnWidthChanged(column);
		}
	}

	internal void OnColumnMinWidthChanged(DataGridColumn column, double oldValue)
	{
		if (column.IsVisible && oldValue != column.ActualMinWidth)
		{
			DataGridLength oldWidth = new DataGridLength(oldValue, column.Width.UnitType, column.Width.DesiredValue, column.Width.DesiredValue);
			if (column.ActualMinWidth > column.Width.DisplayValue)
			{
				AdjustColumnWidths(column.DisplayIndex + 1, column.Width.DisplayValue - column.ActualMinWidth, userInitiated: false);
				column.SetWidthDisplayValue(column.ActualMinWidth);
			}
			else if (column.Width.DisplayValue == oldValue && column.Width.DesiredValue < column.Width.DisplayValue)
			{
				column.Resize(oldWidth, new DataGridLength(column.Width.Value, column.Width.UnitType, column.Width.DesiredValue, column.Width.DesiredValue), userInitiated: false);
			}
			OnColumnWidthChanged(column);
		}
	}

	internal void OnColumnReadOnlyStateChanging(DataGridColumn dataGridColumn, bool isReadOnly)
	{
		if (isReadOnly && CurrentColumnIndex == dataGridColumn.Index && !EndCellEdit(DataGridEditAction.Commit, exitEditingMode: true, ContainsFocus, raiseEvents: true))
		{
			EndCellEdit(DataGridEditAction.Cancel, exitEditingMode: true, ContainsFocus, raiseEvents: false);
		}
	}

	internal void OnColumnVisibleStateChanged(DataGridColumn updatedColumn)
	{
		CorrectColumnFrozenStates();
		UpdateDisplayedColumns();
		EnsureRowsPresenterVisibility();
		EnsureHorizontalLayout();
		InvalidateColumnHeadersMeasure();
		if (updatedColumn.IsVisible && ColumnsInternal.VisibleColumnCount == 1 && CurrentColumnIndex == -1)
		{
			if (SelectedIndex != -1)
			{
				SetAndSelectCurrentCell(updatedColumn.Index, SelectedIndex, forceCurrentCellSelection: true);
			}
			else
			{
				MakeFirstDisplayedCellCurrentCell();
			}
		}
		ColumnHeaders?.InvalidateChildIndex();
		foreach (DataGridRow allRow in GetAllRows())
		{
			allRow.Cells[updatedColumn.Index].IsVisible = updatedColumn.IsVisible;
			allRow.InvalidateCellsIndex();
		}
	}

	internal void OnColumnVisibleStateChanging(DataGridColumn targetColumn)
	{
		if (targetColumn.IsVisible && CurrentColumn == targetColumn)
		{
			DataGridColumn dataGridColumn = ColumnsInternal.GetNextVisibleColumn(targetColumn);
			if (dataGridColumn == null)
			{
				dataGridColumn = ColumnsInternal.GetPreviousVisibleNonFillerColumn(targetColumn);
			}
			if (dataGridColumn == null)
			{
				SetCurrentCellCore(-1, -1);
			}
			else
			{
				SetCurrentCellCore(dataGridColumn.Index, CurrentSlot);
			}
		}
	}

	internal void OnColumnWidthChanged(DataGridColumn updatedColumn)
	{
		if (updatedColumn.IsVisible)
		{
			EnsureHorizontalLayout();
		}
	}

	internal void OnFillerColumnWidthNeeded(double finalWidth)
	{
		DataGridFillerColumn fillerColumn = ColumnsInternal.FillerColumn;
		double visibleEdgedColumnsWidth = ColumnsInternal.VisibleEdgedColumnsWidth;
		if (finalWidth - visibleEdgedColumnsWidth > LayoutHelper.LayoutEpsilon)
		{
			fillerColumn.FillerWidth = finalWidth - visibleEdgedColumnsWidth;
		}
		else
		{
			fillerColumn.FillerWidth = 0.0;
		}
	}

	internal void OnInsertedColumn_PostNotification(DataGridCellCoordinates newCurrentCellCoordinates, int newDisplayIndex)
	{
		if (newCurrentCellCoordinates.ColumnIndex != -1)
		{
			SetAndSelectCurrentCell(newCurrentCellCoordinates.ColumnIndex, newCurrentCellCoordinates.Slot, ColumnsInternal.VisibleColumnCount == 1);
			if (newDisplayIndex < FrozenColumnCountWithFiller)
			{
				CorrectColumnFrozenStates();
			}
		}
	}

	internal void OnInsertedColumn_PreNotification(DataGridColumn insertedColumn)
	{
		CorrectColumnIndexesAfterInsertion(insertedColumn, 1);
		CorrectColumnDisplayIndexesAfterInsertion(insertedColumn);
		InsertDisplayedColumnHeader(insertedColumn);
		if (SlotCount > 0)
		{
			int count = ColumnsItemsInternal.Count;
			foreach (DataGridRow allRow in GetAllRows())
			{
				if (allRow.Cells.Count < count)
				{
					AddNewCellPrivate(allRow, insertedColumn);
				}
			}
		}
		if (insertedColumn.IsVisible)
		{
			EnsureHorizontalLayout();
		}
		if (insertedColumn is DataGridBoundColumn { IsAutoGenerated: false } dataGridBoundColumn)
		{
			dataGridBoundColumn.SetHeaderFromBinding();
		}
	}

	internal DataGridCellCoordinates OnInsertingColumn(int columnIndexInserted, DataGridColumn insertColumn)
	{
		if (insertColumn.OwningGrid != null && insertColumn != ColumnsInternal.RowGroupSpacerColumn)
		{
			throw DataGridError.DataGrid.ColumnCannotBeReassignedToDifferentDataGrid();
		}
		DataGridCellCoordinates result;
		if (CurrentColumnIndex != -1)
		{
			_temporarilyResetCurrentCell = true;
			result = new DataGridCellCoordinates((columnIndexInserted <= CurrentColumnIndex) ? (CurrentColumnIndex + 1) : CurrentColumnIndex, CurrentSlot);
			ResetCurrentCellCore();
		}
		else
		{
			result = new DataGridCellCoordinates(-1, -1);
		}
		return result;
	}

	internal void OnRemovedColumn_PostNotification(DataGridCellCoordinates newCurrentCellCoordinates)
	{
		if (newCurrentCellCoordinates.ColumnIndex != -1)
		{
			SetAndSelectCurrentCell(newCurrentCellCoordinates.ColumnIndex, newCurrentCellCoordinates.Slot, forceCurrentCellSelection: false);
		}
	}

	internal void OnRemovedColumn_PreNotification(DataGridColumn removedColumn)
	{
		CorrectColumnIndexesAfterDeletion(removedColumn);
		CorrectColumnDisplayIndexesAfterDeletion(removedColumn);
		if (removedColumn.IsFrozen)
		{
			removedColumn.IsFrozen = false;
			CorrectColumnFrozenStates();
		}
		UpdateDisplayedColumns();
		int count = ColumnsItemsInternal.Count;
		if (_rowsPresenter != null)
		{
			foreach (DataGridRow allRow in GetAllRows())
			{
				if (allRow.Cells.Count > count)
				{
					allRow.Cells.RemoveAt(removedColumn.Index);
				}
			}
			_rowsPresenter.InvalidateArrange();
		}
		RemoveDisplayedColumnHeader(removedColumn);
	}

	internal DataGridCellCoordinates OnRemovingColumn(DataGridColumn dataGridColumn)
	{
		_temporarilyResetCurrentCell = false;
		int index = dataGridColumn.Index;
		DataGridCellCoordinates result;
		if (CurrentColumnIndex != -1)
		{
			int num = CurrentColumnIndex;
			if (index == num)
			{
				DataGridColumn nextVisibleColumn = ColumnsInternal.GetNextVisibleColumn(ColumnsItemsInternal[index]);
				if (nextVisibleColumn != null)
				{
					num = ((nextVisibleColumn.Index <= index) ? nextVisibleColumn.Index : (nextVisibleColumn.Index - 1));
				}
				else
				{
					DataGridColumn previousVisibleNonFillerColumn = ColumnsInternal.GetPreviousVisibleNonFillerColumn(ColumnsItemsInternal[index]);
					num = ((previousVisibleNonFillerColumn == null) ? (-1) : ((previousVisibleNonFillerColumn.Index <= index) ? previousVisibleNonFillerColumn.Index : (previousVisibleNonFillerColumn.Index - 1)));
				}
			}
			else if (index < num)
			{
				num--;
			}
			result = new DataGridCellCoordinates(num, (num == -1) ? (-1) : CurrentSlot);
			if (index == CurrentColumnIndex)
			{
				if (!CommitEdit(DataGridEditingUnit.Row, exitEditingMode: false))
				{
					CancelEdit(DataGridEditingUnit.Row, raiseEvents: false);
				}
				SetCurrentCellCore(-1, -1);
			}
			else
			{
				_temporarilyResetCurrentCell = true;
				SetCurrentCellCore(-1, -1);
			}
		}
		else
		{
			result = new DataGridCellCoordinates(-1, -1);
		}
		if (ColumnsItemsInternal.Count == 1)
		{
			ClearRows(recycle: false);
		}
		if (dataGridColumn.IsVisible && !dataGridColumn.IsFrozen && DisplayData.FirstDisplayedScrollingCol >= 0)
		{
			if (DisplayData.FirstDisplayedScrollingCol == dataGridColumn.Index)
			{
				_horizontalOffset -= _negHorizontalOffset;
				_negHorizontalOffset = 0.0;
			}
			else if (!ColumnsInternal.DisplayInOrder(DisplayData.FirstDisplayedScrollingCol, dataGridColumn.Index))
			{
				_horizontalOffset -= GetEdgedColumnWidth(dataGridColumn);
			}
			if (_hScrollBar != null && _hScrollBar.IsVisible)
			{
				_hScrollBar.Value = _horizontalOffset;
			}
		}
		return result;
	}

	internal void RefreshColumnElements(DataGridColumn dataGridColumn, string propertyName)
	{
		for (int i = 0; i < _loadedRows.Count; i++)
		{
			DataGridRow dataGridRow = _loadedRows[i];
			if (!IsSlotVisible(dataGridRow.Slot))
			{
				RefreshCellElement(dataGridColumn, dataGridRow, propertyName);
			}
		}
		if (_rowsPresenter == null)
		{
			return;
		}
		foreach (DataGridRow allRow in GetAllRows())
		{
			RefreshCellElement(dataGridColumn, allRow, propertyName);
		}
		InvalidateRowHeightEstimate();
		InvalidateMeasure();
	}

	private double AdjustStarColumnWidths(int displayIndex, double adjustment, bool userInitiated)
	{
		double num = adjustment;
		if (MathUtilities.IsZero(num))
		{
			return num;
		}
		bool increase = num > 0.0;
		bool flag = false;
		double num2 = 0.0;
		double num3 = 0.0;
		double num4 = 0.0;
		List<DataGridColumn> list = new List<DataGridColumn>();
		foreach (DataGridColumn displayedColumn in ColumnsInternal.GetDisplayedColumns((DataGridColumn c) => c.Width.IsStar && c.IsVisible && (c.ActualCanUserResize || !userInitiated)))
		{
			if (displayedColumn.DisplayIndex < displayIndex)
			{
				flag = true;
				continue;
			}
			list.Add(displayedColumn);
			num4 += displayedColumn.Width.Value;
			num2 += displayedColumn.Width.DisplayValue;
			num3 += (increase ? displayedColumn.ActualMaxWidth : displayedColumn.ActualMinWidth);
		}
		double val = num3 - num2;
		val = (increase ? Math.Min(val, adjustment) : Math.Max(val, adjustment));
		foreach (DataGridColumn item in list)
		{
			item.SetWidthDesiredValue((num2 + val) * item.Width.Value / num4);
		}
		num = AdjustStarColumnWidths(displayIndex, num, userInitiated, (DataGridColumn c) => c.Width.DesiredValue);
		num = AdjustStarColumnWidths(displayIndex, num, userInitiated, (DataGridColumn c) => (!increase) ? c.ActualMinWidth : c.ActualMaxWidth);
		if (flag)
		{
			double num5 = (num2 + adjustment - num) / num2;
			foreach (DataGridColumn item2 in list)
			{
				item2.SetWidthStarValue(Math.Min(double.MaxValue, num5 * item2.Width.Value));
			}
		}
		return num;
	}

	private double AdjustStarColumnWidths(int displayIndex, double remainingAdjustment, bool userInitiated, Func<DataGridColumn, double> targetWidth)
	{
		if (MathUtilities.IsZero(remainingAdjustment))
		{
			return remainingAdjustment;
		}
		bool flag = remainingAdjustment > 0.0;
		double num = 0.0;
		double num2 = 0.0;
		List<KeyValuePair<DataGridColumn, double>> list = new List<KeyValuePair<DataGridColumn, double>>();
		foreach (DataGridColumn displayedColumn in ColumnsInternal.GetDisplayedColumns((DataGridColumn c) => c.Width.IsStar && c.DisplayIndex >= displayIndex && c.IsVisible && c.Width.Value > 0.0 && (c.ActualCanUserResize || !userInitiated)))
		{
			int num3 = 0;
			double val = Math.Min(displayedColumn.ActualMaxWidth, Math.Max(targetWidth(displayedColumn), displayedColumn.ActualMinWidth)) - displayedColumn.Width.DisplayValue;
			double num4 = (flag ? Math.Max(0.0, val) : Math.Min(0.0, val)) / displayedColumn.Width.Value;
			foreach (KeyValuePair<DataGridColumn, double> item in list)
			{
				if (flag ? (num4 <= item.Value) : (num4 >= item.Value))
				{
					break;
				}
				num3++;
			}
			list.Insert(num3, new KeyValuePair<DataGridColumn, double>(displayedColumn, num4));
			num += displayedColumn.Width.Value;
			num2 += displayedColumn.Width.DisplayValue;
		}
		foreach (KeyValuePair<DataGridColumn, double> item2 in list)
		{
			double val2 = item2.Value * item2.Key.Width.Value;
			double val3 = item2.Key.Width.Value * remainingAdjustment / num;
			double num5 = (flag ? Math.Min(val2, val3) : Math.Max(val2, val3));
			remainingAdjustment -= num5;
			num -= item2.Key.Width.Value;
			item2.Key.SetWidthDisplayValue(Math.Max(0.001, item2.Key.Width.DisplayValue + num5));
		}
		return remainingAdjustment;
	}

	private bool ComputeDisplayedColumns()
	{
		bool result = false;
		int num = 0;
		int num2 = 0;
		double cellsWidth = CellsWidth;
		double num3 = 0.0;
		int num4 = -1;
		int num5 = DisplayData.FirstDisplayedScrollingCol;
		if (cellsWidth <= 0.0 || ColumnsInternal.VisibleColumnCount == 0)
		{
			DisplayData.FirstDisplayedScrollingCol = -1;
			DisplayData.LastTotallyDisplayedScrollingCol = -1;
			return result;
		}
		foreach (DataGridColumn visibleFrozenColumn in ColumnsInternal.GetVisibleFrozenColumns())
		{
			if (num4 == -1)
			{
				num4 = visibleFrozenColumn.Index;
			}
			num3 += GetEdgedColumnWidth(visibleFrozenColumn);
			if (num3 >= cellsWidth)
			{
				break;
			}
		}
		if (num3 < cellsWidth && num5 >= 0)
		{
			DataGridColumn dataGridColumn = ColumnsItemsInternal[num5];
			if (dataGridColumn.IsFrozen)
			{
				dataGridColumn = ColumnsInternal.FirstVisibleScrollingColumn;
				_negHorizontalOffset = 0.0;
				if (dataGridColumn == null)
				{
					DataGridDisplayData displayData = DisplayData;
					int firstDisplayedScrollingCol = (DisplayData.LastTotallyDisplayedScrollingCol = -1);
					displayData.FirstDisplayedScrollingCol = firstDisplayedScrollingCol;
					return result;
				}
				num5 = dataGridColumn.Index;
			}
			num3 -= _negHorizontalOffset;
			while (num3 < cellsWidth && dataGridColumn != null)
			{
				num3 += GetEdgedColumnWidth(dataGridColumn);
				num2++;
				dataGridColumn = ColumnsInternal.GetNextVisibleColumn(dataGridColumn);
			}
			num = num2;
			if (num3 < cellsWidth)
			{
				if (_negHorizontalOffset > 0.0)
				{
					result = true;
					if (cellsWidth - num3 > _negHorizontalOffset)
					{
						num3 += _negHorizontalOffset;
						_horizontalOffset -= _negHorizontalOffset;
						if (_horizontalOffset < LayoutHelper.LayoutEpsilon)
						{
							_horizontalOffset = 0.0;
						}
						_negHorizontalOffset = 0.0;
					}
					else
					{
						_horizontalOffset -= cellsWidth - num3;
						_negHorizontalOffset -= cellsWidth - num3;
						num3 = cellsWidth;
					}
					HorizontalAdjustment = Math.Min(HorizontalAdjustment, _horizontalOffset);
				}
				if (num3 < cellsWidth && _horizontalOffset > 0.0)
				{
					dataGridColumn = ColumnsInternal.GetPreviousVisibleScrollingColumn(ColumnsItemsInternal[num5]);
					while (dataGridColumn != null && num3 + GetEdgedColumnWidth(dataGridColumn) <= cellsWidth)
					{
						num3 += GetEdgedColumnWidth(dataGridColumn);
						num2++;
						result = true;
						num5 = dataGridColumn.Index;
						_horizontalOffset -= GetEdgedColumnWidth(dataGridColumn);
						dataGridColumn = ColumnsInternal.GetPreviousVisibleScrollingColumn(dataGridColumn);
					}
				}
				if (num3 < cellsWidth && _horizontalOffset > 0.0)
				{
					dataGridColumn = ColumnsInternal.GetPreviousVisibleScrollingColumn(ColumnsItemsInternal[num5]);
					num5 = dataGridColumn.Index;
					_negHorizontalOffset = GetEdgedColumnWidth(dataGridColumn) - cellsWidth + num3;
					_horizontalOffset -= cellsWidth - num3;
					num2++;
					result = true;
					num3 = cellsWidth;
				}
				num = num2;
			}
			int num7 = num - 1;
			if (num3 > cellsWidth)
			{
				num7--;
			}
			if (num7 < 0)
			{
				DisplayData.LastTotallyDisplayedScrollingCol = -1;
			}
			else
			{
				dataGridColumn = ColumnsItemsInternal[num5];
				for (int i = 0; i < num7; i++)
				{
					dataGridColumn = ColumnsInternal.GetNextVisibleColumn(dataGridColumn);
				}
				DisplayData.LastTotallyDisplayedScrollingCol = dataGridColumn.Index;
			}
		}
		else
		{
			DisplayData.LastTotallyDisplayedScrollingCol = -1;
		}
		DisplayData.FirstDisplayedScrollingCol = num5;
		return result;
	}

	private int ComputeFirstVisibleScrollingColumn()
	{
		if (ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth() >= CellsWidth)
		{
			_negHorizontalOffset = 0.0;
			return -1;
		}
		DataGridColumn dataGridColumn = ColumnsInternal.FirstVisibleScrollingColumn;
		if (_horizontalOffset == 0.0)
		{
			_negHorizontalOffset = 0.0;
			return dataGridColumn?.Index ?? (-1);
		}
		double num = 0.0;
		while (dataGridColumn != null)
		{
			num += GetEdgedColumnWidth(dataGridColumn);
			if (num > _horizontalOffset)
			{
				break;
			}
			dataGridColumn = ColumnsInternal.GetNextVisibleColumn(dataGridColumn);
		}
		if (dataGridColumn == null)
		{
			dataGridColumn = ColumnsInternal.FirstVisibleScrollingColumn;
			if (dataGridColumn == null)
			{
				_negHorizontalOffset = 0.0;
				return -1;
			}
			if (_negHorizontalOffset != _horizontalOffset)
			{
				_negHorizontalOffset = 0.0;
			}
			return dataGridColumn.Index;
		}
		_negHorizontalOffset = GetEdgedColumnWidth(dataGridColumn) - (num - _horizontalOffset);
		return dataGridColumn.Index;
	}

	private void CorrectColumnDisplayIndexesAfterDeletion(DataGridColumn deletedColumn)
	{
		try
		{
			InDisplayIndexAdjustments = true;
			ColumnsInternal.DisplayIndexMap.RemoveAt(deletedColumn.DisplayIndexWithFiller);
			for (int i = 0; i < ColumnsInternal.DisplayIndexMap.Count; i++)
			{
				if (ColumnsInternal.DisplayIndexMap[i] > deletedColumn.Index)
				{
					ColumnsInternal.DisplayIndexMap[i]--;
				}
				if (i >= deletedColumn.DisplayIndexWithFiller)
				{
					DataGridColumn columnAtDisplayIndex = ColumnsInternal.GetColumnAtDisplayIndex(i);
					columnAtDisplayIndex.DisplayIndexWithFiller--;
					columnAtDisplayIndex.DisplayIndexHasChanged = true;
				}
			}
			FlushDisplayIndexChanged(raiseEvent: true);
		}
		finally
		{
			InDisplayIndexAdjustments = false;
			FlushDisplayIndexChanged(raiseEvent: false);
		}
	}

	private void CorrectColumnDisplayIndexesAfterInsertion(DataGridColumn insertedColumn)
	{
		if (insertedColumn.DisplayIndexWithFiller == -1 || insertedColumn.DisplayIndexWithFiller >= ColumnsItemsInternal.Count)
		{
			insertedColumn.DisplayIndexWithFiller = insertedColumn.Index;
		}
		try
		{
			InDisplayIndexAdjustments = true;
			for (int i = 0; i < ColumnsInternal.DisplayIndexMap.Count; i++)
			{
				if (ColumnsInternal.DisplayIndexMap[i] >= insertedColumn.Index)
				{
					ColumnsInternal.DisplayIndexMap[i]++;
				}
				if (i >= insertedColumn.DisplayIndexWithFiller)
				{
					DataGridColumn columnAtDisplayIndex = ColumnsInternal.GetColumnAtDisplayIndex(i);
					columnAtDisplayIndex.DisplayIndexWithFiller++;
					columnAtDisplayIndex.DisplayIndexHasChanged = true;
				}
			}
			ColumnsInternal.DisplayIndexMap.Insert(insertedColumn.DisplayIndexWithFiller, insertedColumn.Index);
			FlushDisplayIndexChanged(raiseEvent: true);
		}
		finally
		{
			InDisplayIndexAdjustments = false;
			FlushDisplayIndexChanged(raiseEvent: false);
		}
	}

	private void CorrectColumnFrozenStates()
	{
		int num = 0;
		double num2 = 0.0;
		double num3 = 0.0;
		foreach (DataGridColumn displayedColumn in ColumnsInternal.GetDisplayedColumns())
		{
			if (displayedColumn.IsFrozen)
			{
				num3 += displayedColumn.ActualWidth;
			}
			displayedColumn.IsFrozen = num < FrozenColumnCountWithFiller;
			if (displayedColumn.IsFrozen)
			{
				num2 += displayedColumn.ActualWidth;
			}
			num++;
		}
		if (HorizontalOffset > Math.Max(0.0, num2 - num3))
		{
			UpdateHorizontalOffset(HorizontalOffset - num2 + num3);
		}
		else
		{
			UpdateHorizontalOffset(0.0);
		}
	}

	private void CorrectColumnIndexesAfterDeletion(DataGridColumn deletedColumn)
	{
		for (int i = deletedColumn.Index; i < ColumnsItemsInternal.Count; i++)
		{
			ColumnsItemsInternal[i].Index = ColumnsItemsInternal[i].Index - 1;
		}
	}

	private void CorrectColumnIndexesAfterInsertion(DataGridColumn insertedColumn, int insertionCount)
	{
		for (int i = insertedColumn.Index + insertionCount; i < ColumnsItemsInternal.Count; i++)
		{
			ColumnsItemsInternal[i].Index = i;
		}
	}

	private static double DecreaseNonStarColumnWidth(DataGridColumn column, double targetWidth, double amount)
	{
		if (MathUtilities.GreaterThanOrClose(targetWidth, column.Width.DisplayValue))
		{
			return amount;
		}
		double num = Math.Max(column.ActualMinWidth - column.Width.DisplayValue, Math.Max(targetWidth - column.Width.DisplayValue, amount));
		column.SetWidthDisplayValue(column.Width.DisplayValue + num);
		return amount - num;
	}

	private double DecreaseNonStarColumnWidths(int displayIndex, Func<DataGridColumn, double> targetWidth, double amount, bool reverse, bool affectNewColumns)
	{
		if (MathUtilities.GreaterThanOrClose(amount, 0.0))
		{
			return amount;
		}
		foreach (DataGridColumn displayedColumn in ColumnsInternal.GetDisplayedColumns(reverse, (DataGridColumn column) => column.IsVisible && column.Width.UnitType != DataGridLengthUnitType.Star && column.DisplayIndex >= displayIndex && column.ActualCanUserResize && (affectNewColumns || column.IsInitialDesiredWidthDetermined)))
		{
			amount = DecreaseNonStarColumnWidth(displayedColumn, Math.Max(displayedColumn.ActualMinWidth, targetWidth(displayedColumn)), amount);
			if (MathUtilities.IsZero(amount))
			{
				break;
			}
		}
		return amount;
	}

	private void FlushDisplayIndexChanged(bool raiseEvent)
	{
		foreach (DataGridColumn item in ColumnsItemsInternal)
		{
			if (item.DisplayIndexHasChanged)
			{
				item.DisplayIndexHasChanged = false;
				if (raiseEvent)
				{
					OnColumnDisplayIndexChanged(item);
				}
			}
		}
	}

	private bool GetColumnEffectiveReadOnlyState(DataGridColumn dataGridColumn)
	{
		if (!IsReadOnly && !dataGridColumn.IsReadOnly)
		{
			return dataGridColumn is DataGridFillerColumn;
		}
		return true;
	}

	private double GetColumnXFromIndex(int index)
	{
		double num = 0.0;
		foreach (DataGridColumn visibleColumn in ColumnsInternal.GetVisibleColumns())
		{
			if (index == visibleColumn.Index)
			{
				break;
			}
			num += GetEdgedColumnWidth(visibleColumn);
		}
		return num;
	}

	private double GetNegHorizontalOffsetFromHorizontalOffset(double horizontalOffset)
	{
		foreach (DataGridColumn visibleScrollingColumn in ColumnsInternal.GetVisibleScrollingColumns())
		{
			if (GetEdgedColumnWidth(visibleScrollingColumn) > horizontalOffset)
			{
				break;
			}
			horizontalOffset -= GetEdgedColumnWidth(visibleScrollingColumn);
		}
		return horizontalOffset;
	}

	private static double IncreaseNonStarColumnWidth(DataGridColumn column, double targetWidth, double amount)
	{
		if (targetWidth <= column.Width.DisplayValue)
		{
			return amount;
		}
		double num = Math.Min(column.ActualMaxWidth - column.Width.DisplayValue, Math.Min(targetWidth - column.Width.DisplayValue, amount));
		column.SetWidthDisplayValue(column.Width.DisplayValue + num);
		return amount - num;
	}

	private double IncreaseNonStarColumnWidths(int displayIndex, Func<DataGridColumn, double> targetWidth, double amount, bool reverse, bool affectNewColumns)
	{
		if (MathUtilities.LessThanOrClose(amount, 0.0))
		{
			return amount;
		}
		foreach (DataGridColumn displayedColumn in ColumnsInternal.GetDisplayedColumns(reverse, (DataGridColumn column) => column.IsVisible && column.Width.UnitType != DataGridLengthUnitType.Star && column.DisplayIndex >= displayIndex && column.ActualCanUserResize && (affectNewColumns || column.IsInitialDesiredWidthDetermined)))
		{
			amount = IncreaseNonStarColumnWidth(displayedColumn, Math.Min(displayedColumn.ActualMaxWidth, targetWidth(displayedColumn)), amount);
			if (MathUtilities.IsZero(amount))
			{
				break;
			}
		}
		return amount;
	}

	private void InsertDisplayedColumnHeader(DataGridColumn dataGridColumn)
	{
		if (_columnHeadersPresenter != null)
		{
			dataGridColumn.HeaderCell.IsVisible = dataGridColumn.IsVisible;
			_columnHeadersPresenter.Children.Insert(dataGridColumn.DisplayIndexWithFiller, dataGridColumn.HeaderCell);
		}
	}

	private static void RefreshCellElement(DataGridColumn dataGridColumn, DataGridRow dataGridRow, string propertyName)
	{
		if (dataGridRow.Cells[dataGridColumn.Index].Content is Control element)
		{
			dataGridColumn.RefreshCellContent(element, propertyName);
		}
	}

	private void RemoveAutoGeneratedColumns()
	{
		int i = 0;
		_autoGeneratingColumnOperationCount++;
		try
		{
			while (i < ColumnsInternal.Count)
			{
				for (; i < ColumnsInternal.Count && !ColumnsInternal[i].IsAutoGenerated; i++)
				{
				}
				while (i < ColumnsInternal.Count && ColumnsInternal[i].IsAutoGenerated)
				{
					ColumnsInternal.RemoveAt(i);
				}
			}
			ColumnsInternal.AutogeneratedColumnCount = 0;
		}
		finally
		{
			_autoGeneratingColumnOperationCount--;
		}
	}

	private bool ScrollColumnIntoView(int columnIndex)
	{
		if (DisplayData.FirstDisplayedScrollingCol != -1 && !ColumnsItemsInternal[columnIndex].IsFrozen && (columnIndex != DisplayData.FirstDisplayedScrollingCol || _negHorizontalOffset > 0.0))
		{
			if (ColumnsInternal.DisplayInOrder(columnIndex, DisplayData.FirstDisplayedScrollingCol))
			{
				int num = ColumnsInternal.GetColumnCount(isVisible: true, isFrozen: false, columnIndex, DisplayData.FirstDisplayedScrollingCol);
				if (_negHorizontalOffset > 0.0)
				{
					num++;
				}
				ScrollColumns(-num);
			}
			else if (columnIndex == DisplayData.FirstDisplayedScrollingCol && _negHorizontalOffset > 0.0)
			{
				ScrollColumns(-1);
			}
			else if (DisplayData.LastTotallyDisplayedScrollingCol == -1 || (DisplayData.LastTotallyDisplayedScrollingCol != columnIndex && ColumnsInternal.DisplayInOrder(DisplayData.LastTotallyDisplayedScrollingCol, columnIndex)))
			{
				double columnXFromIndex = GetColumnXFromIndex(columnIndex);
				double num2 = columnXFromIndex + GetEdgedColumnWidth(ColumnsItemsInternal[columnIndex]) - HorizontalOffset - CellsWidth;
				double num3 = num2;
				DataGridColumn dataGridColumn = ColumnsItemsInternal[DisplayData.FirstDisplayedScrollingCol];
				DataGridColumn nextVisibleColumn = ColumnsInternal.GetNextVisibleColumn(dataGridColumn);
				double num4 = GetEdgedColumnWidth(dataGridColumn) - _negHorizontalOffset;
				while (nextVisibleColumn != null && num3 >= num4)
				{
					num3 -= num4;
					dataGridColumn = nextVisibleColumn;
					num4 = GetEdgedColumnWidth(dataGridColumn);
					nextVisibleColumn = ColumnsInternal.GetNextVisibleColumn(dataGridColumn);
					_negHorizontalOffset = 0.0;
				}
				_negHorizontalOffset += num3;
				DisplayData.LastTotallyDisplayedScrollingCol = columnIndex;
				if (dataGridColumn.Index == columnIndex)
				{
					_negHorizontalOffset = 0.0;
					double visibleFrozenEdgedColumnsWidth = ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth();
					if (num4 > CellsWidth - visibleFrozenEdgedColumnsWidth)
					{
						DisplayData.LastTotallyDisplayedScrollingCol = -1;
						num2 = columnXFromIndex - HorizontalOffset - visibleFrozenEdgedColumnsWidth;
					}
				}
				DisplayData.FirstDisplayedScrollingCol = dataGridColumn.Index;
				if (num2 != 0.0)
				{
					UpdateHorizontalOffset(HorizontalOffset + num2);
				}
			}
		}
		return true;
	}

	private void ScrollColumns(int columns)
	{
		DataGridColumn dataGridColumn = null;
		int i = 0;
		if (columns > 0)
		{
			DataGridColumn dataGridColumn2;
			if (DisplayData.LastTotallyDisplayedScrollingCol >= 0)
			{
				dataGridColumn2 = ColumnsItemsInternal[DisplayData.LastTotallyDisplayedScrollingCol];
				for (; i < columns; i++)
				{
					if (dataGridColumn2 == null)
					{
						break;
					}
					dataGridColumn2 = ColumnsInternal.GetNextVisibleColumn(dataGridColumn2);
				}
				if (dataGridColumn2 == null)
				{
					return;
				}
			}
			dataGridColumn2 = ColumnsItemsInternal[DisplayData.FirstDisplayedScrollingCol];
			for (i = 0; i < columns; i++)
			{
				if (dataGridColumn2 == null)
				{
					break;
				}
				dataGridColumn2 = ColumnsInternal.GetNextVisibleColumn(dataGridColumn2);
			}
			dataGridColumn = dataGridColumn2;
		}
		if (columns < 0)
		{
			DataGridColumn dataGridColumn2 = ColumnsItemsInternal[DisplayData.FirstDisplayedScrollingCol];
			if (_negHorizontalOffset > 0.0)
			{
				i++;
			}
			for (; i < -columns; i++)
			{
				if (dataGridColumn2 == null)
				{
					break;
				}
				dataGridColumn2 = ColumnsInternal.GetPreviousVisibleScrollingColumn(dataGridColumn2);
			}
			dataGridColumn = dataGridColumn2;
			if (dataGridColumn == null)
			{
				if (_negHorizontalOffset == 0.0)
				{
					return;
				}
				dataGridColumn = ColumnsItemsInternal[DisplayData.FirstDisplayedScrollingCol];
			}
		}
		double num = 0.0;
		foreach (DataGridColumn visibleScrollingColumn in ColumnsInternal.GetVisibleScrollingColumns())
		{
			if (visibleScrollingColumn == dataGridColumn)
			{
				break;
			}
			num += GetEdgedColumnWidth(visibleScrollingColumn);
		}
		UpdateHorizontalOffset(num);
	}

	private void UpdateDisplayedColumns()
	{
		DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
		ComputeDisplayedColumns();
	}

	private static DataGridBoundColumn GetDataGridColumnFromType(Type type)
	{
		if (type == typeof(bool))
		{
			return new DataGridCheckBoxColumn();
		}
		if (type == typeof(bool?))
		{
			return new DataGridCheckBoxColumn
			{
				IsThreeState = true
			};
		}
		return new DataGridTextColumn();
	}

	private void AutoGenerateColumnsPrivate()
	{
		if (!_measured || _autoGeneratingColumnOperationCount > 0)
		{
			return;
		}
		_autoGeneratingColumnOperationCount++;
		try
		{
			RemoveAutoGeneratedColumns();
			GenerateColumnsFromProperties();
			EnsureRowsPresenterVisibility();
			InvalidateRowHeightEstimate();
		}
		finally
		{
			_autoGeneratingColumnOperationCount--;
		}
	}

	private void GenerateColumnsFromProperties()
	{
		if (DataConnection.DataProperties != null && DataConnection.DataProperties.Length != 0)
		{
			List<KeyValuePair<int, DataGridAutoGeneratingColumnEventArgs>> list = new List<KeyValuePair<int, DataGridAutoGeneratingColumnEventArgs>>();
			PropertyInfo[] dataProperties = DataConnection.DataProperties;
			foreach (PropertyInfo propertyInfo in dataProperties)
			{
				string header = propertyInfo.Name;
				int num = 10000;
				object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), inherit: true);
				if (customAttributes != null && customAttributes.Length != 0)
				{
					DisplayAttribute displayAttribute = customAttributes[0] as DisplayAttribute;
					bool? autoGenerateField = displayAttribute.GetAutoGenerateField();
					if (autoGenerateField.HasValue && !autoGenerateField.Value)
					{
						continue;
					}
					string shortName = displayAttribute.GetShortName();
					if (shortName != null)
					{
						header = shortName;
					}
					int? order = displayAttribute.GetOrder();
					if (order.HasValue)
					{
						num = order.Value;
					}
				}
				int num2 = 0;
				if (num == int.MaxValue)
				{
					num2 = list.Count;
				}
				else
				{
					using List<KeyValuePair<int, DataGridAutoGeneratingColumnEventArgs>>.Enumerator enumerator = list.GetEnumerator();
					while (enumerator.MoveNext() && enumerator.Current.Key <= num)
					{
						num2++;
					}
				}
				DataGridAutoGeneratingColumnEventArgs value = GenerateColumn(propertyInfo.PropertyType, propertyInfo.Name, header);
				list.Insert(num2, new KeyValuePair<int, DataGridAutoGeneratingColumnEventArgs>(num, value));
			}
			{
				foreach (KeyValuePair<int, DataGridAutoGeneratingColumnEventArgs> item in list)
				{
					AddGeneratedColumn(item.Value);
				}
				return;
			}
		}
		if (DataConnection.DataIsPrimitive)
		{
			AddGeneratedColumn(GenerateColumn(DataConnection.DataType, string.Empty, DataConnection.DataType.Name));
		}
	}

	private static DataGridAutoGeneratingColumnEventArgs GenerateColumn(Type propertyType, string propertyName, string header)
	{
		DataGridBoundColumn dataGridColumnFromType = GetDataGridColumnFromType(propertyType);
		dataGridColumnFromType.Binding = new Binding(propertyName);
		dataGridColumnFromType.Header = header;
		dataGridColumnFromType.IsAutoGenerated = true;
		return new DataGridAutoGeneratingColumnEventArgs(propertyName, propertyType, dataGridColumnFromType);
	}

	private bool AddGeneratedColumn(DataGridAutoGeneratingColumnEventArgs e)
	{
		OnAutoGeneratingColumn(e);
		if (e.Cancel)
		{
			return false;
		}
		if (e.Column != null)
		{
			e.Column.IsAutoGenerated = true;
		}
		ColumnsInternal.Add(e.Column);
		ColumnsInternal.AutogeneratedColumnCount++;
		return true;
	}

	internal void ClearRowSelection(bool resetAnchorSlot)
	{
		if (resetAnchorSlot)
		{
			AnchorSlot = -1;
		}
		if (_selectedItems.Count <= 0)
		{
			return;
		}
		_noSelectionChangeCount++;
		try
		{
			for (int i = DisplayData.FirstScrollingSlot; i > -1 && i <= DisplayData.LastScrollingSlot; i++)
			{
				if (DisplayData.GetDisplayedElement(i) is DataGridRow dataGridRow && _selectedItems.ContainsSlot(dataGridRow.Slot))
				{
					SelectSlot(dataGridRow.Slot, isSelected: false);
				}
			}
			_selectedItems.ClearRows();
			SelectionHasChanged = true;
		}
		finally
		{
			NoSelectionChangeCount--;
		}
	}

	internal void ClearRowSelection(int slotException, bool setAnchorSlot)
	{
		_noSelectionChangeCount++;
		try
		{
			bool flag = false;
			if (_selectedItems.Count > 0)
			{
				for (int i = DisplayData.FirstScrollingSlot; i > -1 && i <= DisplayData.LastScrollingSlot; i++)
				{
					if (i != slotException && _selectedItems.ContainsSlot(i))
					{
						SelectSlot(i, isSelected: false);
						SelectionHasChanged = true;
					}
				}
				flag = _selectedItems.ContainsSlot(slotException);
				int count = _selectedItems.Count;
				if (count > 0)
				{
					if (count > 1)
					{
						SelectionHasChanged = true;
					}
					else if (_selectedItems.GetIndexes().First() != slotException)
					{
						SelectionHasChanged = true;
					}
					_selectedItems.ClearRows();
				}
			}
			if (flag)
			{
				_selectedItems.SelectSlot(slotException, select: true);
				if (setAnchorSlot)
				{
					AnchorSlot = slotException;
				}
			}
			else
			{
				SetRowSelection(slotException, isSelected: true, setAnchorSlot);
			}
		}
		finally
		{
			NoSelectionChangeCount--;
		}
	}

	internal int GetCollapsedSlotCount(int startSlot, int endSlot)
	{
		return _collapsedSlotsTable.GetIndexCount(startSlot, endSlot);
	}

	internal int GetNextVisibleSlot(int slot)
	{
		return _collapsedSlotsTable.GetNextGap(slot);
	}

	internal int GetPreviousVisibleSlot(int slot)
	{
		return _collapsedSlotsTable.GetPreviousGap(slot);
	}

	internal DataGridRow GetRowFromItem(object dataItem)
	{
		int num = DataConnection.IndexOf(dataItem);
		if (num < 0)
		{
			return null;
		}
		int slot = SlotFromRowIndex(num);
		if (!IsSlotVisible(slot))
		{
			return null;
		}
		return DisplayData.GetDisplayedElement(slot) as DataGridRow;
	}

	internal bool GetRowSelection(int slot)
	{
		return _selectedItems.ContainsSlot(slot);
	}

	internal void InsertElementAt(int slot, int rowIndex, object item, DataGridRowGroupInfo groupInfo, bool isCollapsed)
	{
		bool flag = rowIndex != -1;
		if (isCollapsed)
		{
			InsertElement(slot, null, updateVerticalScrollBarOnly: true, isCollapsed: true, flag);
		}
		else if (SlotIsDisplayed(slot))
		{
			if (flag)
			{
				InsertElement(slot, GenerateRow(rowIndex, slot, item), updateVerticalScrollBarOnly: false, isCollapsed: false, flag);
			}
			else
			{
				InsertElement(slot, GenerateRowGroupHeader(slot, groupInfo), updateVerticalScrollBarOnly: false, isCollapsed: false, flag);
			}
		}
		else
		{
			InsertElement(slot, null, _vScrollBar == null || _vScrollBar.IsVisible, isCollapsed: false, flag);
		}
	}

	internal void InsertRowAt(int rowIndex)
	{
		int slot = SlotFromRowIndex(rowIndex);
		object dataItem = DataConnection.GetDataItem(rowIndex);
		InsertElementAt(slot, rowIndex, dataItem, null, isCollapsed: false);
	}

	internal bool IsColumnDisplayed(int columnIndex)
	{
		if (columnIndex >= FirstDisplayedNonFillerColumnIndex)
		{
			return columnIndex <= DisplayData.LastTotallyDisplayedScrollingCol;
		}
		return false;
	}

	internal bool IsRowRecyclable(DataGridRow row)
	{
		if (row != EditingRow)
		{
			return row != _focusedRow;
		}
		return false;
	}

	internal bool IsSlotVisible(int slot)
	{
		if (slot >= DisplayData.FirstScrollingSlot && slot <= DisplayData.LastScrollingSlot && slot != -1)
		{
			return !_collapsedSlotsTable.Contains(slot);
		}
		return false;
	}

	internal void OnRowsMeasure()
	{
		if (!MathUtilities.IsZero(DisplayData.PendingVerticalScrollHeight))
		{
			ScrollSlotsByHeight(DisplayData.PendingVerticalScrollHeight);
			DisplayData.PendingVerticalScrollHeight = 0.0;
		}
	}

	internal void RefreshRows(bool recycleRows, bool clearRows)
	{
		if (_measured)
		{
			_desiredCurrentColumnIndex = CurrentColumnIndex;
			double val = _verticalOffset;
			if (DisplayData.PendingVerticalScrollHeight > 0.0)
			{
				val = DisplayData.PendingVerticalScrollHeight;
			}
			_verticalOffset = 0.0;
			NegVerticalOffset = 0.0;
			if (clearRows)
			{
				ClearRows(recycleRows);
				ClearRowGroupHeadersTable();
				PopulateRowGroupHeadersTable();
			}
			RefreshRowGroupHeaders();
			if (recycleRows && DataConnection.CollectionView != null)
			{
				CurrentSlot = ((DataConnection.CollectionView.CurrentPosition == -1) ? (-1) : SlotFromRowIndex(DataConnection.CollectionView.CurrentPosition));
				if (CurrentSlot == -1)
				{
					SetCurrentCellCore(-1, -1);
				}
			}
			if (DataConnection != null && ColumnsItemsInternal.Count > 0)
			{
				AddSlots(DataConnection.Count);
				AddSlots(DataConnection.Count + RowGroupHeadersTable.IndexCount);
				InvalidateMeasure();
			}
			EnsureRowGroupSpacerColumn();
			if (VerticalScrollBar != null)
			{
				DisplayData.PendingVerticalScrollHeight = Math.Min(val, VerticalScrollBar.Maximum);
			}
		}
		else
		{
			if (clearRows)
			{
				ClearRows(recycleRows);
			}
			ClearRowGroupHeadersTable();
			PopulateRowGroupHeadersTable();
		}
	}

	internal void RemoveRowAt(int rowIndex, object item)
	{
		RemoveElementAt(SlotFromRowIndex(rowIndex), item, isRow: true);
	}

	internal int RowIndexFromSlot(int slot)
	{
		return slot - RowGroupHeadersTable.GetIndexCount(0, slot);
	}

	internal bool ScrollSlotIntoView(int slot, bool scrolledHorizontally)
	{
		if (scrolledHorizontally && DisplayData.FirstScrollingSlot <= slot && DisplayData.LastScrollingSlot >= slot)
		{
			foreach (DataGridRow scrollingRow in DisplayData.GetScrollingRows())
			{
				scrollingRow.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			}
			UpdateDisplayedRows(DisplayData.FirstScrollingSlot, CellsHeight);
		}
		if (DisplayData.FirstScrollingSlot < slot && (DisplayData.LastScrollingSlot > slot || DisplayData.LastScrollingSlot == -1))
		{
			return true;
		}
		if (DisplayData.FirstScrollingSlot == slot && slot != -1)
		{
			if (!MathUtilities.IsZero(NegVerticalOffset))
			{
				DisplayData.PendingVerticalScrollHeight = 0.0 - NegVerticalOffset;
				InvalidateRowsMeasure(invalidateIndividualElements: false);
			}
			return true;
		}
		double num = 0.0;
		if (DisplayData.FirstScrollingSlot > slot)
		{
			int toSlot = DisplayData.FirstScrollingSlot - 1;
			if (MathUtilities.GreaterThan(NegVerticalOffset, 0.0))
			{
				num = 0.0 - NegVerticalOffset;
			}
			num -= GetSlotElementsHeight(slot, toSlot);
			if (DisplayData.FirstScrollingSlot - slot > 1)
			{
				ResetDisplayedRows();
			}
			NegVerticalOffset = 0.0;
			UpdateDisplayedRows(slot, CellsHeight);
		}
		else if (DisplayData.LastScrollingSlot <= slot)
		{
			int toSlot = DisplayData.LastScrollingSlot;
			double exactSlotElementHeight = GetExactSlotElementHeight(DisplayData.LastScrollingSlot);
			double num2 = AvailableSlotElementRoom + exactSlotElementHeight;
			if (MathUtilities.AreClose(exactSlotElementHeight, num2))
			{
				if (DisplayData.LastScrollingSlot == slot)
				{
					return true;
				}
				toSlot++;
			}
			else if (exactSlotElementHeight > num2)
			{
				toSlot++;
				num += exactSlotElementHeight - num2;
			}
			if (slot >= toSlot)
			{
				num += GetSlotElementsHeight(toSlot, slot);
			}
			if (slot - DisplayData.LastScrollingSlot > 1)
			{
				ResetDisplayedRows();
			}
			if (MathUtilities.GreaterThanOrClose(GetExactSlotElementHeight(slot), CellsHeight))
			{
				NegVerticalOffset = 0.0;
				UpdateDisplayedRows(slot, CellsHeight);
			}
			else
			{
				UpdateDisplayedRowsFromBottom(slot);
			}
		}
		_verticalOffset += num;
		if (_verticalOffset < 0.0 || DisplayData.FirstScrollingSlot == 0)
		{
			_verticalOffset = NegVerticalOffset;
		}
		SetVerticalOffset(_verticalOffset);
		InvalidateMeasure();
		InvalidateRowsMeasure(invalidateIndividualElements: false);
		return true;
	}

	internal void SetRowSelection(int slot, bool isSelected, bool setAnchorSlot)
	{
		_noSelectionChangeCount++;
		try
		{
			if (SelectionMode == DataGridSelectionMode.Single && isSelected && _selectedItems.Count > 0)
			{
				int num = _selectedItems.GetIndexes().First();
				if (num != slot)
				{
					SelectSlot(num, isSelected: false);
					SelectionHasChanged = true;
				}
			}
			if (_selectedItems.ContainsSlot(slot) != isSelected)
			{
				SelectSlot(slot, isSelected);
				SelectionHasChanged = true;
			}
			if (setAnchorSlot)
			{
				AnchorSlot = slot;
			}
		}
		finally
		{
			NoSelectionChangeCount--;
		}
	}

	internal void SetRowsSelection(int startSlot, int endSlot)
	{
		_noSelectionChangeCount++;
		try
		{
			if (!_selectedItems.ContainsAll(startSlot, endSlot))
			{
				SelectSlots(startSlot, endSlot, isSelected: true);
				SelectionHasChanged = true;
			}
		}
		finally
		{
			NoSelectionChangeCount--;
		}
	}

	internal int SlotFromRowIndex(int rowIndex)
	{
		return rowIndex + RowGroupHeadersTable.GetIndexCountBeforeGap(0, rowIndex);
	}

	private void AddSlotElement(int slot, Control element)
	{
		OnAddedElement_Phase1(slot, element);
		SlotCount++;
		VisibleSlotCount++;
		OnAddedElement_Phase2(slot, updateVerticalScrollBarOnly: false);
		OnElementsChanged(grew: true);
	}

	private void AddSlots(int totalSlots)
	{
		SlotCount = 0;
		VisibleSlotCount = 0;
		IEnumerator<int> enumerator = null;
		int num = -1;
		if (RowGroupHeadersTable.RangeCount > 0)
		{
			enumerator = RowGroupHeadersTable.GetIndexes().GetEnumerator();
			if (enumerator != null && enumerator.MoveNext())
			{
				num = enumerator.Current;
			}
		}
		int i = 0;
		int num2 = 0;
		for (; i < totalSlots; i++)
		{
			if (!(AvailableSlotElementRoom > 0.0))
			{
				break;
			}
			if (i == num)
			{
				DataGridRowGroupInfo valueAt = RowGroupHeadersTable.GetValueAt(i);
				AddSlotElement(i, GenerateRowGroupHeader(i, valueAt));
				num = (enumerator.MoveNext() ? enumerator.Current : (-1));
			}
			else
			{
				AddSlotElement(i, GenerateRow(num2, i));
				num2++;
			}
		}
		if (i < totalSlots)
		{
			SlotCount += totalSlots - i;
			VisibleSlotCount += totalSlots - i;
			OnAddedElement_Phase2(0, _vScrollBar == null || _vScrollBar.IsVisible);
			OnElementsChanged(grew: true);
		}
	}

	private void ApplyDisplayedRowsState(int startSlot, int endSlot)
	{
		int num = Math.Max(DisplayData.FirstScrollingSlot, startSlot);
		int num2 = Math.Min(DisplayData.LastScrollingSlot, endSlot);
		if (num < 0)
		{
			return;
		}
		for (int nextVisibleSlot = GetNextVisibleSlot(num - 1); nextVisibleSlot <= num2; nextVisibleSlot = GetNextVisibleSlot(nextVisibleSlot))
		{
			if (DisplayData.GetDisplayedElement(nextVisibleSlot) is DataGridRow dataGridRow)
			{
				dataGridRow.UpdatePseudoClasses();
			}
		}
	}

	private void ClearRows(bool recycle)
	{
		SetCurrentCellCore(-1, -1, commitEdit: false, endRowEdit: false);
		ClearRowSelection(resetAnchorSlot: true);
		UnloadElements(recycle);
		_showDetailsTable.Clear();
		SlotCount = 0;
		NegVerticalOffset = 0.0;
		SetVerticalOffset(0.0);
		ComputeScrollBarsLayout();
	}

	private double CollapseSlotsInTable(int startSlot, int endSlot, ref int slotsExpanded, int lastDisplayedSlot, ref double heightChangeBelowLastDisplayedSlot)
	{
		int num = startSlot;
		double num2 = 0.0;
		while (num <= endSlot)
		{
			num = _collapsedSlotsTable.GetNextGap(num - 1);
			int num3 = _collapsedSlotsTable.GetNextIndex(num) - 1;
			int num4 = ((num3 == -2) ? endSlot : Math.Min(endSlot, num3));
			if (num > num4)
			{
				continue;
			}
			double heightEstimate = GetHeightEstimate(num, num4);
			num2 -= heightEstimate;
			slotsExpanded -= num4 - num + 1;
			if (num4 > lastDisplayedSlot)
			{
				if (num > lastDisplayedSlot)
				{
					heightChangeBelowLastDisplayedSlot -= heightEstimate;
				}
				else
				{
					heightChangeBelowLastDisplayedSlot -= GetHeightEstimate(lastDisplayedSlot + 1, num4);
				}
			}
			num = num4 + 1;
		}
		_collapsedSlotsTable.AddValues(startSlot, endSlot - startSlot + 1, value: false);
		return num2;
	}

	private static void CorrectRowAfterDeletion(DataGridRow row, bool rowDeleted)
	{
		row.Slot--;
		if (rowDeleted)
		{
			row.Index--;
		}
	}

	private static void CorrectRowAfterInsertion(DataGridRow row, bool rowInserted)
	{
		row.Slot++;
		if (rowInserted)
		{
			row.Index++;
		}
	}

	private void CorrectSlotsAfterDeletion(int slotDeleted, bool wasRow)
	{
		int num = 0;
		while (num < _loadedRows.Count)
		{
			DataGridRow dataGridRow = _loadedRows[num];
			if (IsSlotVisible(dataGridRow.Slot))
			{
				num++;
			}
			else if (dataGridRow.Slot > slotDeleted)
			{
				CorrectRowAfterDeletion(dataGridRow, wasRow);
				num++;
			}
			else if (dataGridRow.Slot == slotDeleted)
			{
				_loadedRows.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
		if (EditingRow != null && !IsSlotVisible(EditingRow.Slot) && EditingRow.Slot > slotDeleted)
		{
			CorrectRowAfterDeletion(EditingRow, wasRow);
		}
		if (_focusedRow != null && _focusedRow != EditingRow && !IsSlotVisible(_focusedRow.Slot) && _focusedRow.Slot > slotDeleted)
		{
			CorrectRowAfterDeletion(_focusedRow, wasRow);
		}
		foreach (DataGridRow scrollingRow in DisplayData.GetScrollingRows())
		{
			if (scrollingRow.Slot > slotDeleted)
			{
				CorrectRowAfterDeletion(scrollingRow, wasRow);
				_rowsPresenter?.InvalidateChildIndex(scrollingRow);
			}
		}
		foreach (int index in RowGroupHeadersTable.GetIndexes())
		{
			DataGridRowGroupInfo valueAt = RowGroupHeadersTable.GetValueAt(index);
			if (valueAt.Slot > slotDeleted)
			{
				valueAt.Slot--;
			}
			if (valueAt.LastSubItemSlot >= slotDeleted)
			{
				valueAt.LastSubItemSlot--;
			}
		}
		if (_lastEstimatedRow >= slotDeleted)
		{
			_lastEstimatedRow--;
		}
	}

	private void CorrectSlotsAfterInsertion(int slotInserted, bool isCollapsed, bool rowInserted)
	{
		foreach (DataGridRow loadedRow in _loadedRows)
		{
			if (!IsSlotVisible(loadedRow.Slot) && loadedRow.Slot >= slotInserted)
			{
				CorrectRowAfterInsertion(loadedRow, rowInserted);
			}
		}
		if (_focusedRow != null && _focusedRow != EditingRow && !IsSlotVisible(_focusedRow.Slot) && !(_focusedRow.Slot == slotInserted && isCollapsed) && _focusedRow.Slot >= slotInserted)
		{
			CorrectRowAfterInsertion(_focusedRow, rowInserted);
		}
		foreach (DataGridRow scrollingRow in DisplayData.GetScrollingRows())
		{
			if (scrollingRow.Slot >= slotInserted)
			{
				CorrectRowAfterInsertion(scrollingRow, rowInserted);
				_rowsPresenter?.InvalidateChildIndex(scrollingRow);
			}
		}
		if (EditingRow != null)
		{
			EditingRow.Index = DataConnection.IndexOf(EditingRow.DataContext);
			EditingRow.Slot = SlotFromRowIndex(EditingRow.Index);
		}
		foreach (int index in RowGroupHeadersTable.GetIndexes(slotInserted))
		{
			DataGridRowGroupInfo valueAt = RowGroupHeadersTable.GetValueAt(index);
			if (valueAt.Slot >= slotInserted)
			{
				valueAt.Slot++;
			}
			if (valueAt.LastSubItemSlot > slotInserted)
			{
				valueAt.LastSubItemSlot++;
			}
		}
		if (_lastEstimatedRow >= slotInserted)
		{
			_lastEstimatedRow++;
		}
	}

	private IEnumerable<DataGridRow> GetAllRows()
	{
		if (_rowsPresenter == null)
		{
			yield break;
		}
		foreach (Control child in _rowsPresenter.Children)
		{
			if (child is DataGridRow dataGridRow)
			{
				yield return dataGridRow;
			}
		}
	}

	private void ExpandSlots(int startSlot, int endSlot, bool isDisplayed, ref int slotsExpanded, ref double totalHeightChange)
	{
		double num = 0.0;
		if (isDisplayed)
		{
			for (int num2 = DisplayData.FirstScrollingSlot; num2 < startSlot; num2 = GetNextVisibleSlot(num2))
			{
				num += GetExactSlotElementHeight(num2);
			}
			for (int i = 0; i < endSlot - startSlot + 1; i++)
			{
				if (DisplayData.LastScrollingSlot <= endSlot)
				{
					break;
				}
				RemoveDisplayedElement(DisplayData.LastScrollingSlot, wasDeleted: false, updateSlotInformation: true);
			}
		}
		double num3 = 0.0;
		int num4 = startSlot;
		int num5 = endSlot;
		while (num4 <= endSlot)
		{
			num4 = _collapsedSlotsTable.GetNextIndex(num4 - 1);
			if (num4 == -1)
			{
				break;
			}
			num5 = Math.Min(endSlot, _collapsedSlotsTable.GetNextGap(num4) - 1);
			if (num4 <= num5)
			{
				if (!isDisplayed)
				{
					double headersHeight;
					double num6 = num5 - num4 - GetRowGroupHeaderCount(num4, num5, false, out headersHeight) + 1;
					double num7 = GetDetailsCountInclusive(num4, num5);
					num3 += headersHeight + num7 * RowDetailsHeightEstimate + num6 * RowHeightEstimate;
				}
				slotsExpanded += num5 - num4 + 1;
				num4 = num5 + 1;
			}
		}
		_collapsedSlotsTable.RemoveValues(startSlot, endSlot - startSlot + 1);
		if (isDisplayed)
		{
			double num8 = CellsHeight - num;
			for (int j = startSlot; j <= endSlot; j++)
			{
				if (!(num3 < num8))
				{
					break;
				}
				Control control = InsertDisplayedElement(j, updateSlotInformation: false);
				num3 += control.DesiredSize.Height;
				if (j > DisplayData.LastScrollingSlot)
				{
					DisplayData.LastScrollingSlot = j;
				}
			}
		}
		totalHeightChange += num3;
	}

	private void GenerateEditingElements()
	{
		if (EditingRow == null || EditingRow.Cells == null)
		{
			return;
		}
		foreach (DataGridColumn displayedColumn in ColumnsInternal.GetDisplayedColumns((DataGridColumn c) => c.IsVisible && !c.IsReadOnly))
		{
			displayedColumn.GenerateEditingElementInternal(EditingRow.Cells[displayedColumn.Index], EditingRow.DataContext);
		}
	}

	private DataGridRow GenerateRow(int rowIndex, int slot)
	{
		return GenerateRow(rowIndex, slot, DataConnection.GetDataItem(rowIndex));
	}

	private DataGridRow GenerateRow(int rowIndex, int slot, object dataContext)
	{
		DataGridRow dataGridRow = GetGeneratedRow(dataContext);
		if (dataGridRow == null)
		{
			dataGridRow = DisplayData.GetUsedRow() ?? new DataGridRow();
			dataGridRow.Index = rowIndex;
			dataGridRow.Slot = slot;
			dataGridRow.OwningGrid = this;
			dataGridRow.DataContext = dataContext;
			ControlTheme rowTheme = RowTheme;
			if (rowTheme != null)
			{
				dataGridRow.SetValue(StyledElement.ThemeProperty, rowTheme, BindingPriority.Template);
			}
			CompleteCellsCollection(dataGridRow);
			OnLoadingRow(new DataGridRowEventArgs(dataGridRow));
		}
		return dataGridRow;
	}

	private double GetExactSlotElementHeight(int slot)
	{
		if (IsSlotVisible(slot))
		{
			return DisplayData.GetDisplayedElement(slot).DesiredSize.Height;
		}
		return InsertDisplayedElement(slot, updateSlotInformation: true).DesiredSize.Height;
	}

	private double GetHeightEstimate(int fromSlot, int toSlot)
	{
		double headersHeight;
		double num = toSlot - fromSlot - GetRowGroupHeaderCount(fromSlot, toSlot, true, out headersHeight) + 1;
		double num2 = GetDetailsCountInclusive(fromSlot, toSlot);
		return headersHeight + num2 * RowDetailsHeightEstimate + num * RowHeightEstimate;
	}

	private double GetSlotElementHeight(int slot)
	{
		if (IsSlotVisible(slot))
		{
			return DisplayData.GetDisplayedElement(slot).DesiredSize.Height;
		}
		DataGridRowGroupInfo valueAt = RowGroupHeadersTable.GetValueAt(slot);
		if (valueAt != null)
		{
			return _rowGroupHeightsByLevel[valueAt.Level];
		}
		return RowHeightEstimate + (GetRowDetailsVisibility(slot) ? RowDetailsHeightEstimate : 0.0);
	}

	private double GetSlotElementsHeight(int fromSlot, int toSlot)
	{
		double num = 0.0;
		for (int i = fromSlot; i <= toSlot; i++)
		{
			num += GetSlotElementHeight(i);
		}
		return num;
	}

	private DataGridRow GetGeneratedRow(object dataContext)
	{
		DataGridRow loadedRow = GetLoadedRow(dataContext);
		if (loadedRow != null)
		{
			return loadedRow;
		}
		if (EditingRow != null && dataContext == EditingRow.DataContext)
		{
			return EditingRow;
		}
		if (_focusedRow != null && dataContext == _focusedRow.DataContext)
		{
			return _focusedRow;
		}
		return null;
	}

	private DataGridRow GetLoadedRow(object dataContext)
	{
		foreach (DataGridRow loadedRow in _loadedRows)
		{
			if (loadedRow.DataContext == dataContext)
			{
				return loadedRow;
			}
		}
		return null;
	}

	private Control InsertDisplayedElement(int slot, bool updateSlotInformation)
	{
		Control control = ((!RowGroupHeadersTable.Contains(slot)) ? ((TemplatedControl)GenerateRow(RowIndexFromSlot(slot), slot)) : ((TemplatedControl)GenerateRowGroupHeader(slot, RowGroupHeadersTable.GetValueAt(slot))));
		InsertDisplayedElement(slot, control, wasNewlyAdded: false, updateSlotInformation);
		return control;
	}

	private void InsertDisplayedElement(int slot, Control element, bool wasNewlyAdded, bool updateSlotInformation)
	{
		if (_rowsPresenter != null)
		{
			DataGridRowGroupHeader dataGridRowGroupHeader = null;
			DataGridRow dataGridRow = element as DataGridRow;
			if (dataGridRow != null)
			{
				LoadRowVisualsForDisplay(dataGridRow);
				if (IsRowRecyclable(dataGridRow))
				{
					if (!dataGridRow.IsRecycled)
					{
						_rowsPresenter.Children.Add(dataGridRow);
					}
				}
				else
				{
					element.Clip = null;
				}
			}
			else
			{
				dataGridRowGroupHeader = element as DataGridRowGroupHeader;
				if (dataGridRowGroupHeader != null)
				{
					dataGridRowGroupHeader.TotalIndent = ((dataGridRowGroupHeader.Level == 0) ? 0.0 : RowGroupSublevelIndents[dataGridRowGroupHeader.Level - 1]);
					if (!dataGridRowGroupHeader.IsRecycled)
					{
						_rowsPresenter.Children.Add(element);
					}
					dataGridRowGroupHeader.LoadVisualsForDisplay();
				}
			}
			element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			AvailableSlotElementRoom -= element.DesiredSize.Height;
			if (dataGridRowGroupHeader != null)
			{
				_rowGroupHeightsByLevel[dataGridRowGroupHeader.Level] = dataGridRowGroupHeader.DesiredSize.Height;
			}
			if (dataGridRow != null && RowHeightEstimate == 22.0 && double.IsNaN(dataGridRow.Height))
			{
				RowHeightEstimate = element.DesiredSize.Height;
			}
		}
		if (wasNewlyAdded)
		{
			DisplayData.CorrectSlotsAfterInsertion(slot, element, isCollapsed: false);
		}
		else
		{
			DisplayData.LoadScrollingSlot(slot, element, updateSlotInformation);
		}
	}

	private void InsertElement(int slot, Control element, bool updateVerticalScrollBarOnly, bool isCollapsed, bool isRow)
	{
		OnInsertingElement(slot, firstInsertion: true, isCollapsed);
		OnInsertedElement_Phase1(slot, element, isCollapsed, isRow);
		SlotCount++;
		if (!isCollapsed)
		{
			VisibleSlotCount++;
		}
		OnInsertedElement_Phase2(slot, updateVerticalScrollBarOnly, isCollapsed);
	}

	private void InvalidateRowHeightEstimate()
	{
		_lastEstimatedRow = -1;
	}

	private void OnAddedElement_Phase1(int slot, Control element)
	{
		if (SlotIsDisplayed(slot))
		{
			InsertDisplayedElement(slot, element, wasNewlyAdded: true, updateSlotInformation: true);
		}
	}

	private void OnAddedElement_Phase2(int slot, bool updateVerticalScrollBarOnly)
	{
		if (slot < DisplayData.FirstScrollingSlot - 1)
		{
			double num = (RowGroupHeadersTable.Contains(slot) ? RowGroupHeaderHeightEstimate : RowHeightEstimate);
			SetVerticalOffset(_verticalOffset + num);
		}
		if (updateVerticalScrollBarOnly)
		{
			UpdateVerticalScrollBar();
			return;
		}
		ComputeScrollBarsLayout();
		InvalidateRowsArrange();
	}

	private void OnElementsChanged(bool grew)
	{
		if (grew && ColumnsItemsInternal.Count > 0 && CurrentColumnIndex == -1)
		{
			MakeFirstDisplayedCellCurrentCell();
		}
	}

	private void OnInsertedElement_Phase1(int slot, Control element, bool isCollapsed, bool isRow)
	{
		CorrectSlotsAfterInsertion(slot, isCollapsed, isRow);
		if (element != null)
		{
			OnAddedElement_Phase1(slot, element);
		}
		else if (slot <= DisplayData.FirstScrollingSlot || (isCollapsed && slot <= DisplayData.LastScrollingSlot))
		{
			DisplayData.CorrectSlotsAfterInsertion(slot, null, isCollapsed);
		}
	}

	private void OnInsertedElement_Phase2(int slot, bool updateVerticalScrollBarOnly, bool isCollapsed)
	{
		if (!isCollapsed)
		{
			OnAddedElement_Phase2(slot, updateVerticalScrollBarOnly);
		}
	}

	private void OnInsertingElement(int slotInserted, bool firstInsertion, bool isCollapsed)
	{
		if (firstInsertion && CurrentSlot != -1 && slotInserted <= CurrentSlot)
		{
			_temporarilyResetCurrentCell = true;
			SetCurrentCellCore(-1, -1);
		}
		_showDetailsTable.InsertIndex(slotInserted);
		RowGroupHeadersTable.InsertIndex(slotInserted);
		_selectedItems.InsertIndex(slotInserted);
		if (isCollapsed)
		{
			_collapsedSlotsTable.InsertIndexAndValue(slotInserted, value: false);
		}
		else
		{
			_collapsedSlotsTable.InsertIndex(slotInserted);
		}
		if (slotInserted <= SelectedIndex)
		{
			SetValueNoCallback(SelectedIndexProperty, SelectedIndex + 1);
		}
	}

	private void OnRemovedElement(int slotDeleted, object itemDeleted)
	{
		SlotCount--;
		bool flag = _collapsedSlotsTable.Contains(slotDeleted);
		if (!flag)
		{
			VisibleSlotCount--;
		}
		if (_focusedRow != null && _focusedRow.Slot == slotDeleted)
		{
			ResetFocusedRow();
		}
		Control control = null;
		if (slotDeleted <= DisplayData.LastScrollingSlot)
		{
			if (slotDeleted >= DisplayData.FirstScrollingSlot && !flag)
			{
				control = DisplayData.GetDisplayedElement(slotDeleted);
				UpdateTablesForRemoval(slotDeleted, itemDeleted);
				RemoveDisplayedElement(control, slotDeleted, wasDeleted: true, updateSlotInformation: true);
			}
			else
			{
				UpdateTablesForRemoval(slotDeleted, itemDeleted);
				DisplayData.CorrectSlotsAfterDeletion(slotDeleted, flag);
			}
		}
		else
		{
			UpdateTablesForRemoval(slotDeleted, itemDeleted);
		}
		if (slotDeleted < SelectedIndex)
		{
			SetValueNoCallback(SelectedIndexProperty, SelectedIndex - 1);
		}
		if (flag)
		{
			return;
		}
		if (slotDeleted >= DisplayData.LastScrollingSlot && control == null)
		{
			UpdateVerticalScrollBar();
			return;
		}
		if (control != null)
		{
			AvailableSlotElementRoom += control.DesiredSize.Height;
		}
		else
		{
			SetVerticalOffset(Math.Max(0.0, _verticalOffset - RowHeightEstimate));
		}
		ComputeScrollBarsLayout();
		InvalidateRowsArrange();
	}

	private void OnRemovingElement(int slotDeleted)
	{
		_temporarilyResetCurrentCell = false;
		if (CurrentSlot != -1 && slotDeleted <= CurrentSlot)
		{
			_desiredCurrentColumnIndex = CurrentColumnIndex;
			if (slotDeleted == CurrentSlot)
			{
				SetCurrentCellCore(-1, -1, commitEdit: false, endRowEdit: false);
				return;
			}
			_temporarilyResetCurrentCell = true;
			SetCurrentCellCore(-1, -1);
		}
	}

	private void LoadRowVisualsForDisplay(DataGridRow row)
	{
		if (row.IsRecycled)
		{
			row.ApplyCellsState();
			_rowsPresenter?.InvalidateChildIndex(row);
		}
		else if (row == EditingRow)
		{
			row.ApplyCellsState();
		}
		row.EnsureHeaderStyleAndVisibility(null);
		if (CurrentColumnIndex != -1 && CurrentSlot != -1 && row.Index == CurrentSlot)
		{
			row.Cells[CurrentColumnIndex].UpdatePseudoClasses();
		}
		if (row.IsSelected || row.IsRecycled)
		{
			row.UpdatePseudoClasses();
		}
		EnsureRowDetailsVisibility(row, raiseNotification: false, animate: false);
	}

	private void RemoveDisplayedElement(int slot, bool wasDeleted, bool updateSlotInformation)
	{
		RemoveDisplayedElement(DisplayData.GetDisplayedElement(slot), slot, wasDeleted, updateSlotInformation);
	}

	private void RemoveDisplayedElement(Control element, int slot, bool wasDeleted, bool updateSlotInformation)
	{
		if (element is DataGridRow dataGridRow)
		{
			if (IsRowRecyclable(dataGridRow))
			{
				UnloadRow(dataGridRow);
			}
			else
			{
				dataGridRow.Clip = new RectangleGeometry();
			}
		}
		else if (element is DataGridRowGroupHeader dataGridRowGroupHeader)
		{
			OnUnloadingRowGroup(new DataGridRowGroupHeaderEventArgs(dataGridRowGroupHeader));
			DisplayData.AddRecylableRowGroupHeader(dataGridRowGroupHeader);
		}
		else if (_rowsPresenter != null)
		{
			_rowsPresenter.Children.Remove(element);
		}
		if (wasDeleted)
		{
			DisplayData.CorrectSlotsAfterDeletion(slot, wasCollapsed: false);
		}
		else
		{
			DisplayData.UnloadScrollingElement(slot, updateSlotInformation, wasDeleted: false);
		}
	}

	private void RemoveEditingElements()
	{
		if (EditingRow == null || EditingRow.Cells == null)
		{
			return;
		}
		foreach (DataGridColumn column in Columns)
		{
			column.RemoveEditingElement();
		}
	}

	private void RemoveElementAt(int slot, object item, bool isRow)
	{
		OnRemovingElement(slot);
		CorrectSlotsAfterDeletion(slot, isRow);
		OnRemovedElement(slot, item);
		if (_temporarilyResetCurrentCell && _editingColumnIndex != -1 && _previousCurrentItem != null && EditingRow != null && EditingRow.Slot != -1)
		{
			ProcessSelectionAndCurrency(_editingColumnIndex, _previousCurrentItem, EditingRow.Slot, DataGridSelectionAction.None, scrollIntoView: false);
		}
	}

	private void RemoveNonDisplayedRows(int newFirstDisplayedSlot, int newLastDisplayedSlot)
	{
		while (DisplayData.FirstScrollingSlot < newFirstDisplayedSlot)
		{
			RemoveDisplayedElement(DisplayData.FirstScrollingSlot, wasDeleted: false, updateSlotInformation: true);
		}
		while (DisplayData.LastScrollingSlot > newLastDisplayedSlot)
		{
			RemoveDisplayedElement(DisplayData.LastScrollingSlot, wasDeleted: false, updateSlotInformation: true);
		}
	}

	private void ResetDisplayedRows()
	{
		if (this.UnloadingRow != null || this.UnloadingRowGroup != null)
		{
			foreach (Control scrollingElement in DisplayData.GetScrollingElements())
			{
				if (scrollingElement is DataGridRow dataGridRow)
				{
					if (IsRowRecyclable(dataGridRow))
					{
						OnUnloadingRow(new DataGridRowEventArgs(dataGridRow));
					}
				}
				else if (scrollingElement is DataGridRowGroupHeader rowGroupHeader)
				{
					OnUnloadingRowGroup(new DataGridRowGroupHeaderEventArgs(rowGroupHeader));
				}
			}
		}
		DisplayData.ClearElements(recycle: true);
		AvailableSlotElementRoom = CellsHeight;
	}

	private bool SlotIsDisplayed(int slot)
	{
		if (slot >= DisplayData.FirstScrollingSlot && slot <= DisplayData.LastScrollingSlot)
		{
			return true;
		}
		if (DisplayData.FirstScrollingSlot == -1 && CellsHeight > 0.0 && CellsWidth > 0.0)
		{
			return true;
		}
		if (slot == GetNextVisibleSlot(DisplayData.LastScrollingSlot) && AvailableSlotElementRoom > 0.0)
		{
			return true;
		}
		return false;
	}

	private void ScrollSlotsByHeight(double height)
	{
		_scrollingByHeight = true;
		try
		{
			double num = 0.0;
			int num2 = DisplayData.FirstScrollingSlot;
			double num3 = _verticalOffset + height;
			if (height > 0.0)
			{
				int previousVisibleSlot = GetPreviousVisibleSlot(SlotCount);
				if (_vScrollBar != null && MathUtilities.AreClose(_vScrollBar.Maximum, num3))
				{
					ResetDisplayedRows();
					UpdateDisplayedRowsFromBottom(previousVisibleSlot);
					num2 = DisplayData.FirstScrollingSlot;
				}
				else
				{
					num = GetSlotElementHeight(num2) - NegVerticalOffset;
					if (MathUtilities.LessThan(height, num))
					{
						NegVerticalOffset += height;
					}
					else
					{
						NegVerticalOffset = 0.0;
						if (height > 2.0 * CellsHeight && (RowDetailsVisibilityMode != 0 || RowDetailsTemplate == null))
						{
							ResetDisplayedRows();
							double num4 = RowHeightEstimate + ((RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Visible) ? RowDetailsHeightEstimate : 0.0);
							int num5 = num2 + (int)(height / num4);
							num5 += _collapsedSlotsTable.GetIndexCount(num2, num2 + num5);
							num2 = Math.Min(GetNextVisibleSlot(num5), previousVisibleSlot);
						}
						else
						{
							double exactSlotElementHeight;
							for (; MathUtilities.LessThanOrClose(num, height); num += exactSlotElementHeight)
							{
								if (num2 < previousVisibleSlot)
								{
									if (IsSlotVisible(num2))
									{
										RemoveDisplayedElement(num2, wasDeleted: false, updateSlotInformation: true);
									}
									num2 = GetNextVisibleSlot(num2);
									exactSlotElementHeight = GetExactSlotElementHeight(num2);
									double num6 = height - num;
									if (!MathUtilities.LessThanOrClose(exactSlotElementHeight, num6))
									{
										NegVerticalOffset = num6;
										break;
									}
									continue;
								}
								NegVerticalOffset = 0.0;
								break;
							}
						}
					}
				}
			}
			else
			{
				if (MathUtilities.GreaterThanOrClose(height + NegVerticalOffset, 0.0))
				{
					NegVerticalOffset += height;
				}
				else
				{
					num = 0.0 - NegVerticalOffset;
					NegVerticalOffset = 0.0;
					if (height < -2.0 * CellsHeight && (RowDetailsVisibilityMode != 0 || RowDetailsTemplate == null))
					{
						if (num3 == 0.0)
						{
							num2 = 0;
						}
						else
						{
							double num7 = RowHeightEstimate + ((RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Visible) ? RowDetailsHeightEstimate : 0.0);
							int num8 = num2 + (int)(height / num7);
							num8 -= _collapsedSlotsTable.GetIndexCount(num8, num2);
							num2 = Math.Max(0, GetPreviousVisibleSlot(num8 + 1));
						}
						ResetDisplayedRows();
					}
					else
					{
						int slot = DisplayData.LastScrollingSlot;
						while (MathUtilities.GreaterThan(num, height))
						{
							if (num2 > 0)
							{
								if (IsSlotVisible(slot))
								{
									RemoveDisplayedElement(slot, wasDeleted: false, updateSlotInformation: true);
									slot = GetPreviousVisibleSlot(slot);
								}
								num2 = GetPreviousVisibleSlot(num2);
								double exactSlotElementHeight2 = GetExactSlotElementHeight(num2);
								double num9 = height - num;
								if (MathUtilities.LessThanOrClose(exactSlotElementHeight2 + num9, 0.0))
								{
									num -= exactSlotElementHeight2;
									continue;
								}
								NegVerticalOffset = exactSlotElementHeight2 + num9;
								break;
							}
							NegVerticalOffset = 0.0;
							break;
						}
					}
				}
				if (MathUtilities.GreaterThanOrClose(0.0, num3) && num2 != 0)
				{
					ResetDisplayedRows();
					NegVerticalOffset = 0.0;
					UpdateDisplayedRows(0, CellsHeight);
					num2 = 0;
				}
			}
			if (MathUtilities.LessThan(GetExactSlotElementHeight(num2), NegVerticalOffset))
			{
				if (num2 < SlotCount - 1)
				{
					num2 = GetNextVisibleSlot(num2);
				}
				NegVerticalOffset = 0.0;
			}
			UpdateDisplayedRows(num2, CellsHeight);
			double exactSlotElementHeight3 = GetExactSlotElementHeight(DisplayData.FirstScrollingSlot);
			if (MathUtilities.GreaterThan(NegVerticalOffset, exactSlotElementHeight3))
			{
				int num10 = DisplayData.FirstScrollingSlot;
				while (num2 > 0 && MathUtilities.GreaterThan(NegVerticalOffset, exactSlotElementHeight3))
				{
					int previousVisibleSlot2 = GetPreviousVisibleSlot(num10);
					if (previousVisibleSlot2 == -1)
					{
						NegVerticalOffset = 0.0;
						_verticalOffset = 0.0;
						continue;
					}
					NegVerticalOffset -= exactSlotElementHeight3;
					_verticalOffset = Math.Max(0.0, _verticalOffset - exactSlotElementHeight3);
					num10 = previousVisibleSlot2;
					exactSlotElementHeight3 = GetExactSlotElementHeight(num10);
				}
				if (num10 != DisplayData.FirstScrollingSlot)
				{
					UpdateDisplayedRows(num10, CellsHeight);
				}
			}
			if (DisplayData.FirstScrollingSlot == 0)
			{
				_verticalOffset = NegVerticalOffset;
			}
			else if (MathUtilities.GreaterThan(NegVerticalOffset, num3))
			{
				NegVerticalOffset = num3;
				_verticalOffset = num3;
			}
			else
			{
				_verticalOffset = num3;
			}
			SetVerticalOffset(_verticalOffset);
			DisplayData.FullyRecycleElements();
		}
		finally
		{
			_scrollingByHeight = false;
		}
	}

	private void SelectDisplayedElement(int slot)
	{
		Control displayedElement = DisplayData.GetDisplayedElement(slot);
		if (displayedElement is DataGridRow dataGridRow)
		{
			dataGridRow.UpdatePseudoClasses();
			EnsureRowDetailsVisibility(dataGridRow, raiseNotification: true, animate: true);
		}
		else
		{
			(displayedElement as DataGridRowGroupHeader).UpdatePseudoClasses();
		}
	}

	private void SelectSlot(int slot, bool isSelected)
	{
		_selectedItems.SelectSlot(slot, isSelected);
		if (IsSlotVisible(slot))
		{
			SelectDisplayedElement(slot);
		}
	}

	private void SelectSlots(int startSlot, int endSlot, bool isSelected)
	{
		_selectedItems.SelectSlots(startSlot, endSlot, isSelected);
		int num = Math.Max(DisplayData.FirstScrollingSlot, startSlot);
		int num2 = Math.Min(DisplayData.LastScrollingSlot, endSlot);
		for (int i = num; i <= num2; i++)
		{
			if (IsSlotVisible(i))
			{
				SelectDisplayedElement(i);
			}
		}
	}

	private void UnloadElements(bool recycle)
	{
		if (!CommitEdit())
		{
			CancelEdit(DataGridEditingUnit.Row, raiseEvents: false);
		}
		ResetEditingRow();
		if (_focusedRow != null)
		{
			ResetFocusedRow();
			Focus();
		}
		if (_rowsPresenter != null)
		{
			foreach (Control child in _rowsPresenter.Children)
			{
				if (child is DataGridRow dataGridRow)
				{
					if (IsSlotVisible(dataGridRow.Slot))
					{
						OnUnloadingRow(new DataGridRowEventArgs(dataGridRow));
					}
					dataGridRow.DetachFromDataGrid(recycle && dataGridRow.IsRecyclable);
				}
				else if (child is DataGridRowGroupHeader dataGridRowGroupHeader && IsSlotVisible(dataGridRowGroupHeader.RowGroupInfo.Slot))
				{
					OnUnloadingRowGroup(new DataGridRowGroupHeaderEventArgs(dataGridRowGroupHeader));
				}
			}
			if (!recycle)
			{
				_rowsPresenter.Children.Clear();
			}
		}
		DisplayData.ClearElements(recycle);
		AvailableSlotElementRoom = CellsHeight;
		VisibleSlotCount = 0;
	}

	private void UnloadRow(DataGridRow dataGridRow)
	{
		if (!_loadedRows.Contains(dataGridRow))
		{
			OnUnloadingRow(new DataGridRowEventArgs(dataGridRow));
			if (CurrentSlot != dataGridRow.Index)
			{
				DisplayData.AddRecyclableRow(dataGridRow);
				return;
			}
			_rowsPresenter.Children.Remove(dataGridRow);
			dataGridRow.DetachFromDataGrid(recycle: false);
		}
	}

	private void UpdateDisplayedRows(int newFirstDisplayedSlot, double displayHeight)
	{
		int num = newFirstDisplayedSlot;
		int newLastDisplayedSlot = -1;
		double num2 = 0.0 - NegVerticalOffset;
		int num3 = 0;
		if (MathUtilities.LessThanOrClose(displayHeight, 0.0) || SlotCount == 0 || ColumnsItemsInternal.Count == 0)
		{
			return;
		}
		if (num == -1)
		{
			num = 0;
		}
		int num4 = num;
		while (num4 < SlotCount && !MathUtilities.GreaterThanOrClose(num2, displayHeight))
		{
			num2 += GetExactSlotElementHeight(num4);
			num3++;
			newLastDisplayedSlot = num4;
			num4 = GetNextVisibleSlot(num4);
		}
		while (MathUtilities.LessThan(num2, displayHeight) && num4 >= 0)
		{
			num4 = GetPreviousVisibleSlot(num);
			if (num4 >= 0)
			{
				num2 += GetExactSlotElementHeight(num4);
				num = num4;
				num3++;
			}
		}
		if (num == 0 && MathUtilities.LessThan(num2, displayHeight))
		{
			double num5 = Math.Max(0.0, NegVerticalOffset - displayHeight + num2);
			num2 += NegVerticalOffset - num5;
			NegVerticalOffset = num5;
		}
		if (MathUtilities.GreaterThan(num2, displayHeight) || (MathUtilities.AreClose(num2, displayHeight) && MathUtilities.GreaterThan(NegVerticalOffset, 0.0)))
		{
			DisplayData.NumTotallyDisplayedScrollingElements = num3 - 1;
		}
		else
		{
			DisplayData.NumTotallyDisplayedScrollingElements = num3;
		}
		if (num3 == 0)
		{
			num = -1;
		}
		RemoveNonDisplayedRows(num, newLastDisplayedSlot);
	}

	private void UpdateDisplayedRowsFromBottom(int newLastDisplayedScrollingRow)
	{
		int num = newLastDisplayedScrollingRow;
		int newFirstDisplayedSlot = -1;
		double cellsHeight = CellsHeight;
		double num2 = 0.0;
		int num3 = 0;
		if (MathUtilities.LessThanOrClose(cellsHeight, 0.0) || SlotCount == 0 || ColumnsItemsInternal.Count == 0)
		{
			ResetDisplayedRows();
			return;
		}
		if (num == -1)
		{
			num = 0;
		}
		int num4 = num;
		while (MathUtilities.LessThan(num2, cellsHeight) && num4 >= 0)
		{
			num2 += GetExactSlotElementHeight(num4);
			num3++;
			newFirstDisplayedSlot = num4;
			num4 = GetPreviousVisibleSlot(num4);
		}
		DisplayData.NumTotallyDisplayedScrollingElements = ((num2 > cellsHeight) ? (num3 - 1) : num3);
		NegVerticalOffset = Math.Max(0.0, num2 - cellsHeight);
		RemoveNonDisplayedRows(newFirstDisplayedSlot, num);
	}

	private void UpdateTablesForRemoval(int slotDeleted, object itemDeleted)
	{
		if (RowGroupHeadersTable.Contains(slotDeleted))
		{
			RowGroupHeadersTable.RemoveIndexAndValue(slotDeleted);
			_collapsedSlotsTable.RemoveIndexAndValue(slotDeleted);
			_selectedItems.DeleteSlot(slotDeleted);
			return;
		}
		if (_selectedItems.ContainsSlot(slotDeleted))
		{
			SelectionHasChanged = true;
		}
		_selectedItems.Delete(slotDeleted, itemDeleted);
		RowGroupHeadersTable.RemoveIndex(slotDeleted);
		_collapsedSlotsTable.RemoveIndex(slotDeleted);
	}

	private void CollectionViewGroup_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (_rowGroupHeightsByLevel != null && DataConnection.CollectionView != null && DataConnection.CollectionView.IsGrouping && DataConnection.CollectionView.GroupingDepth == _rowGroupHeightsByLevel.Length)
		{
			switch (e.Action)
			{
			case NotifyCollectionChangedAction.Add:
				CollectionViewGroup_CollectionChanged_Add(sender, e);
				break;
			case NotifyCollectionChangedAction.Remove:
				CollectionViewGroup_CollectionChanged_Remove(sender, e);
				break;
			}
		}
	}

	private void CollectionViewGroup_CollectionChanged_Add(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.NewItems == null || e.NewItems.Count <= 0)
		{
			return;
		}
		int num = -1;
		DataGridRowGroupInfo parentGroupInfo = GetParentGroupInfo(sender);
		DataGridCollectionViewGroup dataGridCollectionViewGroup = e.NewItems[0] as DataGridCollectionViewGroup;
		if (parentGroupInfo != null)
		{
			if (dataGridCollectionViewGroup != null || parentGroupInfo.Level == -1)
			{
				num = parentGroupInfo.Slot + 1;
				for (int i = 0; i < e.NewStartingIndex; i++)
				{
					DataGridRowGroupInfo valueAt;
					do
					{
						num = RowGroupHeadersTable.GetNextIndex(num);
						valueAt = RowGroupHeadersTable.GetValueAt(num);
					}
					while (valueAt != null && valueAt.Level > parentGroupInfo.Level + 1);
					if (valueAt == null)
					{
						num = SlotCount;
					}
				}
			}
			else
			{
				num = parentGroupInfo.Slot + e.NewStartingIndex + 1;
			}
		}
		if (num == -1)
		{
			return;
		}
		bool isCollapsed = parentGroupInfo != null && (!parentGroupInfo.IsVisible || _collapsedSlotsTable.Contains(parentGroupInfo.Slot));
		if (dataGridCollectionViewGroup != null)
		{
			if (dataGridCollectionViewGroup.Items != null)
			{
				dataGridCollectionViewGroup.Items.CollectionChanged += CollectionViewGroup_CollectionChanged;
			}
			DataGridRowGroupInfo dataGridRowGroupInfo = new DataGridRowGroupInfo(dataGridCollectionViewGroup, isVisible: true, parentGroupInfo.Level + 1, num, num);
			InsertElementAt(num, -1, null, dataGridRowGroupInfo, isCollapsed);
			RowGroupHeadersTable.AddValue(num, dataGridRowGroupInfo);
		}
		else
		{
			int rowIndex = DataConnection.IndexOf(e.NewItems[0]);
			if (SlotCount == 0 && DataConnection.ShouldAutoGenerateColumns)
			{
				AutoGenerateColumnsPrivate();
			}
			InsertElementAt(num, rowIndex, e.NewItems[0], null, isCollapsed);
		}
		CorrectLastSubItemSlotsAfterInsertion(parentGroupInfo);
		if (parentGroupInfo.LastSubItemSlot - parentGroupInfo.Slot == 1)
		{
			EnsureAncestorsExpanderButtonChecked(parentGroupInfo);
		}
	}

	private void CollectionViewGroup_CollectionChanged_Remove(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.OldItems == null || e.OldItems.Count <= 0)
		{
			return;
		}
		if (e.OldItems[0] is DataGridCollectionViewGroup dataGridCollectionViewGroup)
		{
			if (dataGridCollectionViewGroup.Items != null)
			{
				dataGridCollectionViewGroup.Items.CollectionChanged -= CollectionViewGroup_CollectionChanged;
			}
			DataGridRowGroupInfo dataGridRowGroupInfo = RowGroupInfoFromCollectionViewGroup(dataGridCollectionViewGroup);
			if (dataGridRowGroupInfo.Level == _rowGroupHeightsByLevel.Length - 1 && dataGridCollectionViewGroup.Items != null && dataGridCollectionViewGroup.Items.Count > 0)
			{
				for (int i = 0; i < dataGridCollectionViewGroup.Items.Count; i++)
				{
					RemoveElementAt(dataGridRowGroupInfo.Slot + 1, dataGridCollectionViewGroup.Items[i], isRow: true);
				}
			}
			RemoveElementAt(dataGridRowGroupInfo.Slot, null, isRow: false);
		}
		else
		{
			DataGridRowGroupInfo parentGroupInfo = GetParentGroupInfo(sender);
			if (parentGroupInfo != null)
			{
				int slot = ((parentGroupInfo.CollectionViewGroup != null || RowGroupHeadersTable.IndexCount <= 0) ? (parentGroupInfo.Slot + e.OldStartingIndex + 1) : (SlotCount - 1));
				RemoveElementAt(slot, e.OldItems[0], isRow: true);
			}
		}
	}

	private void ClearRowGroupHeadersTable()
	{
		foreach (int index in RowGroupHeadersTable.GetIndexes())
		{
			DataGridRowGroupInfo valueAt = RowGroupHeadersTable.GetValueAt(index);
			if (valueAt.CollectionViewGroup.Items != null)
			{
				valueAt.CollectionViewGroup.Items.CollectionChanged -= CollectionViewGroup_CollectionChanged;
			}
		}
		if (_topLevelGroup != null)
		{
			_topLevelGroup.CollectionChanged -= CollectionViewGroup_CollectionChanged;
			_topLevelGroup = null;
		}
		RowGroupHeadersTable.Clear();
		_collapsedSlotsTable.Clear();
		_rowGroupHeightsByLevel = null;
		RowGroupSublevelIndents = null;
	}

	private void CollectionViewGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == "ItemCount")
		{
			DataGridRowGroupInfo dataGridRowGroupInfo = RowGroupInfoFromCollectionViewGroup(sender as DataGridCollectionViewGroup);
			if (dataGridRowGroupInfo != null && IsSlotVisible(dataGridRowGroupInfo.Slot) && DisplayData.GetDisplayedElement(dataGridRowGroupInfo.Slot) is DataGridRowGroupHeader dataGridRowGroupHeader)
			{
				dataGridRowGroupHeader.UpdateTitleElements();
			}
		}
	}

	private void CorrectLastSubItemSlotsAfterInsertion(DataGridRowGroupInfo subGroupInfo)
	{
		while (subGroupInfo != null)
		{
			int level = subGroupInfo.Level;
			subGroupInfo.LastSubItemSlot++;
			while (subGroupInfo != null && subGroupInfo.Level >= level)
			{
				int previousIndex = RowGroupHeadersTable.GetPreviousIndex(subGroupInfo.Slot);
				subGroupInfo = RowGroupHeadersTable.GetValueAt(previousIndex);
			}
		}
	}

	private int CountAndPopulateGroupHeaders(object group, int rootSlot, int level)
	{
		int num = 1;
		if (group is DataGridCollectionViewGroup dataGridCollectionViewGroup)
		{
			if (dataGridCollectionViewGroup.Items != null && dataGridCollectionViewGroup.Items.Count > 0)
			{
				dataGridCollectionViewGroup.Items.CollectionChanged += CollectionViewGroup_CollectionChanged;
				if (dataGridCollectionViewGroup.Items[0] is DataGridCollectionViewGroup)
				{
					foreach (object item in dataGridCollectionViewGroup.Items)
					{
						num += CountAndPopulateGroupHeaders(item, rootSlot + num, level + 1);
					}
				}
				else
				{
					num += dataGridCollectionViewGroup.Items.Count;
				}
			}
			RowGroupHeadersTable.AddValue(rootSlot, new DataGridRowGroupInfo(dataGridCollectionViewGroup, isVisible: true, level, rootSlot, rootSlot + num - 1));
		}
		return num;
	}

	private void EnsureAncestorsExpanderButtonChecked(DataGridRowGroupInfo parentGroupInfo)
	{
		if (!IsSlotVisible(parentGroupInfo.Slot))
		{
			return;
		}
		DataGridRowGroupHeader dataGridRowGroupHeader = DisplayData.GetDisplayedElement(parentGroupInfo.Slot) as DataGridRowGroupHeader;
		while (dataGridRowGroupHeader != null)
		{
			dataGridRowGroupHeader.EnsureExpanderButtonIsChecked();
			if (dataGridRowGroupHeader.Level > 0)
			{
				int previousIndex = RowGroupHeadersTable.GetPreviousIndex(dataGridRowGroupHeader.RowGroupInfo.Slot);
				if (IsSlotVisible(previousIndex))
				{
					dataGridRowGroupHeader = DisplayData.GetDisplayedElement(previousIndex) as DataGridRowGroupHeader;
					continue;
				}
				break;
			}
			break;
		}
	}

	private void PopulateRowGroupHeadersTable()
	{
		if (DataConnection.CollectionView != null && DataConnection.CollectionView.CanGroup && DataConnection.CollectionView.Groups != null)
		{
			int num = 0;
			_topLevelGroup = DataConnection.CollectionView.Groups;
			_topLevelGroup.CollectionChanged += CollectionViewGroup_CollectionChanged;
			foreach (object group in DataConnection.CollectionView.Groups)
			{
				num += CountAndPopulateGroupHeaders(group, num, 0);
			}
		}
		SlotCount = DataConnection.Count + RowGroupHeadersTable.IndexCount;
		VisibleSlotCount = SlotCount;
	}

	private void RefreshRowGroupHeaders()
	{
		if (DataConnection.CollectionView == null || !DataConnection.CollectionView.CanGroup || DataConnection.CollectionView.Groups == null || !DataConnection.CollectionView.IsGrouping || DataConnection.CollectionView.GroupingDepth <= 0)
		{
			return;
		}
		int groupingDepth = DataConnection.CollectionView.GroupingDepth;
		if (_rowGroupHeightsByLevel == null || _rowGroupHeightsByLevel.Length != groupingDepth)
		{
			_rowGroupHeightsByLevel = new double[groupingDepth];
			for (int i = 0; i < groupingDepth; i++)
			{
				_rowGroupHeightsByLevel[i] = 22.0;
			}
		}
		if (RowGroupSublevelIndents == null || RowGroupSublevelIndents.Length != groupingDepth)
		{
			RowGroupSublevelIndents = new double[groupingDepth];
			for (int j = 0; j < groupingDepth; j++)
			{
				double num = 20.0;
				RowGroupSublevelIndents[j] = num;
				if (j > 0)
				{
					RowGroupSublevelIndents[j] += RowGroupSublevelIndents[j - 1];
				}
			}
		}
		EnsureRowGroupSpacerColumnWidth(groupingDepth);
	}

	private void EnsureRowGroupSpacerColumn()
	{
		if (ColumnsInternal.EnsureRowGrouping(!RowGroupHeadersTable.IsEmpty))
		{
			if (ColumnsInternal.RowGroupSpacerColumn.IsRepresented && CurrentColumnIndex == 0)
			{
				CurrentColumn = ColumnsInternal.FirstVisibleNonFillerColumn;
			}
			ProcessFrozenColumnCount();
		}
	}

	private void EnsureRowGroupSpacerColumnWidth(int groupLevelCount)
	{
		if (groupLevelCount == 0)
		{
			ColumnsInternal.RowGroupSpacerColumn.Width = new DataGridLength(0.0);
		}
		else
		{
			ColumnsInternal.RowGroupSpacerColumn.Width = new DataGridLength(RowGroupSublevelIndents[groupLevelCount - 1]);
		}
	}

	private void EnsureRowGroupVisibility(DataGridRowGroupInfo rowGroupInfo, bool isVisible, bool setCurrent)
	{
		if (rowGroupInfo == null || rowGroupInfo.IsVisible == isVisible)
		{
			return;
		}
		if (IsSlotVisible(rowGroupInfo.Slot))
		{
			(DisplayData.GetDisplayedElement(rowGroupInfo.Slot) as DataGridRowGroupHeader).ToggleExpandCollapse(isVisible, setCurrent);
			return;
		}
		if (_collapsedSlotsTable.Contains(rowGroupInfo.Slot))
		{
			rowGroupInfo.IsVisible = isVisible;
			return;
		}
		if (rowGroupInfo.Slot < DisplayData.FirstScrollingSlot)
		{
			double num = UpdateRowGroupVisibility(rowGroupInfo, isVisible, isDisplayed: false);
			SetVerticalOffset(Math.Max(2.220446049250313E-16, _verticalOffset + num));
		}
		else
		{
			UpdateRowGroupVisibility(rowGroupInfo, isVisible, isDisplayed: false);
		}
		UpdateVerticalScrollBar();
	}

	private int GetRowGroupHeaderCount(int startSlot, int endSlot, bool? isVisible, out double headersHeight)
	{
		int num = 0;
		headersHeight = 0.0;
		foreach (int index in RowGroupHeadersTable.GetIndexes(startSlot))
		{
			if (index > endSlot)
			{
				return num;
			}
			DataGridRowGroupInfo valueAt = RowGroupHeadersTable.GetValueAt(index);
			if (!isVisible.HasValue || (isVisible.Value && !_collapsedSlotsTable.Contains(index)) || (!isVisible.Value && _collapsedSlotsTable.Contains(index)))
			{
				num++;
				headersHeight += _rowGroupHeightsByLevel[valueAt.Level];
			}
		}
		return num;
	}

	private double UpdateRowGroupVisibility(DataGridRowGroupInfo targetRowGroupInfo, bool newIsVisible, bool isDisplayed)
	{
		double totalHeightChange = 0.0;
		int slotsExpanded = 0;
		int num = targetRowGroupInfo.Slot + 1;
		targetRowGroupInfo.IsVisible = newIsVisible;
		if (newIsVisible)
		{
			foreach (int index in RowGroupHeadersTable.GetIndexes(targetRowGroupInfo.Slot + 1))
			{
				if (index >= num)
				{
					DataGridRowGroupInfo valueAt = RowGroupHeadersTable.GetValueAt(index);
					if (valueAt.Level <= targetRowGroupInfo.Level)
					{
						break;
					}
					if (!valueAt.IsVisible)
					{
						int slot = valueAt.Slot;
						ExpandSlots(num, slot, isDisplayed, ref slotsExpanded, ref totalHeightChange);
						num = valueAt.LastSubItemSlot + 1;
					}
				}
			}
			if (targetRowGroupInfo.LastSubItemSlot >= num)
			{
				ExpandSlots(num, targetRowGroupInfo.LastSubItemSlot, isDisplayed, ref slotsExpanded, ref totalHeightChange);
			}
			if (isDisplayed)
			{
				UpdateDisplayedRows(DisplayData.FirstScrollingSlot, CellsHeight);
			}
		}
		else
		{
			int slot = SlotCount - 1;
			foreach (int index2 in RowGroupHeadersTable.GetIndexes(targetRowGroupInfo.Slot + 1))
			{
				if (RowGroupHeadersTable.GetValueAt(index2).Level <= targetRowGroupInfo.Level)
				{
					slot = index2 - 1;
					break;
				}
			}
			int lastScrollingSlot = DisplayData.LastScrollingSlot;
			int num2 = Math.Min(slot, DisplayData.LastScrollingSlot);
			if (isDisplayed)
			{
				int num3 = num2 - num + 1 - _collapsedSlotsTable.GetIndexCount(num, num2);
				if (_focusedRow != null && _focusedRow.Slot >= num && _focusedRow.Slot <= slot)
				{
					_focusedRow = null;
				}
				for (int i = 0; i < num3; i++)
				{
					RemoveDisplayedElement(num, wasDeleted: false, updateSlotInformation: false);
				}
			}
			double heightChangeBelowLastDisplayedSlot = 0.0;
			if (DisplayData.FirstScrollingSlot >= num && DisplayData.FirstScrollingSlot <= slot)
			{
				int num4 = DisplayData.FirstScrollingSlot - num - _collapsedSlotsTable.GetIndexCount(num, DisplayData.FirstScrollingSlot);
				int nextVisibleSlot = GetNextVisibleSlot(DisplayData.FirstScrollingSlot);
				while (num4 > 1 && nextVisibleSlot < SlotCount)
				{
					num4--;
					nextVisibleSlot = GetNextVisibleSlot(nextVisibleSlot);
				}
				totalHeightChange += CollapseSlotsInTable(num, slot, ref slotsExpanded, lastScrollingSlot, ref heightChangeBelowLastDisplayedSlot);
				if (isDisplayed)
				{
					if (nextVisibleSlot >= SlotCount)
					{
						UpdateDisplayedRowsFromBottom(targetRowGroupInfo.Slot);
					}
					else
					{
						UpdateDisplayedRows(nextVisibleSlot, CellsHeight);
					}
				}
			}
			else
			{
				totalHeightChange += CollapseSlotsInTable(num, slot, ref slotsExpanded, lastScrollingSlot, ref heightChangeBelowLastDisplayedSlot);
			}
			if (DisplayData.LastScrollingSlot >= num && DisplayData.LastScrollingSlot <= slot)
			{
				DisplayData.LastScrollingSlot = GetPreviousVisibleSlot(DisplayData.LastScrollingSlot);
			}
			if (isDisplayed && _verticalOffset > 0.0)
			{
				int previousVisibleSlot = GetPreviousVisibleSlot(SlotCount);
				int nextVisibleSlot2 = GetNextVisibleSlot(lastScrollingSlot);
				double num5 = AvailableSlotElementRoom + heightChangeBelowLastDisplayedSlot;
				while (num5 > totalHeightChange && nextVisibleSlot2 < previousVisibleSlot)
				{
					num5 -= GetSlotElementHeight(nextVisibleSlot2);
					nextVisibleSlot2 = GetNextVisibleSlot(nextVisibleSlot2);
				}
				if (num5 > totalHeightChange)
				{
					double num6 = _verticalOffset + totalHeightChange - num5;
					if (num6 > 0.0)
					{
						SetVerticalOffset(num6);
					}
					else
					{
						ResetDisplayedRows();
						NegVerticalOffset = 0.0;
						SetVerticalOffset(0.0);
						int nextVisibleSlot3 = GetNextVisibleSlot(-1);
						UpdateDisplayedRows(nextVisibleSlot3, CellsHeight);
					}
				}
			}
		}
		VisibleSlotCount += slotsExpanded;
		return totalHeightChange;
	}

	private DataGridRowGroupHeader GenerateRowGroupHeader(int slot, DataGridRowGroupInfo rowGroupInfo)
	{
		DataGridRowGroupHeader dataGridRowGroupHeader = DisplayData.GetUsedGroupHeader() ?? new DataGridRowGroupHeader();
		dataGridRowGroupHeader.OwningGrid = this;
		dataGridRowGroupHeader.RowGroupInfo = rowGroupInfo;
		dataGridRowGroupHeader.DataContext = rowGroupInfo.CollectionViewGroup;
		dataGridRowGroupHeader.Level = rowGroupInfo.Level;
		ControlTheme rowGroupTheme = RowGroupTheme;
		if (rowGroupTheme != null)
		{
			dataGridRowGroupHeader.SetValue(StyledElement.ThemeProperty, rowGroupTheme, BindingPriority.Template);
		}
		string groupingPropertyNameAtDepth = DataConnection.CollectionView.GetGroupingPropertyNameAtDepth(dataGridRowGroupHeader.Level);
		if (string.IsNullOrWhiteSpace(groupingPropertyNameAtDepth))
		{
			dataGridRowGroupHeader.PropertyName = null;
		}
		else
		{
			dataGridRowGroupHeader.PropertyName = DataConnection.DataType?.GetDisplayName(groupingPropertyNameAtDepth) ?? groupingPropertyNameAtDepth;
		}
		INotifyPropertyChanged collectionViewGroup = rowGroupInfo.CollectionViewGroup;
		if (collectionViewGroup != null)
		{
			collectionViewGroup.PropertyChanged -= CollectionViewGroup_PropertyChanged;
			collectionViewGroup.PropertyChanged += CollectionViewGroup_PropertyChanged;
		}
		dataGridRowGroupHeader.UpdateTitleElements();
		OnLoadingRowGroup(new DataGridRowGroupHeaderEventArgs(dataGridRowGroupHeader));
		return dataGridRowGroupHeader;
	}

	private DataGridRowGroupInfo GetParentGroupInfo(object collection)
	{
		if (collection == DataConnection.CollectionView.Groups)
		{
			return new DataGridRowGroupInfo(null, isVisible: true, -1, -1, -1);
		}
		foreach (int index in RowGroupHeadersTable.GetIndexes())
		{
			DataGridRowGroupInfo valueAt = RowGroupHeadersTable.GetValueAt(index);
			if (valueAt.CollectionViewGroup.Items == collection)
			{
				return valueAt;
			}
		}
		return null;
	}

	internal void OnRowGroupHeaderToggled(DataGridRowGroupHeader groupHeader, bool newIsVisible, bool setCurrent)
	{
		if (!WaitForLostFocus(delegate
		{
			OnRowGroupHeaderToggled(groupHeader, newIsVisible, setCurrent);
		}) && CommitEdit())
		{
			if (setCurrent && CurrentSlot != groupHeader.RowGroupInfo.Slot)
			{
				UpdateSelectionAndCurrency(CurrentColumnIndex, groupHeader.RowGroupInfo.Slot, DataGridSelectionAction.SelectCurrent, scrollIntoView: false);
			}
			UpdateRowGroupVisibility(groupHeader.RowGroupInfo, newIsVisible, isDisplayed: true);
			ComputeScrollBarsLayout();
			InvalidateRowsArrange();
		}
	}

	internal void OnSublevelIndentUpdated(DataGridRowGroupHeader groupHeader, double newValue)
	{
		int groupingDepth = DataConnection.CollectionView.GroupingDepth;
		double num = RowGroupSublevelIndents[groupHeader.Level];
		if (groupHeader.Level > 0)
		{
			num -= RowGroupSublevelIndents[groupHeader.Level - 1];
		}
		double num2 = newValue - num;
		for (int i = groupHeader.Level; i < groupingDepth; i++)
		{
			RowGroupSublevelIndents[i] += num2;
		}
		EnsureRowGroupSpacerColumnWidth(groupingDepth);
	}

	internal DataGridRowGroupInfo RowGroupInfoFromCollectionViewGroup(DataGridCollectionViewGroup collectionViewGroup)
	{
		foreach (int index in RowGroupHeadersTable.GetIndexes())
		{
			DataGridRowGroupInfo valueAt = RowGroupHeadersTable.GetValueAt(index);
			if (valueAt.CollectionViewGroup == collectionViewGroup)
			{
				return valueAt;
			}
		}
		return null;
	}

	public void CollapseRowGroup(DataGridCollectionViewGroup collectionViewGroup, bool collapseAllSubgroups)
	{
		if (WaitForLostFocus(delegate
		{
			CollapseRowGroup(collectionViewGroup, collapseAllSubgroups);
		}) || collectionViewGroup == null || !CommitEdit())
		{
			return;
		}
		EnsureRowGroupVisibility(RowGroupInfoFromCollectionViewGroup(collectionViewGroup), isVisible: false, setCurrent: true);
		if (!collapseAllSubgroups)
		{
			return;
		}
		foreach (object item in collectionViewGroup.Items)
		{
			if (item is DataGridCollectionViewGroup collectionViewGroup2)
			{
				CollapseRowGroup(collectionViewGroup2, collapseAllSubgroups);
			}
		}
	}

	public void ExpandRowGroup(DataGridCollectionViewGroup collectionViewGroup, bool expandAllSubgroups)
	{
		if ((WaitForLostFocus(delegate
		{
			ExpandRowGroup(collectionViewGroup, expandAllSubgroups);
		}) || collectionViewGroup == null || !CommitEdit()) && (collectionViewGroup == null || !CommitEdit()))
		{
			return;
		}
		EnsureRowGroupVisibility(RowGroupInfoFromCollectionViewGroup(collectionViewGroup), isVisible: true, setCurrent: true);
		if (!expandAllSubgroups)
		{
			return;
		}
		foreach (object item in collectionViewGroup.Items)
		{
			if (item is DataGridCollectionViewGroup collectionViewGroup2)
			{
				ExpandRowGroup(collectionViewGroup2, expandAllSubgroups);
			}
		}
	}

	private int GetDetailsCountInclusive(int lowerBound, int upperBound)
	{
		int num = upperBound - lowerBound + 1;
		if (num <= 0)
		{
			return 0;
		}
		if (RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Visible)
		{
			return num - _showDetailsTable.GetIndexCount(lowerBound, upperBound, value: false) - RowGroupHeadersTable.GetIndexCount(lowerBound, upperBound);
		}
		if (RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Collapsed)
		{
			return _showDetailsTable.GetIndexCount(lowerBound, upperBound, value: true);
		}
		if (RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
		{
			return _selectedItems.GetIndexCount(lowerBound, upperBound);
		}
		return 0;
	}

	private void EnsureRowDetailsVisibility(DataGridRow row, bool raiseNotification, bool animate)
	{
		row.SetDetailsVisibilityInternal(GetRowDetailsVisibility(row.Index), raiseNotification, animate);
	}

	private void UpdateRowDetailsHeightEstimate()
	{
		if (_rowsPresenter == null || !_measured || RowDetailsTemplate == null)
		{
			return;
		}
		object obj = null;
		if (VisibleSlotCount > 0)
		{
			obj = DataConnection.GetDataItem(0);
		}
		Control control = RowDetailsTemplate.Build(obj);
		if (control != null)
		{
			_rowsPresenter.Children.Add(control);
			if (obj != null)
			{
				control.DataContext = obj;
			}
			control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			RowDetailsHeightEstimate = control.DesiredSize.Height;
			_rowsPresenter.Children.Remove(control);
		}
	}

	internal void OnUnloadingRowDetails(DataGridRow row, Control detailsElement)
	{
		OnUnloadingRowDetails(new DataGridRowDetailsEventArgs(row, detailsElement));
	}

	internal void OnLoadingRowDetails(DataGridRow row, Control detailsElement)
	{
		OnLoadingRowDetails(new DataGridRowDetailsEventArgs(row, detailsElement));
	}

	internal void OnRowDetailsVisibilityPropertyChanged(int rowIndex, bool isVisible)
	{
		_showDetailsTable.AddValue(rowIndex, isVisible);
	}

	internal bool GetRowDetailsVisibility(int rowIndex)
	{
		return GetRowDetailsVisibility(rowIndex, RowDetailsVisibilityMode);
	}

	internal bool GetRowDetailsVisibility(int rowIndex, DataGridRowDetailsVisibilityMode gridLevelRowDetailsVisibility)
	{
		if (_showDetailsTable.Contains(rowIndex))
		{
			return _showDetailsTable.GetValueAt(rowIndex);
		}
		return gridLevelRowDetailsVisibility switch
		{
			DataGridRowDetailsVisibilityMode.VisibleWhenSelected => _selectedItems.ContainsSlot(SlotFromRowIndex(rowIndex)), 
			DataGridRowDetailsVisibilityMode.Visible => true, 
			_ => false, 
		};
	}

	protected internal virtual void OnRowDetailsVisibilityChanged(DataGridRowDetailsEventArgs e)
	{
		this.RowDetailsVisibilityChanged?.Invoke(this, e);
	}
}
