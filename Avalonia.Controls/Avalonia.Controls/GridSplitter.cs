using System;
using Avalonia.Collections;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Utilities;

namespace Avalonia.Controls;

public class GridSplitter : Thumb
{
	private sealed class PreviewAdorner : Decorator
	{
		private readonly TranslateTransform _translation;

		private readonly Decorator _decorator;

		public double OffsetX
		{
			get
			{
				return _translation.X;
			}
			set
			{
				_translation.X = value;
			}
		}

		public double OffsetY
		{
			get
			{
				return _translation.Y;
			}
			set
			{
				_translation.Y = value;
			}
		}

		public PreviewAdorner(Control? previewControl)
		{
			_translation = new TranslateTransform();
			_decorator = new Decorator
			{
				Child = previewControl,
				RenderTransform = _translation
			};
			base.Child = _decorator;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			base.Clip = null;
			return base.ArrangeOverride(finalSize);
		}
	}

	private enum SplitBehavior
	{
		Split,
		Resize1,
		Resize2
	}

	private class ResizeData
	{
		public bool ShowsPreview;

		public PreviewAdorner? Adorner;

		public double MinChange;

		public double MaxChange;

		public Grid? Grid;

		public GridResizeDirection ResizeDirection;

		public GridResizeBehavior ResizeBehavior;

		public DefinitionBase? Definition1;

		public DefinitionBase? Definition2;

		public SplitBehavior SplitBehavior;

		public int SplitterIndex;

		public int Definition1Index;

		public int Definition2Index;

		public GridLength OriginalDefinition1Length;

		public GridLength OriginalDefinition2Length;

		public double OriginalDefinition1ActualLength;

		public double OriginalDefinition2ActualLength;

		public double SplitterLength;

		public double Scaling;
	}

	public static readonly StyledProperty<GridResizeDirection> ResizeDirectionProperty = AvaloniaProperty.Register<GridSplitter, GridResizeDirection>("ResizeDirection", GridResizeDirection.Auto);

	public static readonly StyledProperty<GridResizeBehavior> ResizeBehaviorProperty = AvaloniaProperty.Register<GridSplitter, GridResizeBehavior>("ResizeBehavior", GridResizeBehavior.BasedOnAlignment);

	public static readonly StyledProperty<bool> ShowsPreviewProperty = AvaloniaProperty.Register<GridSplitter, bool>("ShowsPreview", defaultValue: false);

	public static readonly StyledProperty<double> KeyboardIncrementProperty = AvaloniaProperty.Register<GridSplitter, double>("KeyboardIncrement", 10.0);

	public static readonly StyledProperty<double> DragIncrementProperty = AvaloniaProperty.Register<GridSplitter, double>("DragIncrement", 1.0);

	public static readonly StyledProperty<ITemplate<Control>> PreviewContentProperty = AvaloniaProperty.Register<GridSplitter, ITemplate<Control>>("PreviewContent");

	private static readonly Cursor s_columnSplitterCursor = new Cursor(StandardCursorType.SizeWestEast);

	private static readonly Cursor s_rowSplitterCursor = new Cursor(StandardCursorType.SizeNorthSouth);

	private ResizeData? _resizeData;

	public GridResizeDirection ResizeDirection
	{
		get
		{
			return GetValue(ResizeDirectionProperty);
		}
		set
		{
			SetValue(ResizeDirectionProperty, value);
		}
	}

	public GridResizeBehavior ResizeBehavior
	{
		get
		{
			return GetValue(ResizeBehaviorProperty);
		}
		set
		{
			SetValue(ResizeBehaviorProperty, value);
		}
	}

	public bool ShowsPreview
	{
		get
		{
			return GetValue(ShowsPreviewProperty);
		}
		set
		{
			SetValue(ShowsPreviewProperty, value);
		}
	}

	public double KeyboardIncrement
	{
		get
		{
			return GetValue(KeyboardIncrementProperty);
		}
		set
		{
			SetValue(KeyboardIncrementProperty, value);
		}
	}

	public double DragIncrement
	{
		get
		{
			return GetValue(DragIncrementProperty);
		}
		set
		{
			SetValue(DragIncrementProperty, value);
		}
	}

	public ITemplate<Control> PreviewContent
	{
		get
		{
			return GetValue(PreviewContentProperty);
		}
		set
		{
			SetValue(PreviewContentProperty, value);
		}
	}

	internal GridResizeDirection GetEffectiveResizeDirection()
	{
		GridResizeDirection resizeDirection = ResizeDirection;
		if (resizeDirection != 0)
		{
			return resizeDirection;
		}
		if (base.HorizontalAlignment != 0)
		{
			return GridResizeDirection.Columns;
		}
		if (base.VerticalAlignment != 0)
		{
			return GridResizeDirection.Rows;
		}
		if (base.Bounds.Width <= base.Bounds.Height)
		{
			return GridResizeDirection.Columns;
		}
		return GridResizeDirection.Rows;
	}

	private GridResizeBehavior GetEffectiveResizeBehavior(GridResizeDirection direction)
	{
		GridResizeBehavior gridResizeBehavior = ResizeBehavior;
		if (gridResizeBehavior == GridResizeBehavior.BasedOnAlignment)
		{
			gridResizeBehavior = ((direction == GridResizeDirection.Columns) ? (base.HorizontalAlignment switch
			{
				HorizontalAlignment.Left => GridResizeBehavior.PreviousAndCurrent, 
				HorizontalAlignment.Right => GridResizeBehavior.CurrentAndNext, 
				_ => GridResizeBehavior.PreviousAndNext, 
			}) : (base.VerticalAlignment switch
			{
				VerticalAlignment.Top => GridResizeBehavior.PreviousAndCurrent, 
				VerticalAlignment.Bottom => GridResizeBehavior.CurrentAndNext, 
				_ => GridResizeBehavior.PreviousAndNext, 
			}));
		}
		return gridResizeBehavior;
	}

	private void RemovePreviewAdorner()
	{
		if (_resizeData?.Adorner != null)
		{
			AdornerLayer.GetAdornerLayer(this).Children.Remove(_resizeData.Adorner);
		}
	}

	private void InitializeData(bool showsPreview)
	{
		if (base.Parent is Grid grid)
		{
			GridResizeDirection effectiveResizeDirection = GetEffectiveResizeDirection();
			_resizeData = new ResizeData
			{
				Grid = grid,
				ShowsPreview = showsPreview,
				ResizeDirection = effectiveResizeDirection,
				SplitterLength = Math.Min(base.Bounds.Width, base.Bounds.Height),
				ResizeBehavior = GetEffectiveResizeBehavior(effectiveResizeDirection),
				Scaling = ((base.VisualRoot as ILayoutRoot)?.LayoutScaling ?? 1.0)
			};
			if (!SetupDefinitionsToResize())
			{
				_resizeData = null;
			}
			else
			{
				SetupPreviewAdorner();
			}
		}
	}

	private bool SetupDefinitionsToResize()
	{
		if (GetValue((_resizeData.ResizeDirection == GridResizeDirection.Columns) ? Grid.ColumnSpanProperty : Grid.RowSpanProperty) == 1)
		{
			int value = GetValue((_resizeData.ResizeDirection == GridResizeDirection.Columns) ? Grid.ColumnProperty : Grid.RowProperty);
			int num;
			int num2;
			switch (_resizeData.ResizeBehavior)
			{
			case GridResizeBehavior.PreviousAndCurrent:
				num = value - 1;
				num2 = value;
				break;
			case GridResizeBehavior.CurrentAndNext:
				num = value;
				num2 = value + 1;
				break;
			default:
				num = value - 1;
				num2 = value + 1;
				break;
			}
			int num3 = ((_resizeData.ResizeDirection == GridResizeDirection.Columns) ? _resizeData.Grid.ColumnDefinitions.Count : _resizeData.Grid.RowDefinitions.Count);
			if (num >= 0 && num2 < num3)
			{
				_resizeData.SplitterIndex = value;
				_resizeData.Definition1Index = num;
				_resizeData.Definition1 = GetGridDefinition(_resizeData.Grid, num, _resizeData.ResizeDirection);
				_resizeData.OriginalDefinition1Length = _resizeData.Definition1.UserSizeValueCache;
				_resizeData.OriginalDefinition1ActualLength = GetActualLength(_resizeData.Definition1);
				_resizeData.Definition2Index = num2;
				_resizeData.Definition2 = GetGridDefinition(_resizeData.Grid, num2, _resizeData.ResizeDirection);
				_resizeData.OriginalDefinition2Length = _resizeData.Definition2.UserSizeValueCache;
				_resizeData.OriginalDefinition2ActualLength = GetActualLength(_resizeData.Definition2);
				bool flag = IsStar(_resizeData.Definition1);
				bool flag2 = IsStar(_resizeData.Definition2);
				if (flag && flag2)
				{
					_resizeData.SplitBehavior = SplitBehavior.Split;
				}
				else
				{
					_resizeData.SplitBehavior = ((!flag) ? SplitBehavior.Resize1 : SplitBehavior.Resize2);
				}
				return true;
			}
		}
		return false;
	}

	private void SetupPreviewAdorner()
	{
		if (_resizeData.ShowsPreview)
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_resizeData.Grid);
			ITemplate<Control> previewContent = PreviewContent;
			if (adornerLayer != null)
			{
				Control previewControl = previewContent?.Build();
				_resizeData.Adorner = new PreviewAdorner(previewControl);
				AdornerLayer.SetAdornedElement(_resizeData.Adorner, this);
				adornerLayer.Children.Add(_resizeData.Adorner);
				GetDeltaConstraints(out _resizeData.MinChange, out _resizeData.MaxChange);
			}
		}
	}

	protected override void OnPointerEntered(PointerEventArgs e)
	{
		base.OnPointerEntered(e);
		switch (GetEffectiveResizeDirection())
		{
		case GridResizeDirection.Columns:
			base.Cursor = s_columnSplitterCursor;
			break;
		case GridResizeDirection.Rows:
			base.Cursor = s_rowSplitterCursor;
			break;
		}
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		base.OnLostFocus(e);
		if (_resizeData != null)
		{
			CancelResize();
		}
	}

	protected override void OnDragStarted(VectorEventArgs e)
	{
		base.OnDragStarted(e);
		if (_resizeData == null)
		{
			InitializeData(ShowsPreview);
		}
	}

	protected override void OnDragDelta(VectorEventArgs e)
	{
		base.OnDragDelta(e);
		if (_resizeData == null)
		{
			return;
		}
		double x = e.Vector.X;
		double y = e.Vector.Y;
		double dragIncrement = DragIncrement;
		x = Math.Round(x / dragIncrement) * dragIncrement;
		y = Math.Round(y / dragIncrement) * dragIncrement;
		if (_resizeData.ShowsPreview)
		{
			if (_resizeData.ResizeDirection == GridResizeDirection.Columns)
			{
				_resizeData.Adorner.OffsetX = Math.Min(Math.Max(x, _resizeData.MinChange), _resizeData.MaxChange);
			}
			else
			{
				_resizeData.Adorner.OffsetY = Math.Min(Math.Max(y, _resizeData.MinChange), _resizeData.MaxChange);
			}
		}
		else
		{
			MoveSplitter(x, y);
		}
	}

	protected override void OnDragCompleted(VectorEventArgs e)
	{
		base.OnDragCompleted(e);
		if (_resizeData != null)
		{
			if (_resizeData.ShowsPreview)
			{
				MoveSplitter(_resizeData.Adorner.OffsetX, _resizeData.Adorner.OffsetY);
				RemovePreviewAdorner();
			}
			_resizeData = null;
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		switch (e.Key)
		{
		case Key.Escape:
			if (_resizeData != null)
			{
				CancelResize();
				e.Handled = true;
			}
			break;
		case Key.Left:
			e.Handled = KeyboardMoveSplitter(0.0 - KeyboardIncrement, 0.0);
			break;
		case Key.Right:
			e.Handled = KeyboardMoveSplitter(KeyboardIncrement, 0.0);
			break;
		case Key.Up:
			e.Handled = KeyboardMoveSplitter(0.0, 0.0 - KeyboardIncrement);
			break;
		case Key.Down:
			e.Handled = KeyboardMoveSplitter(0.0, KeyboardIncrement);
			break;
		}
	}

	private void CancelResize()
	{
		if (_resizeData.ShowsPreview)
		{
			RemovePreviewAdorner();
		}
		else
		{
			SetDefinitionLength(_resizeData.Definition1, _resizeData.OriginalDefinition1Length);
			SetDefinitionLength(_resizeData.Definition2, _resizeData.OriginalDefinition2Length);
		}
		_resizeData = null;
	}

	private static bool IsStar(DefinitionBase definition)
	{
		return definition.UserSizeValueCache.IsStar;
	}

	private static DefinitionBase GetGridDefinition(Grid grid, int index, GridResizeDirection direction)
	{
		if (direction != GridResizeDirection.Columns)
		{
			return grid.RowDefinitions[index];
		}
		return grid.ColumnDefinitions[index];
	}

	private static double GetActualLength(DefinitionBase definition)
	{
		return (definition as ColumnDefinition)?.ActualWidth ?? ((RowDefinition)definition).ActualHeight;
	}

	private static void SetDefinitionLength(DefinitionBase definition, GridLength length)
	{
		definition.SetValue((definition is ColumnDefinition) ? ColumnDefinition.WidthProperty : RowDefinition.HeightProperty, length);
	}

	private void GetDeltaConstraints(out double minDelta, out double maxDelta)
	{
		double actualLength = GetActualLength(_resizeData.Definition1);
		double num = _resizeData.Definition1.UserMinSizeValueCache;
		double userMaxSizeValueCache = _resizeData.Definition1.UserMaxSizeValueCache;
		double actualLength2 = GetActualLength(_resizeData.Definition2);
		double num2 = _resizeData.Definition2.UserMinSizeValueCache;
		double userMaxSizeValueCache2 = _resizeData.Definition2.UserMaxSizeValueCache;
		if (_resizeData.SplitterIndex == _resizeData.Definition1Index)
		{
			num = Math.Max(num, _resizeData.SplitterLength);
		}
		else if (_resizeData.SplitterIndex == _resizeData.Definition2Index)
		{
			num2 = Math.Max(num2, _resizeData.SplitterLength);
		}
		minDelta = 0.0 - Math.Min(actualLength - num, userMaxSizeValueCache2 - actualLength2);
		maxDelta = Math.Min(userMaxSizeValueCache - actualLength, actualLength2 - num2);
	}

	private void SetLengths(double definition1Pixels, double definition2Pixels)
	{
		if (_resizeData.SplitBehavior == SplitBehavior.Split)
		{
			IAvaloniaReadOnlyList<DefinitionBase> avaloniaReadOnlyList;
			if (_resizeData.ResizeDirection != GridResizeDirection.Columns)
			{
				IAvaloniaReadOnlyList<DefinitionBase> rowDefinitions = _resizeData.Grid.RowDefinitions;
				avaloniaReadOnlyList = rowDefinitions;
			}
			else
			{
				IAvaloniaReadOnlyList<DefinitionBase> rowDefinitions = _resizeData.Grid.ColumnDefinitions;
				avaloniaReadOnlyList = rowDefinitions;
			}
			IAvaloniaReadOnlyList<DefinitionBase> avaloniaReadOnlyList2 = avaloniaReadOnlyList;
			int count = avaloniaReadOnlyList2.Count;
			for (int i = 0; i < count; i++)
			{
				DefinitionBase definition = avaloniaReadOnlyList2[i];
				if (i == _resizeData.Definition1Index)
				{
					SetDefinitionLength(definition, new GridLength(definition1Pixels, GridUnitType.Star));
				}
				else if (i == _resizeData.Definition2Index)
				{
					SetDefinitionLength(definition, new GridLength(definition2Pixels, GridUnitType.Star));
				}
				else if (IsStar(definition))
				{
					SetDefinitionLength(definition, new GridLength(GetActualLength(definition), GridUnitType.Star));
				}
			}
		}
		else if (_resizeData.SplitBehavior == SplitBehavior.Resize1)
		{
			SetDefinitionLength(_resizeData.Definition1, new GridLength(definition1Pixels));
		}
		else
		{
			SetDefinitionLength(_resizeData.Definition2, new GridLength(definition2Pixels));
		}
	}

	private void MoveSplitter(double horizontalChange, double verticalChange)
	{
		double num = ((_resizeData.ResizeDirection == GridResizeDirection.Columns) ? horizontalChange : verticalChange);
		if (base.UseLayoutRounding)
		{
			num = LayoutHelper.RoundLayoutValue(num, LayoutHelper.GetLayoutScale(this));
		}
		DefinitionBase definition = _resizeData.Definition1;
		DefinitionBase definition2 = _resizeData.Definition2;
		if (definition != null && definition2 != null)
		{
			double actualLength = GetActualLength(definition);
			double actualLength2 = GetActualLength(definition2);
			double eps = 1.0 / _resizeData.Scaling + LayoutHelper.LayoutEpsilon;
			if (_resizeData.SplitBehavior == SplitBehavior.Split && !MathUtilities.AreClose(actualLength + actualLength2, _resizeData.OriginalDefinition1ActualLength + _resizeData.OriginalDefinition2ActualLength, eps))
			{
				CancelResize();
				return;
			}
			GetDeltaConstraints(out var minDelta, out var maxDelta);
			num = Math.Min(Math.Max(num, minDelta), maxDelta);
			double num2 = actualLength + num;
			double definition2Pixels = actualLength + actualLength2 - num2;
			SetLengths(num2, definition2Pixels);
		}
	}

	private bool KeyboardMoveSplitter(double horizontalChange, double verticalChange)
	{
		if (_resizeData != null)
		{
			return false;
		}
		InitializeData(showsPreview: false);
		if (_resizeData == null)
		{
			return false;
		}
		MoveSplitter(horizontalChange, verticalChange);
		_resizeData = null;
		return true;
	}
}
