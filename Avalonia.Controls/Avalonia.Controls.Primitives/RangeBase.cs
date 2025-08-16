using System;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Utilities;

namespace Avalonia.Controls.Primitives;

public abstract class RangeBase : TemplatedControl
{
	public static readonly StyledProperty<double> MinimumProperty = AvaloniaProperty.Register<RangeBase, double>("Minimum", 0.0, inherits: false, BindingMode.OneWay, null, CoerceMinimum);

	public static readonly StyledProperty<double> MaximumProperty = AvaloniaProperty.Register<RangeBase, double>("Maximum", 100.0, inherits: false, BindingMode.OneWay, null, CoerceMaximum);

	public static readonly StyledProperty<double> ValueProperty = AvaloniaProperty.Register<RangeBase, double>("Value", 0.0, inherits: false, BindingMode.TwoWay, null, CoerceValue);

	public static readonly StyledProperty<double> SmallChangeProperty = AvaloniaProperty.Register<RangeBase, double>("SmallChange", 1.0);

	public static readonly StyledProperty<double> LargeChangeProperty = AvaloniaProperty.Register<RangeBase, double>("LargeChange", 10.0);

	public static readonly RoutedEvent<RangeBaseValueChangedEventArgs> ValueChangedEvent = RoutedEvent.Register<RangeBase, RangeBaseValueChangedEventArgs>("ValueChanged", RoutingStrategies.Bubble);

	public double Minimum
	{
		get
		{
			return GetValue(MinimumProperty);
		}
		set
		{
			SetValue(MinimumProperty, value);
		}
	}

	public double Maximum
	{
		get
		{
			return GetValue(MaximumProperty);
		}
		set
		{
			SetValue(MaximumProperty, value);
		}
	}

	public double Value
	{
		get
		{
			return GetValue(ValueProperty);
		}
		set
		{
			SetValue(ValueProperty, value);
		}
	}

	public double SmallChange
	{
		get
		{
			return GetValue(SmallChangeProperty);
		}
		set
		{
			SetValue(SmallChangeProperty, value);
		}
	}

	public double LargeChange
	{
		get
		{
			return GetValue(LargeChangeProperty);
		}
		set
		{
			SetValue(LargeChangeProperty, value);
		}
	}

	public event EventHandler<RangeBaseValueChangedEventArgs>? ValueChanged
	{
		add
		{
			AddHandler(ValueChangedEvent, value);
		}
		remove
		{
			RemoveHandler(ValueChangedEvent, value);
		}
	}

	private static double CoerceMinimum(AvaloniaObject sender, double value)
	{
		if (!ValidateDouble(value))
		{
			return sender.GetValue(MinimumProperty);
		}
		return value;
	}

	private void OnMinimumChanged()
	{
		if (base.IsInitialized)
		{
			CoerceValue(MaximumProperty);
			CoerceValue(ValueProperty);
		}
	}

	private static double CoerceMaximum(AvaloniaObject sender, double value)
	{
		if (!ValidateDouble(value))
		{
			return sender.GetValue(MaximumProperty);
		}
		return Math.Max(value, sender.GetValue(MinimumProperty));
	}

	private void OnMaximumChanged()
	{
		if (base.IsInitialized)
		{
			CoerceValue(ValueProperty);
		}
	}

	private static double CoerceValue(AvaloniaObject sender, double value)
	{
		if (!ValidateDouble(value))
		{
			return sender.GetValue(ValueProperty);
		}
		return MathUtilities.Clamp(value, sender.GetValue(MinimumProperty), sender.GetValue(MaximumProperty));
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		CoerceValue(MaximumProperty);
		CoerceValue(ValueProperty);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == MinimumProperty)
		{
			OnMinimumChanged();
		}
		else if (change.Property == MaximumProperty)
		{
			OnMaximumChanged();
		}
		else if (change.Property == ValueProperty)
		{
			RangeBaseValueChangedEventArgs e = new RangeBaseValueChangedEventArgs(change.GetOldValue<double>(), change.GetNewValue<double>(), ValueChangedEvent);
			RaiseEvent(e);
		}
	}

	private static bool ValidateDouble(double value)
	{
		if (!double.IsInfinity(value))
		{
			return !double.IsNaN(value);
		}
		return false;
	}
}
