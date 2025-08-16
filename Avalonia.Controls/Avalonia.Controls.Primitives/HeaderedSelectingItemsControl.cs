using System;
using Avalonia.Collections;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.LogicalTree;

namespace Avalonia.Controls.Primitives;

public class HeaderedSelectingItemsControl : SelectingItemsControl, IContentPresenterHost
{
	private IDisposable? _itemsBinding;

	private ItemsControl? _prepareItemContainerOnAttach;

	public static readonly StyledProperty<object?> HeaderProperty;

	public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty;

	public object? Header
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

	public IDataTemplate? HeaderTemplate
	{
		get
		{
			return GetValue(HeaderTemplateProperty);
		}
		set
		{
			SetValue(HeaderTemplateProperty, value);
		}
	}

	public ContentPresenter? HeaderPresenter { get; private set; }

	IAvaloniaList<ILogical> IContentPresenterHost.LogicalChildren => base.LogicalChildren;

	static HeaderedSelectingItemsControl()
	{
		HeaderProperty = HeaderedContentControl.HeaderProperty.AddOwner<HeaderedSelectingItemsControl>();
		HeaderTemplateProperty = HeaderedItemsControl.HeaderTemplateProperty.AddOwner<HeaderedSelectingItemsControl>();
		HeaderProperty.Changed.AddClassHandler(delegate(HeaderedSelectingItemsControl x, AvaloniaPropertyChangedEventArgs e)
		{
			x.HeaderChanged(e);
		});
	}

	bool IContentPresenterHost.RegisterContentPresenter(ContentPresenter presenter)
	{
		return RegisterContentPresenter(presenter);
	}

	protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		base.OnAttachedToLogicalTree(e);
		if (_prepareItemContainerOnAttach != null)
		{
			PrepareItemContainer(_prepareItemContainerOnAttach);
			_prepareItemContainerOnAttach = null;
		}
	}

	protected virtual bool RegisterContentPresenter(ContentPresenter presenter)
	{
		if (presenter.Name == "PART_HeaderPresenter")
		{
			HeaderPresenter = presenter;
			return true;
		}
		return false;
	}

	internal void PrepareItemContainer(ItemsControl parent)
	{
		_itemsBinding?.Dispose();
		_itemsBinding = null;
		object header = Header;
		if (header == null)
		{
			_prepareItemContainerOnAttach = null;
			return;
		}
		IDataTemplate dataTemplate = HeaderTemplate ?? parent.ItemTemplate;
		if (dataTemplate == null)
		{
			if (((ILogical)this).IsAttachedToLogicalTree)
			{
				dataTemplate = this.FindDataTemplate(header);
			}
			else
			{
				_prepareItemContainerOnAttach = parent;
			}
		}
		if (dataTemplate is ITreeDataTemplate treeDataTemplate && treeDataTemplate.Match(header))
		{
			InstancedBinding instancedBinding = treeDataTemplate.ItemsSelector(header);
			if (instancedBinding != null)
			{
				_itemsBinding = BindingOperations.Apply(this, ItemsControl.ItemsSourceProperty, instancedBinding, null);
			}
		}
	}

	private void HeaderChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.OldValue is ILogical item)
		{
			base.LogicalChildren.Remove(item);
		}
		if (e.NewValue is ILogical item2)
		{
			base.LogicalChildren.Add(item2);
		}
	}
}
