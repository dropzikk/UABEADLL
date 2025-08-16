using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Utils;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Utilities;
using Avalonia.VisualTree;

namespace Avalonia.Controls;

public class VirtualizingStackPanel : VirtualizingPanel, IScrollSnapPointsInfo
{
	private struct MeasureViewport
	{
		public int anchorIndex;

		public double anchorU;

		public double viewportUStart;

		public double viewportUEnd;

		public double measuredV;

		public double realizedEndU;

		public int lastIndex;

		public bool viewportIsDisjunct;
	}

	public static readonly StyledProperty<Orientation> OrientationProperty = StackPanel.OrientationProperty.AddOwner<VirtualizingStackPanel>();

	public static readonly StyledProperty<bool> AreHorizontalSnapPointsRegularProperty = AvaloniaProperty.Register<VirtualizingStackPanel, bool>("AreHorizontalSnapPointsRegular", defaultValue: false);

	public static readonly StyledProperty<bool> AreVerticalSnapPointsRegularProperty = AvaloniaProperty.Register<VirtualizingStackPanel, bool>("AreVerticalSnapPointsRegular", defaultValue: false);

	public static readonly RoutedEvent<RoutedEventArgs> HorizontalSnapPointsChangedEvent = RoutedEvent.Register<VirtualizingStackPanel, RoutedEventArgs>("HorizontalSnapPointsChanged", RoutingStrategies.Bubble);

	public static readonly RoutedEvent<RoutedEventArgs> VerticalSnapPointsChangedEvent = RoutedEvent.Register<VirtualizingStackPanel, RoutedEventArgs>("VerticalSnapPointsChanged", RoutingStrategies.Bubble);

	private static readonly AttachedProperty<object?> RecycleKeyProperty = AvaloniaProperty.RegisterAttached<VirtualizingStackPanel, Control, object>("RecycleKey");

	private static readonly Rect s_invalidViewport = new Rect(double.PositiveInfinity, double.PositiveInfinity, 0.0, 0.0);

	private static readonly object s_itemIsItsOwnContainer = new object();

	private readonly Action<Control, int> _recycleElement;

	private readonly Action<Control> _recycleElementOnItemRemoved;

	private readonly Action<Control, int, int> _updateElementIndex;

	private int _scrollToIndex = -1;

	private Control? _scrollToElement;

	private bool _isInLayout;

	private bool _isWaitingForViewportUpdate;

	private double _lastEstimatedElementSizeU = 25.0;

	private RealizedStackElements? _measureElements;

	private RealizedStackElements? _realizedElements;

	private ScrollViewer? _scrollViewer;

	private Rect _viewport = s_invalidViewport;

	private Dictionary<object, Stack<Control>>? _recyclePool;

	private Control? _focusedElement;

	private int _focusedIndex = -1;

	public Orientation Orientation
	{
		get
		{
			return GetValue(OrientationProperty);
		}
		set
		{
			SetValue(OrientationProperty, value);
		}
	}

	public bool AreHorizontalSnapPointsRegular
	{
		get
		{
			return GetValue(AreHorizontalSnapPointsRegularProperty);
		}
		set
		{
			SetValue(AreHorizontalSnapPointsRegularProperty, value);
		}
	}

	public bool AreVerticalSnapPointsRegular
	{
		get
		{
			return GetValue(AreVerticalSnapPointsRegularProperty);
		}
		set
		{
			SetValue(AreVerticalSnapPointsRegularProperty, value);
		}
	}

	public int FirstRealizedIndex => _realizedElements?.FirstIndex ?? (-1);

	public int LastRealizedIndex => _realizedElements?.LastIndex ?? (-1);

	public event EventHandler<RoutedEventArgs>? HorizontalSnapPointsChanged
	{
		add
		{
			AddHandler(HorizontalSnapPointsChangedEvent, value);
		}
		remove
		{
			RemoveHandler(HorizontalSnapPointsChangedEvent, value);
		}
	}

	public event EventHandler<RoutedEventArgs>? VerticalSnapPointsChanged
	{
		add
		{
			AddHandler(VerticalSnapPointsChangedEvent, value);
		}
		remove
		{
			RemoveHandler(VerticalSnapPointsChangedEvent, value);
		}
	}

	public VirtualizingStackPanel()
	{
		_recycleElement = RecycleElement;
		_recycleElementOnItemRemoved = RecycleElementOnItemRemoved;
		_updateElementIndex = UpdateElementIndex;
		base.EffectiveViewportChanged += OnEffectiveViewportChanged;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		IReadOnlyList<object> items = base.Items;
		if (items.Count == 0)
		{
			return default(Size);
		}
		if (_isWaitingForViewportUpdate)
		{
			return base.DesiredSize;
		}
		_isInLayout = true;
		try
		{
			Orientation orientation = Orientation;
			if (_realizedElements == null)
			{
				_realizedElements = new RealizedStackElements();
			}
			if (_measureElements == null)
			{
				_measureElements = new RealizedStackElements();
			}
			MeasureViewport viewport = CalculateMeasureViewport(items);
			if (viewport.viewportIsDisjunct)
			{
				_realizedElements.RecycleAllElements(_recycleElement);
			}
			RealizeElements(items, availableSize, ref viewport);
			RealizedStackElements realizedElements = _realizedElements;
			RealizedStackElements measureElements = _measureElements;
			_measureElements = realizedElements;
			_realizedElements = measureElements;
			_measureElements.ResetForReuse();
			return CalculateDesiredSize(orientation, items.Count, in viewport);
		}
		finally
		{
			_isInLayout = false;
		}
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		if (_realizedElements == null)
		{
			return default(Size);
		}
		_isInLayout = true;
		try
		{
			Orientation orientation = Orientation;
			double num = _realizedElements.StartU;
			for (int i = 0; i < _realizedElements.Count; i++)
			{
				Control control = _realizedElements.Elements[i];
				if (control != null)
				{
					double num2 = _realizedElements.SizeU[i];
					Rect rect = ((orientation == Orientation.Horizontal) ? new Rect(num, 0.0, num2, finalSize.Height) : new Rect(0.0, num, finalSize.Width, num2));
					control.Arrange(rect);
					_scrollViewer?.RegisterAnchorCandidate(control);
					num += ((orientation == Orientation.Horizontal) ? rect.Width : rect.Height);
				}
			}
			return finalSize;
		}
		finally
		{
			_isInLayout = false;
			RaiseEvent(new RoutedEventArgs((Orientation == Orientation.Horizontal) ? HorizontalSnapPointsChangedEvent : VerticalSnapPointsChangedEvent));
		}
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		_scrollViewer = this.FindAncestorOfType<ScrollViewer>();
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		_scrollViewer = null;
	}

	protected override void OnItemsChanged(IReadOnlyList<object?> items, NotifyCollectionChangedEventArgs e)
	{
		InvalidateMeasure();
		if (_realizedElements != null)
		{
			switch (e.Action)
			{
			case NotifyCollectionChangedAction.Add:
				_realizedElements.ItemsInserted(e.NewStartingIndex, e.NewItems.Count, _updateElementIndex);
				break;
			case NotifyCollectionChangedAction.Remove:
				_realizedElements.ItemsRemoved(e.OldStartingIndex, e.OldItems.Count, _updateElementIndex, _recycleElementOnItemRemoved);
				break;
			case NotifyCollectionChangedAction.Replace:
			case NotifyCollectionChangedAction.Move:
				_realizedElements.ItemsRemoved(e.OldStartingIndex, e.OldItems.Count, _updateElementIndex, _recycleElementOnItemRemoved);
				_realizedElements.ItemsInserted(e.NewStartingIndex, e.NewItems.Count, _updateElementIndex);
				break;
			case NotifyCollectionChangedAction.Reset:
				_realizedElements.ItemsReset(_recycleElementOnItemRemoved);
				break;
			}
		}
	}

	protected override IInputElement? GetControl(NavigationDirection direction, IInputElement? from, bool wrap)
	{
		int count = base.Items.Count;
		Control control = from as Control;
		if (count == 0 || (control == null && direction != NavigationDirection.First))
		{
			return null;
		}
		bool flag = Orientation == Orientation.Horizontal;
		int num = ((control != null) ? IndexFromContainer(control) : (-1));
		int num2 = num;
		switch (direction)
		{
		case NavigationDirection.First:
			num2 = 0;
			break;
		case NavigationDirection.Last:
			num2 = count - 1;
			break;
		case NavigationDirection.Next:
			num2++;
			break;
		case NavigationDirection.Previous:
			num2--;
			break;
		case NavigationDirection.Left:
			if (flag)
			{
				num2--;
			}
			break;
		case NavigationDirection.Right:
			if (flag)
			{
				num2++;
			}
			break;
		case NavigationDirection.Up:
			if (!flag)
			{
				num2--;
			}
			break;
		case NavigationDirection.Down:
			if (!flag)
			{
				num2++;
			}
			break;
		default:
			return null;
		}
		if (num == num2)
		{
			return from;
		}
		if (wrap)
		{
			if (num2 < 0)
			{
				num2 = count - 1;
			}
			else if (num2 >= count)
			{
				num2 = 0;
			}
		}
		return ScrollIntoView(num2);
	}

	protected internal override IEnumerable<Control>? GetRealizedContainers()
	{
		return _realizedElements?.Elements.Where((Control x) => x != null);
	}

	protected internal override Control? ContainerFromIndex(int index)
	{
		if (index < 0 || index >= base.Items.Count)
		{
			return null;
		}
		if (_scrollToIndex == index)
		{
			return _scrollToElement;
		}
		if (_focusedIndex == index)
		{
			return _focusedElement;
		}
		Control realizedElement = GetRealizedElement(index);
		if (realizedElement != null)
		{
			return realizedElement;
		}
		if (base.Items[index] is Control control && control.GetValue(RecycleKeyProperty) == s_itemIsItsOwnContainer)
		{
			return control;
		}
		return null;
	}

	protected internal override int IndexFromContainer(Control container)
	{
		if (container == _scrollToElement)
		{
			return _scrollToIndex;
		}
		if (container == _focusedElement)
		{
			return _focusedIndex;
		}
		return _realizedElements?.GetIndex(container) ?? (-1);
	}

	protected internal override Control? ScrollIntoView(int index)
	{
		IReadOnlyList<object> items = base.Items;
		if (_isInLayout || index < 0 || index >= items.Count || _realizedElements == null || !base.IsEffectivelyVisible)
		{
			return null;
		}
		Control realizedElement = GetRealizedElement(index);
		if (realizedElement != null)
		{
			realizedElement.BringIntoView();
			return realizedElement;
		}
		if (this.GetVisualRoot() is ILayoutRoot layoutRoot)
		{
			Control orCreateElement = GetOrCreateElement(items, index);
			orCreateElement.Measure(Size.Infinity);
			double orEstimateElementU = _realizedElements.GetOrEstimateElementU(index, ref _lastEstimatedElementSizeU);
			Rect rect = ((Orientation == Orientation.Horizontal) ? new Rect(orEstimateElementU, 0.0, orCreateElement.DesiredSize.Width, orCreateElement.DesiredSize.Height) : new Rect(0.0, orEstimateElementU, orCreateElement.DesiredSize.Width, orCreateElement.DesiredSize.Height));
			orCreateElement.Arrange(rect);
			_scrollToElement = orCreateElement;
			_scrollToIndex = index;
			if (!base.Bounds.Contains(rect) && !_viewport.Contains(rect))
			{
				_isWaitingForViewportUpdate = true;
				layoutRoot.LayoutManager.ExecuteLayoutPass();
				_isWaitingForViewportUpdate = false;
			}
			orCreateElement.BringIntoView();
			_isWaitingForViewportUpdate = !_viewport.Contains(rect);
			layoutRoot.LayoutManager.ExecuteLayoutPass();
			if (_isWaitingForViewportUpdate)
			{
				_isWaitingForViewportUpdate = false;
				InvalidateMeasure();
				layoutRoot.LayoutManager.ExecuteLayoutPass();
			}
			_scrollToElement = null;
			_scrollToIndex = -1;
			return orCreateElement;
		}
		return null;
	}

	internal IReadOnlyList<Control?> GetRealizedElements()
	{
		return _realizedElements?.Elements ?? Array.Empty<Control>();
	}

	private MeasureViewport CalculateMeasureViewport(IReadOnlyList<object?> items)
	{
		Rect rect = ((_viewport != s_invalidViewport) ? _viewport : EstimateViewport());
		double num = ((Orientation == Orientation.Horizontal) ? rect.X : rect.Y);
		double num2 = ((Orientation == Orientation.Horizontal) ? rect.Right : rect.Bottom);
		int itemCount = items?.Count ?? 0;
		(int index, double position) orEstimateAnchorElementForViewport = _realizedElements.GetOrEstimateAnchorElementForViewport(num, num2, itemCount, ref _lastEstimatedElementSizeU);
		int item = orEstimateAnchorElementForViewport.index;
		double item2 = orEstimateAnchorElementForViewport.position;
		bool viewportIsDisjunct = item < _realizedElements.FirstIndex || item > _realizedElements.LastIndex;
		MeasureViewport result = default(MeasureViewport);
		result.anchorIndex = item;
		result.anchorU = item2;
		result.viewportUStart = num;
		result.viewportUEnd = num2;
		result.viewportIsDisjunct = viewportIsDisjunct;
		return result;
	}

	private Size CalculateDesiredSize(Orientation orientation, int itemCount, in MeasureViewport viewport)
	{
		double num = 0.0;
		double measuredV = viewport.measuredV;
		if (viewport.lastIndex >= 0)
		{
			int num2 = itemCount - viewport.lastIndex - 1;
			num = viewport.realizedEndU + (double)num2 * _lastEstimatedElementSizeU;
		}
		if (orientation != 0)
		{
			return new Size(measuredV, num);
		}
		return new Size(num, measuredV);
	}

	private double EstimateElementSizeU()
	{
		if (_realizedElements == null)
		{
			return _lastEstimatedElementSizeU;
		}
		double num = _realizedElements.EstimateElementSizeU();
		if (num >= 0.0)
		{
			_lastEstimatedElementSizeU = num;
		}
		return _lastEstimatedElementSizeU;
	}

	private Rect EstimateViewport()
	{
		Visual visual = this.GetVisualParent();
		Rect result = default(Rect);
		if (visual == null)
		{
			return result;
		}
		while (visual != null)
		{
			if (visual.Bounds.Width != 0.0 || visual.Bounds.Height != 0.0)
			{
				Matrix? matrix = visual.TransformToVisual(this);
				if (matrix.HasValue)
				{
					Matrix valueOrDefault = matrix.GetValueOrDefault();
					result = new Rect(0.0, 0.0, visual.Bounds.Width, visual.Bounds.Height).TransformToAABB(valueOrDefault);
					break;
				}
			}
			visual = visual?.GetVisualParent();
		}
		return result.Intersect(new Rect(0.0, 0.0, double.PositiveInfinity, double.PositiveInfinity));
	}

	private void RealizeElements(IReadOnlyList<object?> items, Size availableSize, ref MeasureViewport viewport)
	{
		int num = viewport.anchorIndex;
		bool flag = Orientation == Orientation.Horizontal;
		double num2 = viewport.anchorU;
		if (num2 <= viewport.anchorU)
		{
			_realizedElements.RecycleElementsBefore(viewport.anchorIndex, _recycleElement);
		}
		do
		{
			Control orCreateElement = GetOrCreateElement(items, num);
			orCreateElement.Measure(availableSize);
			double num3 = (flag ? orCreateElement.DesiredSize.Width : orCreateElement.DesiredSize.Height);
			double val = (flag ? orCreateElement.DesiredSize.Height : orCreateElement.DesiredSize.Width);
			_measureElements.Add(num, orCreateElement, num2, num3);
			viewport.measuredV = Math.Max(viewport.measuredV, val);
			num2 += num3;
			num++;
		}
		while (num2 < viewport.viewportUEnd && num < items.Count);
		viewport.lastIndex = num - 1;
		viewport.realizedEndU = num2;
		_realizedElements.RecycleElementsAfter(viewport.lastIndex, _recycleElement);
		num = viewport.anchorIndex - 1;
		num2 = viewport.anchorU;
		while (num2 > viewport.viewportUStart && num >= 0)
		{
			Control orCreateElement2 = GetOrCreateElement(items, num);
			orCreateElement2.Measure(availableSize);
			double num4 = (flag ? orCreateElement2.DesiredSize.Width : orCreateElement2.DesiredSize.Height);
			double val2 = (flag ? orCreateElement2.DesiredSize.Height : orCreateElement2.DesiredSize.Width);
			num2 -= num4;
			_measureElements.Add(num, orCreateElement2, num2, num4);
			viewport.measuredV = Math.Max(viewport.measuredV, val2);
			num--;
		}
		_realizedElements.RecycleElementsBefore(num + 1, _recycleElement);
	}

	private Control GetOrCreateElement(IReadOnlyList<object?> items, int index)
	{
		Control control = GetRealizedElement(index) ?? GetRealizedElement(index, ref _focusedIndex, ref _focusedElement) ?? GetRealizedElement(index, ref _scrollToIndex, ref _scrollToElement);
		if (control != null)
		{
			return control;
		}
		object item = items[index];
		if (base.ItemContainerGenerator.NeedsContainer(item, index, out object recycleKey))
		{
			return GetRecycledElement(item, index, recycleKey) ?? CreateElement(item, index, recycleKey);
		}
		return GetItemAsOwnContainer(item, index);
	}

	private Control? GetRealizedElement(int index)
	{
		return _realizedElements?.GetElement(index);
	}

	private static Control? GetRealizedElement(int index, ref int specialIndex, ref Control? specialElement)
	{
		if (specialIndex == index)
		{
			Control? result = specialElement;
			specialIndex = -1;
			specialElement = null;
			return result;
		}
		return null;
	}

	private Control GetItemAsOwnContainer(object? item, int index)
	{
		Control control = (Control)item;
		ItemContainerGenerator itemContainerGenerator = base.ItemContainerGenerator;
		if (!control.IsSet(RecycleKeyProperty))
		{
			itemContainerGenerator.PrepareItemContainer(control, control, index);
			AddInternalChild(control);
			control.SetValue(RecycleKeyProperty, s_itemIsItsOwnContainer);
			itemContainerGenerator.ItemContainerPrepared(control, item, index);
		}
		control.IsVisible = true;
		return control;
	}

	private Control? GetRecycledElement(object? item, int index, object? recycleKey)
	{
		if (recycleKey == null)
		{
			return null;
		}
		ItemContainerGenerator itemContainerGenerator = base.ItemContainerGenerator;
		Dictionary<object, Stack<Control>>? recyclePool = _recyclePool;
		if (recyclePool != null && recyclePool.TryGetValue(recycleKey, out Stack<Control> value) && value.Count > 0)
		{
			Control control = value.Pop();
			control.IsVisible = true;
			itemContainerGenerator.PrepareItemContainer(control, item, index);
			itemContainerGenerator.ItemContainerPrepared(control, item, index);
			return control;
		}
		return null;
	}

	private Control CreateElement(object? item, int index, object? recycleKey)
	{
		ItemContainerGenerator? itemContainerGenerator = base.ItemContainerGenerator;
		Control control = itemContainerGenerator.CreateContainer(item, index, recycleKey);
		control.SetValue(RecycleKeyProperty, recycleKey);
		itemContainerGenerator.PrepareItemContainer(control, item, index);
		AddInternalChild(control);
		itemContainerGenerator.ItemContainerPrepared(control, item, index);
		return control;
	}

	private void RecycleElement(Control element, int index)
	{
		_scrollViewer?.UnregisterAnchorCandidate(element);
		object value = element.GetValue(RecycleKeyProperty);
		if (value == null)
		{
			RemoveInternalChild(element);
		}
		else if (value == s_itemIsItsOwnContainer)
		{
			element.IsVisible = false;
		}
		else if (element.IsKeyboardFocusWithin)
		{
			_focusedElement = element;
			_focusedIndex = index;
			_focusedElement.LostFocus += OnUnrealizedFocusedElementLostFocus;
		}
		else
		{
			base.ItemContainerGenerator.ClearItemContainer(element);
			PushToRecyclePool(value, element);
			element.IsVisible = false;
		}
	}

	private void RecycleElementOnItemRemoved(Control element)
	{
		object value = element.GetValue(RecycleKeyProperty);
		if (value == null || value == s_itemIsItsOwnContainer)
		{
			RemoveInternalChild(element);
			return;
		}
		base.ItemContainerGenerator.ClearItemContainer(element);
		PushToRecyclePool(value, element);
		element.IsVisible = false;
	}

	private void PushToRecyclePool(object recycleKey, Control element)
	{
		if (_recyclePool == null)
		{
			_recyclePool = new Dictionary<object, Stack<Control>>();
		}
		if (!_recyclePool.TryGetValue(recycleKey, out Stack<Control> value))
		{
			value = new Stack<Control>();
			_recyclePool.Add(recycleKey, value);
		}
		value.Push(element);
	}

	private void UpdateElementIndex(Control element, int oldIndex, int newIndex)
	{
		base.ItemContainerGenerator.ItemContainerIndexChanged(element, oldIndex, newIndex);
	}

	private void OnEffectiveViewportChanged(object? sender, EffectiveViewportChangedEventArgs e)
	{
		bool num = Orientation == Orientation.Vertical;
		double value = (num ? _viewport.Top : _viewport.Left);
		double value2 = (num ? _viewport.Bottom : _viewport.Right);
		_viewport = e.EffectiveViewport.Intersect(new Rect(base.Bounds.Size));
		_isWaitingForViewportUpdate = false;
		double value3 = (num ? _viewport.Top : _viewport.Left);
		double value4 = (num ? _viewport.Bottom : _viewport.Right);
		if (!MathUtilities.AreClose(value, value3) || !MathUtilities.AreClose(value2, value4))
		{
			InvalidateMeasure();
		}
	}

	private void OnUnrealizedFocusedElementLostFocus(object? sender, RoutedEventArgs e)
	{
		if (_focusedElement != null && sender == _focusedElement)
		{
			_focusedElement.LostFocus -= OnUnrealizedFocusedElementLostFocus;
			RecycleElement(_focusedElement, _focusedIndex);
			_focusedElement = null;
			_focusedIndex = -1;
		}
	}

	public IReadOnlyList<double> GetIrregularSnapPoints(Orientation orientation, SnapPointsAlignment snapPointsAlignment)
	{
		if (_realizedElements == null)
		{
			return new List<double>();
		}
		return new VirtualizingSnapPointsList(_realizedElements, (base.ItemsControl?.ItemsSource?.Count()).GetValueOrDefault(), orientation, Orientation, snapPointsAlignment, EstimateElementSizeU());
	}

	public double GetRegularSnapPoints(Orientation orientation, SnapPointsAlignment snapPointsAlignment, out double offset)
	{
		offset = 0.0;
		Control control = _realizedElements?.Elements.FirstOrDefault();
		if (control == null)
		{
			return 0.0;
		}
		double result = 0.0;
		switch (Orientation)
		{
		case Orientation.Horizontal:
			if (!AreHorizontalSnapPointsRegular)
			{
				throw new InvalidOperationException();
			}
			result = control.Bounds.Width;
			switch (snapPointsAlignment)
			{
			case SnapPointsAlignment.Near:
				offset = 0.0;
				break;
			case SnapPointsAlignment.Center:
				offset = (control.Bounds.Right - control.Bounds.Left) / 2.0;
				break;
			case SnapPointsAlignment.Far:
				offset = control.Bounds.Width;
				break;
			}
			break;
		case Orientation.Vertical:
			if (!AreVerticalSnapPointsRegular)
			{
				throw new InvalidOperationException();
			}
			result = control.Bounds.Height;
			switch (snapPointsAlignment)
			{
			case SnapPointsAlignment.Near:
				offset = 0.0;
				break;
			case SnapPointsAlignment.Center:
				offset = (control.Bounds.Bottom - control.Bounds.Top) / 2.0;
				break;
			case SnapPointsAlignment.Far:
				offset = control.Bounds.Height;
				break;
			}
			break;
		}
		return result;
	}
}
