using Avalonia.Collections;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace Avalonia.Controls;

[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
public class ContentControl : TemplatedControl, IContentControl, IContentPresenterHost
{
	public static readonly StyledProperty<object?> ContentProperty;

	public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty;

	public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty;

	public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty;

	[Content]
	[DependsOn("ContentTemplate")]
	public object? Content
	{
		get
		{
			return GetValue(ContentProperty);
		}
		set
		{
			SetValue(ContentProperty, value);
		}
	}

	public IDataTemplate? ContentTemplate
	{
		get
		{
			return GetValue(ContentTemplateProperty);
		}
		set
		{
			SetValue(ContentTemplateProperty, value);
		}
	}

	public ContentPresenter? Presenter { get; private set; }

	public HorizontalAlignment HorizontalContentAlignment
	{
		get
		{
			return GetValue(HorizontalContentAlignmentProperty);
		}
		set
		{
			SetValue(HorizontalContentAlignmentProperty, value);
		}
	}

	public VerticalAlignment VerticalContentAlignment
	{
		get
		{
			return GetValue(VerticalContentAlignmentProperty);
		}
		set
		{
			SetValue(VerticalContentAlignmentProperty, value);
		}
	}

	IAvaloniaList<ILogical> IContentPresenterHost.LogicalChildren => base.LogicalChildren;

	static ContentControl()
	{
		ContentProperty = AvaloniaProperty.Register<ContentControl, object>("Content");
		ContentTemplateProperty = AvaloniaProperty.Register<ContentControl, IDataTemplate>("ContentTemplate");
		HorizontalContentAlignmentProperty = AvaloniaProperty.Register<ContentControl, HorizontalAlignment>("HorizontalContentAlignment", HorizontalAlignment.Stretch);
		VerticalContentAlignmentProperty = AvaloniaProperty.Register<ContentControl, VerticalAlignment>("VerticalContentAlignment", VerticalAlignment.Stretch);
		ContentProperty.Changed.AddClassHandler(delegate(ContentControl x, AvaloniaPropertyChangedEventArgs e)
		{
			x.ContentChanged(e);
		});
	}

	bool IContentPresenterHost.RegisterContentPresenter(ContentPresenter presenter)
	{
		return RegisterContentPresenter(presenter);
	}

	protected virtual bool RegisterContentPresenter(ContentPresenter presenter)
	{
		if (presenter.Name == "PART_ContentPresenter")
		{
			Presenter = presenter;
			return true;
		}
		return false;
	}

	private void ContentChanged(AvaloniaPropertyChangedEventArgs e)
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
