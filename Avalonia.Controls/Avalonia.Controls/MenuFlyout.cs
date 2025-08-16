using System.Collections;
using System.ComponentModel;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using Avalonia.Styling;

namespace Avalonia.Controls;

public class MenuFlyout : PopupFlyoutBase
{
	public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty = AvaloniaProperty.Register<MenuFlyout, IEnumerable>("ItemsSource");

	public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty = AvaloniaProperty.Register<MenuFlyout, IDataTemplate>("ItemTemplate");

	public static readonly StyledProperty<ControlTheme?> ItemContainerThemeProperty = ItemsControl.ItemContainerThemeProperty.AddOwner<MenuFlyout>();

	public static readonly StyledProperty<ControlTheme?> FlyoutPresenterThemeProperty = Flyout.FlyoutPresenterThemeProperty.AddOwner<MenuFlyout>();

	private Classes? _classes;

	public Classes FlyoutPresenterClasses => _classes ?? (_classes = new Classes());

	[Content]
	public ItemCollection Items { get; }

	public IEnumerable? ItemsSource
	{
		get
		{
			return GetValue(ItemsSourceProperty);
		}
		set
		{
			SetValue(ItemsSourceProperty, value);
		}
	}

	public IDataTemplate? ItemTemplate
	{
		get
		{
			return GetValue(ItemTemplateProperty);
		}
		set
		{
			SetValue(ItemTemplateProperty, value);
		}
	}

	public ControlTheme? ItemContainerTheme
	{
		get
		{
			return GetValue(ItemContainerThemeProperty);
		}
		set
		{
			SetValue(ItemContainerThemeProperty, value);
		}
	}

	public ControlTheme? FlyoutPresenterTheme
	{
		get
		{
			return GetValue(FlyoutPresenterThemeProperty);
		}
		set
		{
			SetValue(FlyoutPresenterThemeProperty, value);
		}
	}

	public MenuFlyout()
	{
		Items = new ItemCollection();
	}

	protected override Control CreatePresenter()
	{
		return new MenuFlyoutPresenter
		{
			ItemsSource = Items,
			[!ItemsControl.ItemTemplateProperty] = base[!ItemTemplateProperty],
			[!ItemsControl.ItemContainerThemeProperty] = base[!ItemContainerThemeProperty]
		};
	}

	protected override void OnOpening(CancelEventArgs args)
	{
		Control child = base.Popup.Child;
		if (child != null)
		{
			if (_classes != null)
			{
				PopupFlyoutBase.SetPresenterClasses(child, FlyoutPresenterClasses);
			}
			ControlTheme flyoutPresenterTheme = FlyoutPresenterTheme;
			if (flyoutPresenterTheme != null)
			{
				child.SetValue(StyledElement.ThemeProperty, flyoutPresenterTheme);
			}
		}
		base.OnOpening(args);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == ItemsSourceProperty)
		{
			Items.SetItemsSource(change.GetNewValue<IEnumerable>());
		}
	}
}
