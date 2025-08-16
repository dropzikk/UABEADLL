using System;
using System.Collections.Generic;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Reactive;
using Avalonia.Styling;
using Avalonia.Utilities;
using Avalonia.VisualTree;

namespace Avalonia.Controls;

[TemplatePart("PART_BottomGridLine", typeof(Rectangle))]
[TemplatePart("PART_CellsPresenter", typeof(DataGridCellsPresenter))]
[TemplatePart("PART_DetailsPresenter", typeof(DataGridDetailsPresenter))]
[TemplatePart("PART_Root", typeof(Panel))]
[TemplatePart("PART_RowHeader", typeof(DataGridRowHeader))]
[PseudoClasses(new string[] { ":selected", ":editing", ":invalid" })]
public class DataGridRow : TemplatedControl
{
	private const byte DATAGRIDROW_defaultMinHeight = 0;

	internal const int DATAGRIDROW_maximumHeight = 65536;

	internal const double DATAGRIDROW_minimumHeight = 0.0;

	private const string DATAGRIDROW_elementBottomGridLine = "PART_BottomGridLine";

	private const string DATAGRIDROW_elementCells = "PART_CellsPresenter";

	private const string DATAGRIDROW_elementDetails = "PART_DetailsPresenter";

	internal const string DATAGRIDROW_elementRoot = "PART_Root";

	internal const string DATAGRIDROW_elementRowHeader = "PART_RowHeader";

	private DataGridCellsPresenter _cellsElement;

	private DataGridCell _fillerCell;

	private DataGridRowHeader _headerElement;

	private double _lastHorizontalOffset;

	private int? _mouseOverColumnIndex;

	private bool _isValid = true;

	private Rectangle _bottomGridLine;

	private bool _areHandlersSuspended;

	private bool _checkDetailsContentHeight;

	private double _detailsDesiredHeight;

	private bool _detailsLoaded;

	private bool _detailsVisibilityNotificationPending;

	private Control _detailsContent;

	private IDisposable _detailsContentSizeSubscription;

	private DataGridDetailsPresenter _detailsElement;

	private IDataTemplate _appliedDetailsTemplate;

	private bool? _appliedDetailsVisibility;

	public static readonly StyledProperty<object> HeaderProperty;

	public static readonly DirectProperty<DataGridRow, bool> IsValidProperty;

	public static readonly StyledProperty<IDataTemplate> DetailsTemplateProperty;

	public static readonly StyledProperty<bool> AreDetailsVisibleProperty;

	private double? _previousDetailsHeight;

	public object Header
	{
		get
		{
			return GetValue(HeaderProperty);
		}
		set
		{
			SetValue(HeaderProperty, value);
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
		}
	}

	public IDataTemplate DetailsTemplate
	{
		get
		{
			return GetValue(DetailsTemplateProperty);
		}
		set
		{
			SetValue(DetailsTemplateProperty, value);
		}
	}

	public bool AreDetailsVisible
	{
		get
		{
			return GetValue(AreDetailsVisibleProperty);
		}
		set
		{
			SetValue(AreDetailsVisibleProperty, value);
		}
	}

	internal DataGrid OwningGrid { get; set; }

	internal int Index { get; set; }

	internal double ActualBottomGridLineHeight
	{
		get
		{
			if (_bottomGridLine != null && OwningGrid != null && OwningGrid.AreRowBottomGridLinesRequired)
			{
				return DataGrid.HorizontalGridLinesThickness;
			}
			return 0.0;
		}
	}

	internal DataGridCellCollection Cells { get; private set; }

	internal DataGridCell FillerCell
	{
		get
		{
			if (_fillerCell == null)
			{
				_fillerCell = new DataGridCell
				{
					IsVisible = false,
					OwningRow = this
				};
				ControlTheme cellTheme = OwningGrid.CellTheme;
				if (cellTheme != null)
				{
					_fillerCell.SetValue(StyledElement.ThemeProperty, cellTheme, BindingPriority.Template);
				}
				if (_cellsElement != null)
				{
					_cellsElement.Children.Add(_fillerCell);
				}
			}
			return _fillerCell;
		}
	}

	internal bool HasBottomGridLine => _bottomGridLine != null;

	internal bool HasHeaderCell => _headerElement != null;

	internal DataGridRowHeader HeaderCell => _headerElement;

	internal bool IsEditing
	{
		get
		{
			if (OwningGrid != null)
			{
				return OwningGrid.EditingRow == this;
			}
			return false;
		}
	}

	internal bool IsLayoutDelayed { get; private set; }

	internal bool IsMouseOver
	{
		get
		{
			if (OwningGrid != null)
			{
				return OwningGrid.MouseOverRowIndex == Index;
			}
			return false;
		}
		set
		{
			if (OwningGrid != null && value != IsMouseOver)
			{
				if (value)
				{
					OwningGrid.MouseOverRowIndex = Index;
				}
				else
				{
					OwningGrid.MouseOverRowIndex = null;
				}
			}
		}
	}

	internal bool IsRecycled { get; private set; }

	internal bool IsRecyclable
	{
		get
		{
			if (OwningGrid != null)
			{
				return OwningGrid.IsRowRecyclable(this);
			}
			return true;
		}
	}

	internal bool IsSelected
	{
		get
		{
			if (OwningGrid == null || Slot == -1)
			{
				return false;
			}
			return OwningGrid.GetRowSelection(Slot);
		}
	}

	internal int? MouseOverColumnIndex
	{
		get
		{
			return _mouseOverColumnIndex;
		}
		set
		{
			if (_mouseOverColumnIndex != value)
			{
				DataGridCell dataGridCell = null;
				if (_mouseOverColumnIndex.HasValue && OwningGrid.IsSlotVisible(Slot) && _mouseOverColumnIndex > -1)
				{
					dataGridCell = Cells[_mouseOverColumnIndex.Value];
				}
				_mouseOverColumnIndex = value;
				if (dataGridCell != null && base.IsVisible)
				{
					dataGridCell.UpdatePseudoClasses();
				}
				if (_mouseOverColumnIndex.HasValue && OwningGrid != null && OwningGrid.IsSlotVisible(Slot) && _mouseOverColumnIndex > -1)
				{
					Cells[_mouseOverColumnIndex.Value].UpdatePseudoClasses();
				}
			}
		}
	}

	internal Panel RootElement { get; private set; }

	internal int Slot { get; set; }

	internal double TargetHeight
	{
		get
		{
			if (!double.IsNaN(base.Height))
			{
				return base.Height;
			}
			if (_detailsElement != null && _appliedDetailsVisibility == true && _appliedDetailsTemplate != null)
			{
				return base.DesiredSize.Height + _detailsDesiredHeight - _detailsElement.ContentHeight;
			}
			return base.DesiredSize.Height;
		}
	}

	private IDataTemplate ActualDetailsTemplate => DetailsTemplate ?? OwningGrid.RowDetailsTemplate;

	private bool ActualDetailsVisibility
	{
		get
		{
			if (OwningGrid == null)
			{
				throw DataGridError.DataGrid.NoOwningGrid(GetType());
			}
			if (Index == -1)
			{
				throw DataGridError.DataGridRow.InvalidRowIndexCannotCompleteOperation();
			}
			return OwningGrid.GetRowDetailsVisibility(Index);
		}
	}

	static DataGridRow()
	{
		HeaderProperty = AvaloniaProperty.Register<DataGridRow, object>("Header");
		IsValidProperty = AvaloniaProperty.RegisterDirect("IsValid", (DataGridRow o) => o.IsValid, null, unsetValue: false);
		DetailsTemplateProperty = AvaloniaProperty.Register<DataGridRow, IDataTemplate>("DetailsTemplate");
		AreDetailsVisibleProperty = AvaloniaProperty.Register<DataGridRow, bool>("AreDetailsVisible", defaultValue: false);
		HeaderProperty.Changed.AddClassHandler(delegate(DataGridRow x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnHeaderChanged(e);
		});
		DetailsTemplateProperty.Changed.AddClassHandler(delegate(DataGridRow x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnDetailsTemplateChanged(e);
		});
		AreDetailsVisibleProperty.Changed.AddClassHandler(delegate(DataGridRow x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnAreDetailsVisibleChanged(e);
		});
		InputElement.PointerPressedEvent.AddClassHandler(delegate(DataGridRow x, PointerPressedEventArgs e)
		{
			x.DataGridRow_PointerPressed(e);
		}, RoutingStrategies.Direct | RoutingStrategies.Bubble, handledEventsToo: true);
		InputElement.IsTabStopProperty.OverrideDefaultValue<DataGridRow>(defaultValue: false);
	}

	public DataGridRow()
	{
		base.MinHeight = 0.0;
		Index = -1;
		IsValid = true;
		Slot = -1;
		_mouseOverColumnIndex = null;
		_detailsDesiredHeight = double.NaN;
		_detailsLoaded = false;
		_appliedDetailsVisibility = false;
		Cells = new DataGridCellCollection(this);
		Cells.CellAdded += DataGridCellCollection_CellAdded;
		Cells.CellRemoved += DataGridCellCollection_CellRemoved;
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

	private void OnHeaderChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (_headerElement != null)
		{
			_headerElement.Content = e.NewValue;
		}
	}

	private void OnDetailsTemplateChanged(AvaloniaPropertyChangedEventArgs e)
	{
		IDataTemplate template2 = (IDataTemplate)e.OldValue;
		IDataTemplate template3 = (IDataTemplate)e.NewValue;
		if (!_areHandlersSuspended && OwningGrid != null && actualDetailsTemplate(template3) != actualDetailsTemplate(template2))
		{
			ApplyDetailsTemplate(initializeDetailsPreferredHeight: false);
		}
		IDataTemplate actualDetailsTemplate(IDataTemplate template)
		{
			return template ?? OwningGrid.RowDetailsTemplate;
		}
	}

	private void OnAreDetailsVisibleChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (!_areHandlersSuspended)
		{
			if (OwningGrid == null)
			{
				throw DataGridError.DataGrid.NoOwningGrid(GetType());
			}
			if (Index == -1)
			{
				throw DataGridError.DataGridRow.InvalidRowIndexCannotCompleteOperation();
			}
			bool isVisible = (bool)e.NewValue;
			OwningGrid.OnRowDetailsVisibilityPropertyChanged(Index, isVisible);
			SetDetailsVisibilityInternal(isVisible, raiseNotification: true, animate: true);
		}
	}

	public int GetIndex()
	{
		return Index;
	}

	public static DataGridRow GetRowContainingElement(Control element)
	{
		Visual visual = element;
		DataGridRow dataGridRow = visual as DataGridRow;
		while (visual != null && dataGridRow == null)
		{
			visual = visual.GetVisualParent();
			dataGridRow = visual as DataGridRow;
		}
		return dataGridRow;
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		if (OwningGrid == null)
		{
			return base.ArrangeOverride(finalSize);
		}
		if (_lastHorizontalOffset != OwningGrid.HorizontalOffset)
		{
			_lastHorizontalOffset = OwningGrid.HorizontalOffset;
			InvalidateHorizontalArrange();
		}
		Size result = base.ArrangeOverride(finalSize);
		if (_checkDetailsContentHeight)
		{
			_checkDetailsContentHeight = false;
			EnsureDetailsContentHeight();
		}
		if (RootElement != null)
		{
			foreach (Control child in RootElement.Children)
			{
				if (DataGridFrozenGrid.GetIsFrozen(child))
				{
					TranslateTransform translateTransform = new TranslateTransform();
					translateTransform.X = Math.Round(OwningGrid.HorizontalOffset);
					child.RenderTransform = translateTransform;
				}
			}
		}
		if (_bottomGridLine != null)
		{
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			rectangleGeometry.Rect = new Rect(OwningGrid.HorizontalOffset, 0.0, Math.Max(0.0, base.DesiredSize.Width - OwningGrid.HorizontalOffset), _bottomGridLine.DesiredSize.Height);
			_bottomGridLine.Clip = rectangleGeometry;
		}
		return result;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		if (OwningGrid == null)
		{
			return base.MeasureOverride(availableSize);
		}
		if (_headerElement != null)
		{
			_headerElement.InvalidateMeasure();
		}
		if (_cellsElement != null)
		{
			_cellsElement.InvalidateMeasure();
		}
		if (_detailsElement != null)
		{
			_detailsElement.InvalidateMeasure();
		}
		Size size = base.MeasureOverride(availableSize);
		return size.WithWidth(Math.Max(size.Width, OwningGrid.CellsWidth));
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		RootElement = e.NameScope.Find<Panel>("PART_Root");
		if (RootElement != null)
		{
			UpdatePseudoClasses();
		}
		bool flag = false;
		if (_cellsElement != null)
		{
			_cellsElement.Children.Clear();
			flag = true;
		}
		_cellsElement = e.NameScope.Find<DataGridCellsPresenter>("PART_CellsPresenter");
		if (_cellsElement != null)
		{
			_cellsElement.OwningRow = this;
			if (Cells.Count > 0)
			{
				foreach (DataGridCell cell in Cells)
				{
					_cellsElement.Children.Add(cell);
				}
			}
		}
		_detailsElement = e.NameScope.Find<DataGridDetailsPresenter>("PART_DetailsPresenter");
		if (_detailsElement != null && OwningGrid != null)
		{
			_detailsElement.OwningRow = this;
			if (ActualDetailsVisibility && ActualDetailsTemplate != null && _appliedDetailsTemplate == null)
			{
				SetDetailsVisibilityInternal(ActualDetailsVisibility, _detailsVisibilityNotificationPending, animate: false);
				_detailsVisibilityNotificationPending = false;
			}
		}
		_bottomGridLine = e.NameScope.Find<Rectangle>("PART_BottomGridLine");
		EnsureGridLines();
		_headerElement = e.NameScope.Find<DataGridRowHeader>("PART_RowHeader");
		if (_headerElement != null)
		{
			_headerElement.Owner = this;
			if (Header != null)
			{
				_headerElement.Content = Header;
			}
			EnsureHeaderStyleAndVisibility(null);
		}
		if (OwningGrid != null && flag)
		{
			OwningGrid.UpdateVerticalScrollBar();
		}
	}

	protected override void OnPointerEntered(PointerEventArgs e)
	{
		base.OnPointerEntered(e);
		IsMouseOver = true;
	}

	protected override void OnPointerExited(PointerEventArgs e)
	{
		IsMouseOver = false;
		base.OnPointerExited(e);
	}

	internal void ApplyCellsState()
	{
		foreach (DataGridCell cell in Cells)
		{
			cell.UpdatePseudoClasses();
		}
	}

	internal void ApplyHeaderStatus()
	{
		if (_headerElement != null && OwningGrid.AreRowHeadersVisible)
		{
			_headerElement.UpdatePseudoClasses();
		}
	}

	internal void UpdatePseudoClasses()
	{
		if (RootElement != null && OwningGrid != null && base.IsVisible)
		{
			base.PseudoClasses.Set(":selected", IsSelected);
			base.PseudoClasses.Set(":editing", IsEditing);
			base.PseudoClasses.Set(":invalid", !IsValid);
			ApplyHeaderStatus();
		}
	}

	internal void DetachFromDataGrid(bool recycle)
	{
		UnloadDetailsTemplate(recycle);
		if (recycle)
		{
			IsRecycled = true;
			if (_cellsElement != null)
			{
				_cellsElement.Recycle();
			}
			_checkDetailsContentHeight = false;
			if (_detailsElement != null)
			{
				_detailsElement.ClearValue(DataGridDetailsPresenter.ContentHeightProperty);
			}
		}
		Slot = -1;
	}

	internal void InvalidateCellsIndex()
	{
		_cellsElement?.InvalidateChildIndex();
	}

	internal void EnsureFillerVisibility()
	{
		if (_cellsElement != null)
		{
			_cellsElement.EnsureFillerVisibility();
		}
	}

	internal void EnsureGridLines()
	{
		if (OwningGrid == null)
		{
			return;
		}
		if (_bottomGridLine != null)
		{
			bool flag = OwningGrid.GridLinesVisibility == DataGridGridLinesVisibility.Horizontal || OwningGrid.GridLinesVisibility == DataGridGridLinesVisibility.All;
			if (flag != _bottomGridLine.IsVisible)
			{
				_bottomGridLine.IsVisible = flag;
			}
			_bottomGridLine.Fill = OwningGrid.HorizontalGridLinesBrush;
		}
		foreach (DataGridCell cell in Cells)
		{
			cell.EnsureGridLine(OwningGrid.ColumnsInternal.LastVisibleColumn);
		}
	}

	internal void EnsureHeaderStyleAndVisibility(Style previousStyle)
	{
		if (_headerElement != null && OwningGrid != null)
		{
			_headerElement.IsVisible = OwningGrid.AreRowHeadersVisible;
		}
	}

	internal void EnsureHeaderVisibility()
	{
		if (_headerElement != null && OwningGrid != null)
		{
			_headerElement.IsVisible = OwningGrid.AreRowHeadersVisible;
		}
	}

	internal void InvalidateHorizontalArrange()
	{
		if (_cellsElement != null)
		{
			_cellsElement.InvalidateArrange();
		}
		if (_detailsElement != null)
		{
			_detailsElement.InvalidateArrange();
		}
	}

	internal void InvalidateDesiredHeight()
	{
		_cellsElement?.InvalidateDesiredHeight();
	}

	internal void ResetGridLine()
	{
		_bottomGridLine = null;
	}

	private void DataGridCellCollection_CellAdded(object sender, DataGridCellEventArgs e)
	{
		_cellsElement?.Children.Add(e.Cell);
	}

	private void DataGridCellCollection_CellRemoved(object sender, DataGridCellEventArgs e)
	{
		_cellsElement?.Children.Remove(e.Cell);
	}

	private void DataGridRow_PointerPressed(PointerPressedEventArgs e)
	{
		if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && OwningGrid != null)
		{
			OwningGrid.IsDoubleClickRecordsClickOnCall(this);
			if (OwningGrid.UpdatedStateOnMouseLeftButtonDown)
			{
				OwningGrid.UpdatedStateOnMouseLeftButtonDown = false;
			}
			else
			{
				e.Handled = OwningGrid.UpdateStateOnMouseLeftButtonDown(e, -1, Slot, allowEdit: false);
			}
		}
	}

	private void OnRowDetailsChanged()
	{
		OwningGrid?.OnRowDetailsChanged();
	}

	private void UnloadDetailsTemplate(bool recycle)
	{
		if (_detailsElement != null)
		{
			if (_detailsContent != null)
			{
				if (_detailsLoaded)
				{
					OwningGrid.OnUnloadingRowDetails(this, _detailsContent);
				}
				_detailsContent.DataContext = null;
				if (!recycle)
				{
					_detailsContentSizeSubscription?.Dispose();
					_detailsContentSizeSubscription = null;
					_detailsContent = null;
				}
			}
			if (!recycle)
			{
				_detailsElement.Children.Clear();
			}
			_detailsElement.ContentHeight = 0.0;
		}
		if (!recycle)
		{
			_appliedDetailsTemplate = null;
			SetValueNoCallback(DetailsTemplateProperty, null);
		}
		_detailsLoaded = false;
		_appliedDetailsVisibility = null;
		SetValueNoCallback(AreDetailsVisibleProperty, value: false);
	}

	internal void EnsureDetailsContentHeight()
	{
		if (_detailsElement != null && _detailsContent != null && double.IsNaN(_detailsContent.Height) && AreDetailsVisible && !double.IsNaN(_detailsDesiredHeight) && !MathUtilities.AreClose(_detailsContent.Bounds.Inflate(_detailsContent.Margin).Height, _detailsDesiredHeight) && Slot != -1)
		{
			_detailsDesiredHeight = _detailsContent.Bounds.Inflate(_detailsContent.Margin).Height;
			_detailsElement.ContentHeight = _detailsDesiredHeight;
		}
	}

	private void EnsureDetailsDesiredHeight()
	{
		if (_detailsContent != null)
		{
			_detailsContent.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			_detailsDesiredHeight = _detailsContent.DesiredSize.Height;
		}
		else
		{
			_detailsDesiredHeight = 0.0;
		}
	}

	private void DetailsContent_HeightChanged(double newValue)
	{
		if (_previousDetailsHeight.HasValue)
		{
			double value = _previousDetailsHeight.Value;
			_previousDetailsHeight = newValue;
			if (newValue != value && newValue != _detailsDesiredHeight && AreDetailsVisible && _appliedDetailsTemplate != null)
			{
				_detailsDesiredHeight = newValue;
				_detailsElement.ContentHeight = newValue;
				OnRowDetailsChanged();
			}
		}
		else
		{
			_previousDetailsHeight = newValue;
		}
	}

	private void DetailsContent_SizeChanged(Rect newValue)
	{
		DetailsContent_HeightChanged(newValue.Height);
	}

	private void DetailsContent_MarginChanged(Thickness newValue)
	{
		if (_detailsContent != null)
		{
			DetailsContent_SizeChanged(_detailsContent.Bounds.Inflate(newValue));
		}
	}

	private void DetailsContent_LayoutUpdated(object sender, EventArgs e)
	{
		if (_detailsContent != null)
		{
			Thickness margin = _detailsContent.Margin;
			double newValue = _detailsContent.DesiredSize.Height + margin.Top + margin.Bottom;
			DetailsContent_HeightChanged(newValue);
		}
	}

	internal void SetDetailsVisibilityInternal(bool isVisible, bool raiseNotification, bool animate)
	{
		if (_appliedDetailsVisibility == isVisible)
		{
			return;
		}
		if (_detailsElement == null)
		{
			if (raiseNotification)
			{
				_detailsVisibilityNotificationPending = true;
			}
			return;
		}
		_appliedDetailsVisibility = isVisible;
		SetValueNoCallback(AreDetailsVisibleProperty, isVisible);
		ApplyDetailsTemplate(initializeDetailsPreferredHeight: true);
		if (_appliedDetailsTemplate == null)
		{
			if (_detailsElement.ContentHeight > 0.0)
			{
				_detailsElement.ContentHeight = 0.0;
			}
			return;
		}
		if (AreDetailsVisible)
		{
			_detailsElement.ContentHeight = _detailsDesiredHeight;
			_checkDetailsContentHeight = true;
		}
		else
		{
			_detailsElement.ContentHeight = 0.0;
		}
		OnRowDetailsChanged();
		if (raiseNotification)
		{
			OwningGrid.OnRowDetailsVisibilityChanged(new DataGridRowDetailsEventArgs(this, _detailsContent));
		}
	}

	internal void ApplyDetailsTemplate(bool initializeDetailsPreferredHeight)
	{
		if (_detailsElement == null || !AreDetailsVisible)
		{
			return;
		}
		IDataTemplate appliedDetailsTemplate = _appliedDetailsTemplate;
		if (ActualDetailsTemplate != null && ActualDetailsTemplate != _appliedDetailsTemplate)
		{
			if (_detailsContent != null)
			{
				_detailsContentSizeSubscription?.Dispose();
				_detailsContentSizeSubscription = null;
				if (_detailsLoaded)
				{
					OwningGrid.OnUnloadingRowDetails(this, _detailsContent);
					_detailsLoaded = false;
				}
			}
			_detailsElement.Children.Clear();
			_detailsContent = ActualDetailsTemplate.Build(base.DataContext);
			_appliedDetailsTemplate = ActualDetailsTemplate;
			if (_detailsContent != null)
			{
				Control detailsContent = _detailsContent;
				Layoutable layoutableContent = detailsContent;
				if (layoutableContent != null)
				{
					layoutableContent.LayoutUpdated += DetailsContent_LayoutUpdated;
					_detailsContentSizeSubscription = new CompositeDisposable(2)
					{
						Disposable.Create(delegate
						{
							layoutableContent.LayoutUpdated -= DetailsContent_LayoutUpdated;
						}),
						_detailsContent.GetObservable(Layoutable.MarginProperty).Subscribe(DetailsContent_MarginChanged)
					};
				}
				else
				{
					_detailsContentSizeSubscription = _detailsContent.GetObservable(Layoutable.MarginProperty).Subscribe(DetailsContent_MarginChanged);
				}
				_detailsElement.Children.Add(_detailsContent);
			}
		}
		if (_detailsContent != null && !_detailsLoaded)
		{
			_detailsLoaded = true;
			_detailsContent.DataContext = base.DataContext;
			OwningGrid.OnLoadingRowDetails(this, _detailsContent);
		}
		if (initializeDetailsPreferredHeight && double.IsNaN(_detailsDesiredHeight) && _appliedDetailsTemplate != null && _detailsElement.Children.Count > 0)
		{
			EnsureDetailsDesiredHeight();
		}
		else if (appliedDetailsTemplate == null)
		{
			_detailsDesiredHeight = double.NaN;
			EnsureDetailsDesiredHeight();
			_detailsElement.ContentHeight = _detailsDesiredHeight;
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		if (change.Property == StyledElement.DataContextProperty)
		{
			DataGrid owningGrid = OwningGrid;
			if (owningGrid != null && IsRecycled)
			{
				List<DataGridColumn> columnsItemsInternal = owningGrid.ColumnsItemsInternal;
				int count = columnsItemsInternal.Count;
				for (int i = 0; i < count; i++)
				{
					if (columnsItemsInternal[i] is DataGridTemplateColumn dataGridTemplateColumn)
					{
						dataGridTemplateColumn.RefreshCellContent((Control)Cells[dataGridTemplateColumn.Index].Content, "CellTemplate");
					}
				}
			}
		}
		base.OnPropertyChanged(change);
	}
}
