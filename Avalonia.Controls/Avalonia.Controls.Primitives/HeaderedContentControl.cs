using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.LogicalTree;

namespace Avalonia.Controls.Primitives;

public class HeaderedContentControl : ContentControl, IHeadered
{
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

	public ContentPresenter? HeaderPresenter { get; private set; }

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

	static HeaderedContentControl()
	{
		HeaderProperty = AvaloniaProperty.Register<HeaderedContentControl, object>("Header");
		HeaderTemplateProperty = AvaloniaProperty.Register<HeaderedContentControl, IDataTemplate>("HeaderTemplate");
		HeaderProperty.Changed.AddClassHandler(delegate(HeaderedContentControl x, AvaloniaPropertyChangedEventArgs e)
		{
			x.HeaderChanged(e);
		});
	}

	protected override bool RegisterContentPresenter(ContentPresenter presenter)
	{
		bool result = base.RegisterContentPresenter(presenter);
		if (presenter.Name == "PART_HeaderPresenter")
		{
			HeaderPresenter = presenter;
			result = true;
		}
		return result;
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
