using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AvaloniaEdit.CodeCompletion;

public class OverloadViewer : TemplatedControl
{
	public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<OverloadViewer, string>("Text");

	public static readonly StyledProperty<IOverloadProvider> ProviderProperty = AvaloniaProperty.Register<OverloadViewer, IOverloadProvider>("Provider");

	public string Text
	{
		get
		{
			return GetValue(TextProperty);
		}
		set
		{
			SetValue(TextProperty, value);
		}
	}

	public IOverloadProvider Provider
	{
		get
		{
			return GetValue(ProviderProperty);
		}
		set
		{
			SetValue(ProviderProperty, value);
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs args)
	{
		base.OnApplyTemplate(args);
		Button button = args.NameScope.Find<Button>("PART_UP");
		if (button != null)
		{
			button.Click += delegate(object sender, RoutedEventArgs e)
			{
				e.Handled = true;
				ChangeIndex(-1);
			};
		}
		Button button2 = args.NameScope.Find<Button>("PART_DOWN");
		if (button2 != null)
		{
			button2.Click += delegate(object sender, RoutedEventArgs e)
			{
				e.Handled = true;
				ChangeIndex(1);
			};
		}
	}

	public void ChangeIndex(int relativeIndexChange)
	{
		IOverloadProvider provider = Provider;
		if (provider != null)
		{
			int num = provider.SelectedIndex + relativeIndexChange;
			if (num < 0)
			{
				num = provider.Count - 1;
			}
			if (num >= provider.Count)
			{
				num = 0;
			}
			provider.SelectedIndex = num;
		}
	}
}
