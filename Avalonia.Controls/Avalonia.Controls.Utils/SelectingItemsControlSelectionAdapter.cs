using System;
using System.Collections;
using System.Linq;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace Avalonia.Controls.Utils;

public class SelectingItemsControlSelectionAdapter : ISelectionAdapter
{
	private SelectingItemsControl? _selector;

	private bool IgnoringSelectionChanged { get; set; }

	public SelectingItemsControl? SelectorControl
	{
		get
		{
			return _selector;
		}
		set
		{
			if (_selector != null)
			{
				_selector.SelectionChanged -= OnSelectionChanged;
				_selector.PointerReleased -= OnSelectorPointerReleased;
			}
			_selector = value;
			if (_selector != null)
			{
				_selector.SelectionChanged += OnSelectionChanged;
				_selector.PointerReleased += OnSelectorPointerReleased;
			}
		}
	}

	public object? SelectedItem
	{
		get
		{
			return SelectorControl?.SelectedItem;
		}
		set
		{
			IgnoringSelectionChanged = true;
			if (SelectorControl != null)
			{
				SelectorControl.SelectedItem = value;
			}
			if (value == null)
			{
				ResetScrollViewer();
			}
			IgnoringSelectionChanged = false;
		}
	}

	public IEnumerable? ItemsSource
	{
		get
		{
			return SelectorControl?.ItemsSource;
		}
		set
		{
			if (SelectorControl != null)
			{
				SelectorControl.ItemsSource = value;
			}
		}
	}

	public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

	public event EventHandler<RoutedEventArgs>? Commit;

	public event EventHandler<RoutedEventArgs>? Cancel;

	public SelectingItemsControlSelectionAdapter()
	{
	}

	public SelectingItemsControlSelectionAdapter(SelectingItemsControl selector)
	{
		SelectorControl = selector;
	}

	private void ResetScrollViewer()
	{
		if (SelectorControl != null)
		{
			ScrollViewer scrollViewer = SelectorControl.GetLogicalDescendants().OfType<ScrollViewer>().FirstOrDefault();
			if (scrollViewer != null)
			{
				scrollViewer.Offset = new Vector(0.0, 0.0);
			}
		}
	}

	private void OnSelectorPointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		if (e.InitialPressMouseButton == MouseButton.Left)
		{
			OnCommit();
		}
	}

	private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (!IgnoringSelectionChanged)
		{
			this.SelectionChanged?.Invoke(sender, e);
		}
	}

	protected void SelectedIndexIncrement()
	{
		if (SelectorControl != null)
		{
			SelectorControl.SelectedIndex = ((SelectorControl.SelectedIndex + 1 >= SelectorControl.ItemCount) ? (-1) : (SelectorControl.SelectedIndex + 1));
		}
	}

	protected void SelectedIndexDecrement()
	{
		if (SelectorControl != null)
		{
			int selectedIndex = SelectorControl.SelectedIndex;
			if (selectedIndex >= 0)
			{
				SelectorControl.SelectedIndex--;
			}
			else if (selectedIndex == -1)
			{
				SelectorControl.SelectedIndex = SelectorControl.ItemCount - 1;
			}
		}
	}

	public void HandleKeyDown(KeyEventArgs e)
	{
		switch (e.Key)
		{
		case Key.Return:
			OnCommit();
			e.Handled = true;
			break;
		case Key.Up:
			SelectedIndexDecrement();
			e.Handled = true;
			break;
		case Key.Down:
			if ((e.KeyModifiers & KeyModifiers.Alt) == 0)
			{
				SelectedIndexIncrement();
				e.Handled = true;
			}
			break;
		case Key.Escape:
			OnCancel();
			e.Handled = true;
			break;
		}
	}

	protected virtual void OnCommit()
	{
		OnCommit(this, new RoutedEventArgs());
	}

	private void OnCommit(object? sender, RoutedEventArgs e)
	{
		this.Commit?.Invoke(sender, e);
		AfterAdapterAction();
	}

	protected virtual void OnCancel()
	{
		OnCancel(this, new RoutedEventArgs());
	}

	private void OnCancel(object? sender, RoutedEventArgs e)
	{
		this.Cancel?.Invoke(sender, e);
		AfterAdapterAction();
	}

	private void AfterAdapterAction()
	{
		IgnoringSelectionChanged = true;
		if (SelectorControl != null)
		{
			SelectorControl.SelectedItem = null;
			SelectorControl.SelectedIndex = -1;
		}
		IgnoringSelectionChanged = false;
	}
}
