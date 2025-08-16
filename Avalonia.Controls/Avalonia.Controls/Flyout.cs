using System.ComponentModel;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using Avalonia.Styling;

namespace Avalonia.Controls;

public class Flyout : PopupFlyoutBase
{
	public static readonly StyledProperty<object> ContentProperty = AvaloniaProperty.Register<Flyout, object>("Content");

	private Classes? _classes;

	public static readonly StyledProperty<ControlTheme?> FlyoutPresenterThemeProperty = AvaloniaProperty.Register<Flyout, ControlTheme>("FlyoutPresenterTheme");

	public Classes FlyoutPresenterClasses => _classes ?? (_classes = new Classes());

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

	[Content]
	public object Content
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

	protected override Control CreatePresenter()
	{
		return new FlyoutPresenter { [!ContentControl.ContentProperty] = base[!ContentProperty] };
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
}
