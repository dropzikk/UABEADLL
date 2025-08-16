using System;
using System.Collections.Generic;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Avalonia.Controls.Presenters;

public class ItemsPresenter : Control, ILogicalScrollable, IScrollable
{
	public static readonly StyledProperty<ITemplate<Panel?>> ItemsPanelProperty = Avalonia.Controls.ItemsControl.ItemsPanelProperty.AddOwner<ItemsPresenter>();

	private PanelContainerGenerator? _generator;

	private ILogicalScrollable? _logicalScrollable;

	private EventHandler? _scrollInvalidated;

	public ITemplate<Panel?> ItemsPanel
	{
		get
		{
			return GetValue(ItemsPanelProperty);
		}
		set
		{
			SetValue(ItemsPanelProperty, value);
		}
	}

	public Panel? Panel { get; private set; }

	internal ItemsControl? ItemsControl { get; private set; }

	bool ILogicalScrollable.CanHorizontallyScroll
	{
		get
		{
			return _logicalScrollable?.CanHorizontallyScroll ?? false;
		}
		set
		{
			if (_logicalScrollable != null)
			{
				_logicalScrollable.CanHorizontallyScroll = value;
			}
		}
	}

	bool ILogicalScrollable.CanVerticallyScroll
	{
		get
		{
			return _logicalScrollable?.CanVerticallyScroll ?? false;
		}
		set
		{
			if (_logicalScrollable != null)
			{
				_logicalScrollable.CanVerticallyScroll = value;
			}
		}
	}

	Vector IScrollable.Offset
	{
		get
		{
			return _logicalScrollable?.Offset ?? default(Vector);
		}
		set
		{
			if (_logicalScrollable != null)
			{
				_logicalScrollable.Offset = value;
			}
		}
	}

	bool ILogicalScrollable.IsLogicalScrollEnabled => _logicalScrollable?.IsLogicalScrollEnabled ?? false;

	Size ILogicalScrollable.ScrollSize => _logicalScrollable?.ScrollSize ?? default(Size);

	Size ILogicalScrollable.PageScrollSize => _logicalScrollable?.PageScrollSize ?? default(Size);

	Size IScrollable.Extent => _logicalScrollable?.Extent ?? default(Size);

	Size IScrollable.Viewport => _logicalScrollable?.Viewport ?? default(Size);

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

	public sealed override void ApplyTemplate()
	{
		if (Panel != null || ItemsControl == null)
		{
			return;
		}
		if (_logicalScrollable != null)
		{
			_logicalScrollable.ScrollInvalidated -= OnLogicalScrollInvalidated;
		}
		Panel = ItemsPanel.Build();
		if (Panel != null)
		{
			Panel.TemplatedParent = base.TemplatedParent;
			Panel.IsItemsHost = true;
			base.LogicalChildren.Add(Panel);
			base.VisualChildren.Add(Panel);
			if (Panel is VirtualizingPanel virtualizingPanel)
			{
				virtualizingPanel.Attach(ItemsControl);
			}
			else
			{
				CreateSimplePanelGenerator();
			}
			_logicalScrollable = Panel as ILogicalScrollable;
			if (_logicalScrollable != null)
			{
				_logicalScrollable.ScrollInvalidated += OnLogicalScrollInvalidated;
			}
		}
	}

	bool ILogicalScrollable.BringIntoView(Control target, Rect targetRect)
	{
		return _logicalScrollable?.BringIntoView(target, targetRect) ?? false;
	}

	Control? ILogicalScrollable.GetControlInDirection(NavigationDirection direction, Control? from)
	{
		return _logicalScrollable?.GetControlInDirection(direction, from);
	}

	void ILogicalScrollable.RaiseScrollInvalidated(EventArgs e)
	{
		_scrollInvalidated?.Invoke(this, e);
	}

	internal void ScrollIntoView(int index)
	{
		if (Panel is VirtualizingPanel virtualizingPanel)
		{
			virtualizingPanel.ScrollIntoView(index);
		}
		else if (index >= 0 && index < Panel?.Children.Count)
		{
			Panel.Children[index].BringIntoView();
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == StyledElement.TemplatedParentProperty)
		{
			ResetState();
			ItemsControl = null;
			if (change.NewValue is ItemsControl itemsControl)
			{
				ItemsControl = itemsControl;
				ItemsControl.RegisterItemsPresenter(this);
			}
		}
		else if (change.Property == ItemsPanelProperty)
		{
			ResetState();
			InvalidateMeasure();
		}
	}

	internal void Refresh()
	{
		if (Panel is VirtualizingPanel virtualizingPanel)
		{
			virtualizingPanel.Refresh();
		}
		else
		{
			_generator?.Refresh();
		}
	}

	private void ResetState()
	{
		_generator?.Dispose();
		_generator = null;
		base.LogicalChildren.Clear();
		base.VisualChildren.Clear();
		(Panel as VirtualizingPanel)?.Detach();
		Panel = null;
	}

	private void CreateSimplePanelGenerator()
	{
		if (ItemsControl != null && Panel != null)
		{
			_generator?.Dispose();
			_generator = new PanelContainerGenerator(this);
		}
	}

	internal Control? ContainerFromIndex(int index)
	{
		if (Panel is VirtualizingPanel virtualizingPanel)
		{
			return virtualizingPanel.ContainerFromIndex(index);
		}
		if (index < 0 || !(index < Panel?.Children.Count))
		{
			return null;
		}
		return Panel.Children[index];
	}

	internal IEnumerable<Control>? GetRealizedContainers()
	{
		if (Panel is VirtualizingPanel virtualizingPanel)
		{
			return virtualizingPanel.GetRealizedContainers();
		}
		return Panel?.Children;
	}

	internal int IndexFromContainer(Control container)
	{
		if (Panel is VirtualizingPanel virtualizingPanel)
		{
			return virtualizingPanel.IndexFromContainer(container);
		}
		return Panel?.Children.IndexOf(container) ?? (-1);
	}

	private void OnLogicalScrollInvalidated(object? sender, EventArgs e)
	{
		_scrollInvalidated?.Invoke(this, e);
	}
}
