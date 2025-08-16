using System;
using System.ComponentModel;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Metadata;
using Avalonia.Data;
using Avalonia.Interactivity;

namespace Avalonia.Controls.Primitives;

[PseudoClasses(new string[] { ":checked", ":unchecked", ":indeterminate" })]
public class ToggleButton : Button
{
	public static readonly StyledProperty<bool?> IsCheckedProperty;

	public static readonly StyledProperty<bool> IsThreeStateProperty;

	[Obsolete("Use IsCheckedChangedEvent instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static readonly RoutedEvent<RoutedEventArgs> CheckedEvent;

	[Obsolete("Use IsCheckedChangedEvent instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static readonly RoutedEvent<RoutedEventArgs> UncheckedEvent;

	[Obsolete("Use IsCheckedChangedEvent instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static readonly RoutedEvent<RoutedEventArgs> IndeterminateEvent;

	public static readonly RoutedEvent<RoutedEventArgs> IsCheckedChangedEvent;

	public bool? IsChecked
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

	public bool IsThreeState
	{
		get
		{
			return GetValue(IsThreeStateProperty);
		}
		set
		{
			SetValue(IsThreeStateProperty, value);
		}
	}

	[Obsolete("Use IsCheckedChanged instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public event EventHandler<RoutedEventArgs>? Checked
	{
		add
		{
			AddHandler(CheckedEvent, value);
		}
		remove
		{
			RemoveHandler(CheckedEvent, value);
		}
	}

	[Obsolete("Use IsCheckedChanged instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public event EventHandler<RoutedEventArgs>? Unchecked
	{
		add
		{
			AddHandler(UncheckedEvent, value);
		}
		remove
		{
			RemoveHandler(UncheckedEvent, value);
		}
	}

	[Obsolete("Use IsCheckedChanged instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public event EventHandler<RoutedEventArgs>? Indeterminate
	{
		add
		{
			AddHandler(IndeterminateEvent, value);
		}
		remove
		{
			RemoveHandler(IndeterminateEvent, value);
		}
	}

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

	static ToggleButton()
	{
		IsCheckedProperty = AvaloniaProperty.Register<ToggleButton, bool?>("IsChecked", false, inherits: false, BindingMode.TwoWay);
		IsThreeStateProperty = AvaloniaProperty.Register<ToggleButton, bool>("IsThreeState", defaultValue: false);
		CheckedEvent = RoutedEvent.Register<ToggleButton, RoutedEventArgs>("Checked", RoutingStrategies.Bubble);
		UncheckedEvent = RoutedEvent.Register<ToggleButton, RoutedEventArgs>("Unchecked", RoutingStrategies.Bubble);
		IndeterminateEvent = RoutedEvent.Register<ToggleButton, RoutedEventArgs>("Indeterminate", RoutingStrategies.Bubble);
		IsCheckedChangedEvent = RoutedEvent.Register<ToggleButton, RoutedEventArgs>("IsCheckedChanged", RoutingStrategies.Bubble);
	}

	public ToggleButton()
	{
		UpdatePseudoClasses(IsChecked);
	}

	protected override void OnClick()
	{
		Toggle();
		base.OnClick();
	}

	protected virtual void Toggle()
	{
		SetCurrentValue(value: (!IsChecked.HasValue) ? new bool?(false) : ((!IsChecked.Value) ? new bool?(true) : ((!IsThreeState) ? new bool?(false) : ((bool?)null))), property: IsCheckedProperty);
	}

	[Obsolete("Use OnIsCheckedChanged instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected virtual void OnChecked(RoutedEventArgs e)
	{
		RaiseEvent(e);
	}

	[Obsolete("Use OnIsCheckedChanged instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected virtual void OnUnchecked(RoutedEventArgs e)
	{
		RaiseEvent(e);
	}

	[Obsolete("Use OnIsCheckedChanged instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected virtual void OnIndeterminate(RoutedEventArgs e)
	{
		RaiseEvent(e);
	}

	protected virtual void OnIsCheckedChanged(RoutedEventArgs e)
	{
		RaiseEvent(e);
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new ToggleButtonAutomationPeer(this);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (!(change.Property == IsCheckedProperty))
		{
			return;
		}
		bool? newValue = change.GetNewValue<bool?>();
		UpdatePseudoClasses(newValue);
		if (newValue.HasValue)
		{
			if (newValue == true)
			{
				OnChecked(new RoutedEventArgs(CheckedEvent));
			}
			else
			{
				OnUnchecked(new RoutedEventArgs(UncheckedEvent));
			}
		}
		else
		{
			OnIndeterminate(new RoutedEventArgs(IndeterminateEvent));
		}
		OnIsCheckedChanged(new RoutedEventArgs(IsCheckedChangedEvent));
	}

	private void UpdatePseudoClasses(bool? isChecked)
	{
		base.PseudoClasses.Set(":checked", isChecked == true);
		base.PseudoClasses.Set(":unchecked", isChecked == false);
		base.PseudoClasses.Set(":indeterminate", !isChecked.HasValue);
	}
}
