using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Animation;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Avalonia.Controls;

public class VirtualizingCarouselPanel : VirtualizingPanel, ILogicalScrollable, IScrollable
{
	private static readonly AttachedProperty<object?> RecycleKeyProperty = AvaloniaProperty.RegisterAttached<VirtualizingStackPanel, Control, object>("RecycleKey");

	private static readonly object s_itemIsItsOwnContainer = new object();

	private Size _extent;

	private Vector _offset;

	private Size _viewport;

	private Dictionary<object, Stack<Control>>? _recyclePool;

	private Control? _realized;

	private int _realizedIndex = -1;

	private Control? _transitionFrom;

	private int _transitionFromIndex = -1;

	private CancellationTokenSource? _transition;

	private EventHandler? _scrollInvalidated;

	bool ILogicalScrollable.CanHorizontallyScroll { get; set; }

	bool ILogicalScrollable.CanVerticallyScroll { get; set; }

	bool ILogicalScrollable.IsLogicalScrollEnabled => true;

	Size ILogicalScrollable.ScrollSize => new Size(1.0, 1.0);

	Size ILogicalScrollable.PageScrollSize => new Size(1.0, 1.0);

	Size IScrollable.Extent => Extent;

	Size IScrollable.Viewport => Viewport;

	Vector IScrollable.Offset
	{
		get
		{
			return _offset;
		}
		set
		{
			if ((double)(int)_offset.X != value.X)
			{
				InvalidateMeasure();
			}
			_offset = value;
		}
	}

	private Size Extent
	{
		get
		{
			return _extent;
		}
		set
		{
			if (_extent != value)
			{
				_extent = value;
				_scrollInvalidated?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	private Size Viewport
	{
		get
		{
			return _viewport;
		}
		set
		{
			if (_viewport != value)
			{
				_viewport = value;
				_scrollInvalidated?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	event EventHandler? ILogicalScrollable.ScrollInvalidated
	{
		add
		{
			_scrollInvalidated = (EventHandler)Delegate.Combine(_scrollInvalidated, value);
		}
		remove
		{
			_scrollInvalidated = (EventHandler)Delegate.Remove(_scrollInvalidated, value);
		}
	}

	bool ILogicalScrollable.BringIntoView(Control target, Rect targetRect)
	{
		return false;
	}

	Control? ILogicalScrollable.GetControlInDirection(NavigationDirection direction, Control? from)
	{
		return null;
	}

	void ILogicalScrollable.RaiseScrollInvalidated(EventArgs e)
	{
		_scrollInvalidated?.Invoke(this, e);
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		IReadOnlyList<object> items = base.Items;
		int num = (int)_offset.X;
		if (num != _realizedIndex)
		{
			if (_realized != null)
			{
				bool num2 = _transition != null;
				if (num2)
				{
					_transition.Cancel();
					_transition = null;
					if (_transitionFrom != null)
					{
						RecycleElement(_transitionFrom);
					}
					_transitionFrom = null;
					_transitionFromIndex = -1;
				}
				if (num2 || GetTransition() == null)
				{
					RecycleElement(_realized);
				}
				else
				{
					_transitionFrom = _realized;
					_transitionFromIndex = _realizedIndex;
				}
				_realized = null;
				_realizedIndex = -1;
			}
			if (num >= 0 && num < items.Count)
			{
				_realized = GetOrCreateElement(items, num);
				_realizedIndex = num;
			}
		}
		if (_realized == null)
		{
			Size extent = (Viewport = new Size(0.0, 0.0));
			Extent = extent;
			_transitionFrom = null;
			_transitionFromIndex = -1;
			return default(Size);
		}
		_realized.Measure(availableSize);
		Extent = new Size(items.Count, 1.0);
		Viewport = new Size(1.0, 1.0);
		return _realized.DesiredSize;
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		Size result = base.ArrangeOverride(finalSize);
		if (_transition == null && _transitionFrom != null)
		{
			Control realized = _realized;
			if (realized != null)
			{
				IPageTransition transition = GetTransition();
				if (transition != null)
				{
					_transition = new CancellationTokenSource();
					transition.Start(_transitionFrom, realized, _realizedIndex > _transitionFromIndex, _transition.Token).ContinueWith(TransitionFinished, TaskScheduler.FromCurrentSynchronizationContext());
				}
			}
		}
		return result;
	}

	protected override IInputElement? GetControl(NavigationDirection direction, IInputElement? from, bool wrap)
	{
		return null;
	}

	protected internal override Control? ContainerFromIndex(int index)
	{
		if (index < 0 || index >= base.Items.Count)
		{
			return null;
		}
		if (index == _realizedIndex)
		{
			return _realized;
		}
		if (base.Items[index] is Control control && control.GetValue(RecycleKeyProperty) == s_itemIsItsOwnContainer)
		{
			return control;
		}
		return null;
	}

	protected internal override IEnumerable<Control>? GetRealizedContainers()
	{
		if (_realized == null)
		{
			return null;
		}
		return new Control[1] { _realized };
	}

	protected internal override int IndexFromContainer(Control container)
	{
		if (container != _realized)
		{
			return -1;
		}
		return _realizedIndex;
	}

	protected internal override Control? ScrollIntoView(int index)
	{
		return null;
	}

	protected override void OnItemsChanged(IReadOnlyList<object?> items, NotifyCollectionChangedEventArgs e)
	{
		base.OnItemsChanged(items, e);
		switch (e.Action)
		{
		case NotifyCollectionChangedAction.Add:
			Add(e.NewStartingIndex, e.NewItems.Count);
			break;
		case NotifyCollectionChangedAction.Remove:
			Remove(e.OldStartingIndex, e.OldItems.Count);
			break;
		case NotifyCollectionChangedAction.Replace:
		case NotifyCollectionChangedAction.Move:
			Remove(e.OldStartingIndex, e.OldItems.Count);
			Add(e.NewStartingIndex, e.NewItems.Count);
			break;
		case NotifyCollectionChangedAction.Reset:
			if (_realized != null)
			{
				RecycleElement(_realized);
				_realized = null;
				_realizedIndex = -1;
			}
			break;
		}
		InvalidateMeasure();
		void Add(int index, int count)
		{
			if (index <= _realizedIndex)
			{
				_realizedIndex += count;
			}
		}
		void Remove(int index, int count)
		{
			int num = index + (count - 1);
			if (_realized != null && index <= _realizedIndex && num >= _realizedIndex)
			{
				RecycleElement(_realized);
				_realized = null;
				_realizedIndex = -1;
			}
			else if (index < _realizedIndex)
			{
				_realizedIndex -= count;
			}
		}
	}

	private Control GetOrCreateElement(IReadOnlyList<object?> items, int index)
	{
		Control control = GetRealizedElement(index);
		if (control == null)
		{
			object item = items[index];
			control = ((!base.ItemContainerGenerator.NeedsContainer(item, index, out object recycleKey)) ? GetItemAsOwnContainer(item, index) : (GetRecycledElement(item, index, recycleKey) ?? CreateElement(item, index, recycleKey)));
		}
		return control;
	}

	private Control? GetRealizedElement(int index)
	{
		if (_realizedIndex != index)
		{
			return null;
		}
		return _realized;
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

	private void RecycleElement(Control element)
	{
		object value = element.GetValue(RecycleKeyProperty);
		if (value == s_itemIsItsOwnContainer)
		{
			element.IsVisible = false;
			return;
		}
		base.ItemContainerGenerator.ClearItemContainer(element);
		if (_recyclePool == null)
		{
			_recyclePool = new Dictionary<object, Stack<Control>>();
		}
		if (!_recyclePool.TryGetValue(value, out Stack<Control> value2))
		{
			value2 = new Stack<Control>();
			_recyclePool.Add(value, value2);
		}
		value2.Push(element);
		element.IsVisible = false;
	}

	private IPageTransition? GetTransition()
	{
		return (base.ItemsControl as Carousel)?.PageTransition;
	}

	private void TransitionFinished(Task task)
	{
		if (!task.IsCanceled)
		{
			if (_transitionFrom != null)
			{
				RecycleElement(_transitionFrom);
			}
			_transition = null;
			_transitionFrom = null;
			_transitionFromIndex = -1;
		}
	}
}
