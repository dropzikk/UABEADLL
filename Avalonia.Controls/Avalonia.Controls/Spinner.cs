using System;
using Avalonia.Interactivity;
using Avalonia.Reactive;

namespace Avalonia.Controls;

public abstract class Spinner : ContentControl
{
	public static readonly StyledProperty<ValidSpinDirections> ValidSpinDirectionProperty;

	public static readonly RoutedEvent<SpinEventArgs> SpinEvent;

	public ValidSpinDirections ValidSpinDirection
	{
		get
		{
			return GetValue(ValidSpinDirectionProperty);
		}
		set
		{
			SetValue(ValidSpinDirectionProperty, value);
		}
	}

	public event EventHandler<SpinEventArgs>? Spin
	{
		add
		{
			AddHandler(SpinEvent, value);
		}
		remove
		{
			RemoveHandler(SpinEvent, value);
		}
	}

	static Spinner()
	{
		ValidSpinDirectionProperty = AvaloniaProperty.Register<Spinner, ValidSpinDirections>("ValidSpinDirection", ValidSpinDirections.Increase | ValidSpinDirections.Decrease);
		SpinEvent = RoutedEvent.Register<Spinner, SpinEventArgs>("Spin", RoutingStrategies.Bubble);
		ValidSpinDirectionProperty.Changed.Subscribe(OnValidSpinDirectionPropertyChanged);
	}

	protected virtual void OnValidSpinDirectionChanged(ValidSpinDirections oldValue, ValidSpinDirections newValue)
	{
	}

	protected virtual void OnSpin(SpinEventArgs e)
	{
		ValidSpinDirections validSpinDirections = ((e.Direction == SpinDirection.Increase) ? ValidSpinDirections.Increase : ValidSpinDirections.Decrease);
		if ((ValidSpinDirection & validSpinDirections) == validSpinDirections)
		{
			RaiseEvent(e);
		}
	}

	private static void OnValidSpinDirectionPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Sender is Spinner spinner)
		{
			ValidSpinDirections oldValue = (ValidSpinDirections)e.OldValue;
			ValidSpinDirections newValue = (ValidSpinDirections)e.NewValue;
			spinner.OnValidSpinDirectionChanged(oldValue, newValue);
		}
	}
}
