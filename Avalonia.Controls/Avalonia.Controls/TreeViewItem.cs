using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace Avalonia.Controls;

[TemplatePart("PART_Header", typeof(Control))]
[PseudoClasses(new string[] { ":pressed", ":selected" })]
public class TreeViewItem : HeaderedItemsControl, ISelectable
{
	public static readonly StyledProperty<bool> IsExpandedProperty;

	public static readonly StyledProperty<bool> IsSelectedProperty;

	public static readonly DirectProperty<TreeViewItem, int> LevelProperty;

	private static readonly FuncTemplate<Panel?> DefaultPanel;

	private TreeView? _treeView;

	private Control? _header;

	private Control? _headerPresenter;

	private int _level;

	private bool _templateApplied;

	private bool _deferredBringIntoViewFlag;

	public bool IsExpanded
	{
		get
		{
			return GetValue(IsExpandedProperty);
		}
		set
		{
			SetValue(IsExpandedProperty, value);
		}
	}

	public bool IsSelected
	{
		get
		{
			return GetValue(IsSelectedProperty);
		}
		set
		{
			SetValue(IsSelectedProperty, value);
		}
	}

	public int Level
	{
		get
		{
			return _level;
		}
		private set
		{
			SetAndRaise(LevelProperty, ref _level, value);
		}
	}

	internal TreeView? TreeViewOwner => _treeView;

	static TreeViewItem()
	{
		IsExpandedProperty = AvaloniaProperty.Register<TreeViewItem, bool>("IsExpanded", defaultValue: false, inherits: false, BindingMode.TwoWay);
		IsSelectedProperty = SelectingItemsControl.IsSelectedProperty.AddOwner<TreeViewItem>();
		LevelProperty = AvaloniaProperty.RegisterDirect("Level", (TreeViewItem o) => o.Level, null, 0);
		DefaultPanel = new FuncTemplate<Panel>(() => new StackPanel());
		SelectableMixin.Attach<TreeViewItem>(IsSelectedProperty);
		PressedMixin.Attach<TreeViewItem>();
		InputElement.FocusableProperty.OverrideDefaultValue<TreeViewItem>(defaultValue: true);
		ItemsControl.ItemsPanelProperty.OverrideDefaultValue<TreeViewItem>(DefaultPanel);
		Control.RequestBringIntoViewEvent.AddClassHandler(delegate(TreeViewItem x, RequestBringIntoViewEventArgs e)
		{
			x.OnRequestBringIntoView(e);
		});
	}

	protected internal override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
	{
		return EnsureTreeView().CreateContainerForItemOverride(item, index, recycleKey);
	}

	protected internal override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
	{
		return EnsureTreeView().NeedsContainerOverride(item, index, out recycleKey);
	}

	protected internal override void PrepareContainerForItemOverride(Control container, object? item, int index)
	{
		EnsureTreeView().PrepareContainerForItemOverride(container, item, index);
	}

	protected internal override void ContainerForItemPreparedOverride(Control container, object? item, int index)
	{
		EnsureTreeView().ContainerForItemPreparedOverride(container, item, index);
	}

	protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		base.OnAttachedToLogicalTree(e);
		_treeView = this.GetLogicalAncestors().OfType<TreeView>().FirstOrDefault();
		Level = CalculateDistanceFromLogicalParent<TreeView>(this) - 1;
		if (base.ItemTemplate == null && _treeView?.ItemTemplate != null)
		{
			SetCurrentValue(ItemsControl.ItemTemplateProperty, _treeView.ItemTemplate);
		}
		if (base.ItemContainerTheme == null && _treeView?.ItemContainerTheme != null)
		{
			SetCurrentValue(ItemsControl.ItemContainerThemeProperty, _treeView.ItemContainerTheme);
		}
	}

	protected virtual void OnRequestBringIntoView(RequestBringIntoViewEventArgs e)
	{
		if (e.TargetObject != this)
		{
			return;
		}
		if (!_templateApplied)
		{
			_deferredBringIntoViewFlag = true;
		}
		else if (_header != null)
		{
			Matrix? matrix = _header.TransformToVisual(this);
			if (matrix.HasValue)
			{
				Rect rect = new Rect(_header.Bounds.Size);
				Rect targetRect = rect.TransformToAABB(matrix.Value);
				e.TargetRect = targetRect;
			}
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (!e.Handled)
		{
			Func<TreeViewItem, bool> func = e.Key switch
			{
				Key.Left => ApplyToItemOrRecursivelyIfCtrl(FocusAwareCollapseItem, e.KeyModifiers), 
				Key.Right => ApplyToItemOrRecursivelyIfCtrl(ExpandItem, e.KeyModifiers), 
				Key.Return => ApplyToItemOrRecursivelyIfCtrl(IsExpanded ? new Func<TreeViewItem, bool>(CollapseItem) : new Func<TreeViewItem, bool>(ExpandItem), e.KeyModifiers), 
				Key.Subtract => FocusAwareCollapseItem, 
				Key.Add => ExpandItem, 
				Key.Divide => ApplyToSubtree(CollapseItem), 
				Key.Multiply => ApplyToSubtree(ExpandItem), 
				_ => null, 
			};
			if (func != null)
			{
				e.Handled = func(this);
			}
		}
		static Func<TreeViewItem, bool> ApplyToItemOrRecursivelyIfCtrl(Func<TreeViewItem, bool> f, KeyModifiers keyModifiers)
		{
			if (keyModifiers.HasAllFlags(KeyModifiers.Control))
			{
				return ApplyToSubtree(f);
			}
			return f;
		}
		static Func<TreeViewItem, bool> ApplyToSubtree(Func<TreeViewItem, bool> f)
		{
			return (TreeViewItem t) => (from treeViewItem in SubTree(t).ToList()
				select f(treeViewItem)).Aggregate(seed: false, (bool p, bool c) => p || c);
		}
		static bool CollapseItem(TreeViewItem treeViewItem)
		{
			if (treeViewItem.ItemCount > 0 && treeViewItem.IsExpanded)
			{
				treeViewItem.SetCurrentValue(IsExpandedProperty, value: false);
				return true;
			}
			return false;
		}
		static bool ExpandItem(TreeViewItem treeViewItem)
		{
			if (treeViewItem.ItemCount > 0 && !treeViewItem.IsExpanded)
			{
				treeViewItem.SetCurrentValue(IsExpandedProperty, value: true);
				return true;
			}
			return false;
		}
		static bool FocusAwareCollapseItem(TreeViewItem treeViewItem)
		{
			if (treeViewItem.ItemCount > 0 && treeViewItem.IsExpanded)
			{
				if (treeViewItem.IsFocused)
				{
					treeViewItem.SetCurrentValue(IsExpandedProperty, value: false);
				}
				else
				{
					treeViewItem.Focus(NavigationMethod.Directional);
				}
				return true;
			}
			return false;
		}
		static IEnumerable<TreeViewItem> SubTree(TreeViewItem treeViewItem)
		{
			return new TreeViewItem[1] { treeViewItem }.Concat(treeViewItem.LogicalChildren.OfType<TreeViewItem>().SelectMany((TreeViewItem child) => SubTree(child)));
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		InputElement headerPresenter = _headerPresenter;
		if (headerPresenter != null)
		{
			headerPresenter.DoubleTapped -= HeaderDoubleTapped;
		}
		_header = e.NameScope.Find<Control>("PART_Header");
		_headerPresenter = e.NameScope.Find<Control>("PART_HeaderPresenter");
		_templateApplied = true;
		InputElement headerPresenter2 = _headerPresenter;
		if (headerPresenter2 != null)
		{
			headerPresenter2.DoubleTapped += HeaderDoubleTapped;
		}
		if (_deferredBringIntoViewFlag)
		{
			_deferredBringIntoViewFlag = false;
			Dispatcher.UIThread.Post(((Control)this).BringIntoView);
		}
	}

	protected virtual void OnHeaderDoubleTapped(TappedEventArgs e)
	{
		if (base.ItemCount > 0)
		{
			SetCurrentValue(IsExpandedProperty, !IsExpanded);
			e.Handled = true;
		}
	}

	private protected override void OnItemsViewCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		base.OnItemsViewCollectionChanged(sender, e);
		if (_treeView == null)
		{
			return;
		}
		switch (e.Action)
		{
		case NotifyCollectionChangedAction.Remove:
		case NotifyCollectionChangedAction.Replace:
		{
			foreach (object oldItem in e.OldItems)
			{
				_treeView.SelectedItems.Remove(oldItem);
			}
			break;
		}
		case NotifyCollectionChangedAction.Reset:
		{
			foreach (Control realizedContainer in GetRealizedContainers())
			{
				if (realizedContainer is TreeViewItem { IsSelected: not false })
				{
					_treeView.SelectedItems.Remove(realizedContainer.DataContext);
				}
			}
			break;
		}
		}
	}

	private static int CalculateDistanceFromLogicalParent<T>(ILogical? logical, int @default = -1) where T : class
	{
		int num = 0;
		while (logical != null && !(logical is T))
		{
			num++;
			logical = logical.LogicalParent;
		}
		if (logical == null)
		{
			return @default;
		}
		return num;
	}

	private TreeView EnsureTreeView()
	{
		return _treeView ?? throw new InvalidOperationException("The TreeViewItem is not part of a TreeView.");
	}

	private void HeaderDoubleTapped(object? sender, TappedEventArgs e)
	{
		OnHeaderDoubleTapped(e);
	}
}
