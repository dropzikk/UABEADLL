using System;
using Avalonia.Controls;
using Avalonia.Data;

namespace Avalonia.Diagnostics.Controls;

internal class FilterTextBox : TextBox
{
	public static readonly StyledProperty<bool> UseRegexFilterProperty = AvaloniaProperty.Register<FilterTextBox, bool>("UseRegexFilter", defaultValue: false, inherits: false, BindingMode.TwoWay);

	public static readonly StyledProperty<bool> UseCaseSensitiveFilterProperty = AvaloniaProperty.Register<FilterTextBox, bool>("UseCaseSensitiveFilter", defaultValue: false, inherits: false, BindingMode.TwoWay);

	public static readonly StyledProperty<bool> UseWholeWordFilterProperty = AvaloniaProperty.Register<FilterTextBox, bool>("UseWholeWordFilter", defaultValue: false, inherits: false, BindingMode.TwoWay);

	public bool UseRegexFilter
	{
		get
		{
			return GetValue(UseRegexFilterProperty);
		}
		set
		{
			SetValue(UseRegexFilterProperty, value);
		}
	}

	public bool UseCaseSensitiveFilter
	{
		get
		{
			return GetValue(UseCaseSensitiveFilterProperty);
		}
		set
		{
			SetValue(UseCaseSensitiveFilterProperty, value);
		}
	}

	public bool UseWholeWordFilter
	{
		get
		{
			return GetValue(UseWholeWordFilterProperty);
		}
		set
		{
			SetValue(UseWholeWordFilterProperty, value);
		}
	}

	protected override Type StyleKeyOverride => typeof(TextBox);

	public FilterTextBox()
	{
		base.Classes.Add("filter-text-box");
	}
}
