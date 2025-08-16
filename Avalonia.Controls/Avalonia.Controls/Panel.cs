using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Reactive;

namespace Avalonia.Controls;

public class Panel : Control, IChildIndexProvider
{
	public static readonly StyledProperty<IBrush?> BackgroundProperty;

	private EventHandler<ChildIndexChangedEventArgs>? _childIndexChanged;

	[Content]
	public Controls Children { get; } = new Controls();

	public IBrush? Background
	{
		get
		{
			return GetValue(BackgroundProperty);
		}
		set
		{
			SetValue(BackgroundProperty, value);
		}
	}

	public bool IsItemsHost { get; internal set; }

	event EventHandler<ChildIndexChangedEventArgs>? IChildIndexProvider.ChildIndexChanged
	{
		add
		{
			if (_childIndexChanged == null)
			{
				Children.PropertyChanged += ChildrenPropertyChanged;
			}
			_childIndexChanged = (EventHandler<ChildIndexChangedEventArgs>)Delegate.Combine(_childIndexChanged, value);
		}
		remove
		{
			_childIndexChanged = (EventHandler<ChildIndexChangedEventArgs>)Delegate.Remove(_childIndexChanged, value);
			if (_childIndexChanged == null)
			{
				Children.PropertyChanged -= ChildrenPropertyChanged;
			}
		}
	}

	static Panel()
	{
		BackgroundProperty = Border.BackgroundProperty.AddOwner<Panel>();
		Visual.AffectsRender<Panel>(new AvaloniaProperty[1] { BackgroundProperty });
	}

	public Panel()
	{
		Children.CollectionChanged += ChildrenChanged;
	}

	public sealed override void Render(DrawingContext context)
	{
		IBrush background = Background;
		if (background != null)
		{
			Size size = base.Bounds.Size;
			context.FillRectangle(background, new Rect(size));
		}
		base.Render(context);
	}

	protected static void AffectsParentArrange<TPanel>(params AvaloniaProperty[] properties) where TPanel : Panel
	{
		AnonymousObserver<AvaloniaPropertyChangedEventArgs> observer = new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(delegate(AvaloniaPropertyChangedEventArgs e)
		{
			AffectsParentArrangeInvalidate<TPanel>(e);
		});
		for (int i = 0; i < properties.Length; i++)
		{
			properties[i].Changed.Subscribe(observer);
		}
	}

	protected static void AffectsParentMeasure<TPanel>(params AvaloniaProperty[] properties) where TPanel : Panel
	{
		AnonymousObserver<AvaloniaPropertyChangedEventArgs> observer = new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(delegate(AvaloniaPropertyChangedEventArgs e)
		{
			AffectsParentMeasureInvalidate<TPanel>(e);
		});
		for (int i = 0; i < properties.Length; i++)
		{
			properties[i].Changed.Subscribe(observer);
		}
	}

	protected virtual void ChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		switch (e.Action)
		{
		case NotifyCollectionChangedAction.Add:
			if (!IsItemsHost)
			{
				base.LogicalChildren.InsertRange(e.NewStartingIndex, e.NewItems.OfType<Control>().ToList());
			}
			base.VisualChildren.InsertRange(e.NewStartingIndex, e.NewItems.OfType<Visual>());
			break;
		case NotifyCollectionChangedAction.Move:
			if (!IsItemsHost)
			{
				base.LogicalChildren.MoveRange(e.OldStartingIndex, e.OldItems.Count, e.NewStartingIndex);
			}
			base.VisualChildren.MoveRange(e.OldStartingIndex, e.OldItems.Count, e.NewStartingIndex);
			break;
		case NotifyCollectionChangedAction.Remove:
			if (!IsItemsHost)
			{
				base.LogicalChildren.RemoveAll(e.OldItems.OfType<Control>().ToList());
			}
			base.VisualChildren.RemoveAll(e.OldItems.OfType<Visual>());
			break;
		case NotifyCollectionChangedAction.Replace:
		{
			for (int i = 0; i < e.OldItems.Count; i++)
			{
				int index = i + e.OldStartingIndex;
				Control value = (Control)e.NewItems[i];
				if (!IsItemsHost)
				{
					base.LogicalChildren[index] = value;
				}
				base.VisualChildren[index] = value;
			}
			break;
		}
		case NotifyCollectionChangedAction.Reset:
			throw new NotSupportedException();
		}
		_childIndexChanged?.Invoke(this, ChildIndexChangedEventArgs.ChildIndexesReset);
		InvalidateMeasureOnChildrenChanged();
	}

	private protected virtual void InvalidateMeasureOnChildrenChanged()
	{
		InvalidateMeasure();
	}

	private void ChildrenPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == "Count" || e.PropertyName == null)
		{
			_childIndexChanged?.Invoke(this, ChildIndexChangedEventArgs.TotalCountChanged);
		}
	}

	private static void AffectsParentArrangeInvalidate<TPanel>(AvaloniaPropertyChangedEventArgs e) where TPanel : Panel
	{
		((e.Sender as Control)?.VisualParent as TPanel)?.InvalidateArrange();
	}

	private static void AffectsParentMeasureInvalidate<TPanel>(AvaloniaPropertyChangedEventArgs e) where TPanel : Panel
	{
		((e.Sender as Control)?.VisualParent as TPanel)?.InvalidateMeasure();
	}

	int IChildIndexProvider.GetChildIndex(ILogical child)
	{
		if (!(child is Control item))
		{
			return -1;
		}
		return Children.IndexOf(item);
	}

	bool IChildIndexProvider.TryGetTotalCount(out int count)
	{
		count = Children.Count;
		return true;
	}
}
