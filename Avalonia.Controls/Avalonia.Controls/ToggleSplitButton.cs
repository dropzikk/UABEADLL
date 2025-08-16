using System;
using Avalonia.Controls.Metadata;
using Avalonia.Interactivity;

namespace Avalonia.Controls;

[PseudoClasses(new string[] { ":checked" })]
public class ToggleSplitButton : SplitButton
{
	public static readonly RoutedEvent<RoutedEventArgs> IsCheckedChangedEvent = RoutedEvent.Register<ToggleSplitButton, RoutedEventArgs>("IsCheckedChanged", RoutingStrategies.Bubble);

	public static readonly StyledProperty<bool> IsCheckedProperty = AvaloniaProperty.Register<ToggleSplitButton, bool>("IsChecked", defaultValue: false);

	public bool IsChecked
	{
		get
		{
			return GetValue(IsCheckedProperty);
		}
		set
		{
			SetValue(IsCheckedProperty, value);
		}
	}

	internal override bool InternalIsChecked => IsChecked;

	protected override Type StyleKeyOverride => typeof(SplitButton);

	public event EventHandler<RoutedEventArgs>? IsCheckedChanged
	{
		add
		{
			AddHandler(IsCheckedChangedEvent, value);
		}
		remove
		{
			RemoveHandler(IsCheckedChangedEvent, value);
		}
	}

	protected void Toggle()
	{
		SetCurrentValue(IsCheckedProperty, !IsChecked);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		base.OnPropertyChanged(e);
		if (e.Property == IsCheckedProperty)
		{
			OnIsCheckedChanged();
		}
	}

	protected virtual void OnIsCheckedChanged()
	{
		if (base.Parent != null)
		{
			RoutedEventArgs e = new RoutedEventArgs(IsCheckedChangedEvent);
			RaiseEvent(e);
		}
		UpdatePseudoClasses();
	}

	protected override void OnClickPrimary(RoutedEventArgs? e)
	{
		Toggle();
		base.OnClickPrimary(e);
	}
}
