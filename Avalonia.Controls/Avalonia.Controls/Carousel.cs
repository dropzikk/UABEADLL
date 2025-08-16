using Avalonia.Animation;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace Avalonia.Controls;

public class Carousel : SelectingItemsControl
{
	public static readonly StyledProperty<IPageTransition?> PageTransitionProperty;

	private static readonly FuncTemplate<Panel?> DefaultPanel;

	private IScrollable? _scroller;

	public IPageTransition? PageTransition
	{
		get
		{
			return GetValue(PageTransitionProperty);
		}
		set
		{
			SetValue(PageTransitionProperty, value);
		}
	}

	static Carousel()
	{
		PageTransitionProperty = AvaloniaProperty.Register<Carousel, IPageTransition>("PageTransition");
		DefaultPanel = new FuncTemplate<Panel>(() => new VirtualizingCarouselPanel());
		SelectingItemsControl.SelectionModeProperty.OverrideDefaultValue<Carousel>(SelectionMode.AlwaysSelected);
		ItemsControl.ItemsPanelProperty.OverrideDefaultValue<Carousel>(DefaultPanel);
	}

	public void Next()
	{
		if (base.SelectedIndex < base.ItemCount - 1)
		{
			int selectedIndex = base.SelectedIndex + 1;
			base.SelectedIndex = selectedIndex;
		}
	}

	public void Previous()
	{
		if (base.SelectedIndex > 0)
		{
			int selectedIndex = base.SelectedIndex - 1;
			base.SelectedIndex = selectedIndex;
		}
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		Size result = base.ArrangeOverride(finalSize);
		if (_scroller != null)
		{
			_scroller.Offset = new Vector(base.SelectedIndex, 0.0);
		}
		return result;
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_scroller = e.NameScope.Find<IScrollable>("PART_ScrollViewer");
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == SelectingItemsControl.SelectedIndexProperty && _scroller != null)
		{
			int newValue = change.GetNewValue<int>();
			_scroller.Offset = new Vector(newValue, 0.0);
		}
	}
}
