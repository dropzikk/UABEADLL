using System;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Collections;
using Avalonia.Controls.Automation.Peers;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Utilities;

namespace Avalonia.Controls;

[TemplatePart("PART_DecreaseButton", typeof(Button))]
[TemplatePart("PART_IncreaseButton", typeof(Button))]
[TemplatePart("PART_Track", typeof(Track))]
[PseudoClasses(new string[] { ":vertical", ":horizontal", ":pressed" })]
public class Slider : RangeBase
{
	public static readonly StyledProperty<Orientation> OrientationProperty;

	public static readonly StyledProperty<bool> IsDirectionReversedProperty;

	public static readonly StyledProperty<bool> IsSnapToTickEnabledProperty;

	public static readonly StyledProperty<double> TickFrequencyProperty;

	public static readonly StyledProperty<TickPlacement> TickPlacementProperty;

	public static readonly StyledProperty<AvaloniaList<double>?> TicksProperty;

	private bool _isDragging;

	private Track? _track;

	private Button? _decreaseButton;

	private Button? _increaseButton;

	private IDisposable? _decreaseButtonPressDispose;

	private IDisposable? _decreaseButtonReleaseDispose;

	private IDisposable? _increaseButtonSubscription;

	private IDisposable? _increaseButtonReleaseDispose;

	private IDisposable? _pointerMovedDispose;

	private const double Tolerance = 0.0001;

	public AvaloniaList<double>? Ticks
	{
		get
		{
			return GetValue(TicksProperty);
		}
		set
		{
			SetValue(TicksProperty, value);
		}
	}

	public Orientation Orientation
	{
		get
		{
			return GetValue(OrientationProperty);
		}
		set
		{
			SetValue(OrientationProperty, value);
		}
	}

	public bool IsDirectionReversed
	{
		get
		{
			return GetValue(IsDirectionReversedProperty);
		}
		set
		{
			SetValue(IsDirectionReversedProperty, value);
		}
	}

	public bool IsSnapToTickEnabled
	{
		get
		{
			return GetValue(IsSnapToTickEnabledProperty);
		}
		set
		{
			SetValue(IsSnapToTickEnabledProperty, value);
		}
	}

	public double TickFrequency
	{
		get
		{
			return GetValue(TickFrequencyProperty);
		}
		set
		{
			SetValue(TickFrequencyProperty, value);
		}
	}

	public TickPlacement TickPlacement
	{
		get
		{
			return GetValue(TickPlacementProperty);
		}
		set
		{
			SetValue(TickPlacementProperty, value);
		}
	}

	protected bool IsDragging => _isDragging;

	protected Track? Track => _track;

	static Slider()
	{
		OrientationProperty = ScrollBar.OrientationProperty.AddOwner<Slider>();
		IsDirectionReversedProperty = Avalonia.Controls.Primitives.Track.IsDirectionReversedProperty.AddOwner<Slider>();
		IsSnapToTickEnabledProperty = AvaloniaProperty.Register<Slider, bool>("IsSnapToTickEnabled", defaultValue: false);
		TickFrequencyProperty = AvaloniaProperty.Register<Slider, double>("TickFrequency", 0.0);
		TickPlacementProperty = AvaloniaProperty.Register<Slider, TickPlacement>("TickPlacement", TickPlacement.None);
		TicksProperty = TickBar.TicksProperty.AddOwner<Slider>();
		PressedMixin.Attach<Slider>();
		InputElement.FocusableProperty.OverrideDefaultValue<Slider>(defaultValue: true);
		OrientationProperty.OverrideDefaultValue(typeof(Slider), Orientation.Horizontal);
		Thumb.DragStartedEvent.AddClassHandler(delegate(Slider x, VectorEventArgs e)
		{
			x.OnThumbDragStarted(e);
		}, RoutingStrategies.Bubble);
		Thumb.DragCompletedEvent.AddClassHandler(delegate(Slider x, VectorEventArgs e)
		{
			x.OnThumbDragCompleted(e);
		}, RoutingStrategies.Bubble);
		RangeBase.ValueProperty.OverrideMetadata<Slider>(new StyledPropertyMetadata<double>(default(Optional<double>), BindingMode.Default, null, enableDataValidation: true));
		AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<Slider>(AutomationControlType.Slider);
	}

	public Slider()
	{
		UpdatePseudoClasses(Orientation);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_decreaseButtonPressDispose?.Dispose();
		_decreaseButtonReleaseDispose?.Dispose();
		_increaseButtonSubscription?.Dispose();
		_increaseButtonReleaseDispose?.Dispose();
		_pointerMovedDispose?.Dispose();
		_decreaseButton = e.NameScope.Find<Button>("PART_DecreaseButton");
		_track = e.NameScope.Find<Track>("PART_Track");
		_increaseButton = e.NameScope.Find<Button>("PART_IncreaseButton");
		if (_track != null)
		{
			_track.IgnoreThumbDrag = true;
		}
		if (_decreaseButton != null)
		{
			_decreaseButtonPressDispose = _decreaseButton.AddDisposableHandler(InputElement.PointerPressedEvent, TrackPressed, RoutingStrategies.Tunnel);
			_decreaseButtonReleaseDispose = _decreaseButton.AddDisposableHandler(InputElement.PointerReleasedEvent, TrackReleased, RoutingStrategies.Tunnel);
		}
		if (_increaseButton != null)
		{
			_increaseButtonSubscription = _increaseButton.AddDisposableHandler(InputElement.PointerPressedEvent, TrackPressed, RoutingStrategies.Tunnel);
			_increaseButtonReleaseDispose = _increaseButton.AddDisposableHandler(InputElement.PointerReleasedEvent, TrackReleased, RoutingStrategies.Tunnel);
		}
		_pointerMovedDispose = this.AddDisposableHandler(InputElement.PointerMovedEvent, TrackMoved, RoutingStrategies.Tunnel);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (!e.Handled && e.KeyModifiers == KeyModifiers.None)
		{
			bool handled = true;
			switch (e.Key)
			{
			case Key.Left:
			case Key.Down:
				MoveToNextTick(IsDirectionReversed ? base.SmallChange : (0.0 - base.SmallChange));
				break;
			case Key.Up:
			case Key.Right:
				MoveToNextTick(IsDirectionReversed ? (0.0 - base.SmallChange) : base.SmallChange);
				break;
			case Key.PageUp:
				MoveToNextTick(IsDirectionReversed ? (0.0 - base.LargeChange) : base.LargeChange);
				break;
			case Key.PageDown:
				MoveToNextTick(IsDirectionReversed ? base.LargeChange : (0.0 - base.LargeChange));
				break;
			case Key.Home:
				SetCurrentValue(RangeBase.ValueProperty, base.Minimum);
				break;
			case Key.End:
				SetCurrentValue(RangeBase.ValueProperty, base.Maximum);
				break;
			default:
				handled = false;
				break;
			}
			e.Handled = handled;
		}
	}

	private void MoveToNextTick(double direction)
	{
		if (direction == 0.0)
		{
			return;
		}
		double value = base.Value;
		double num = SnapToTick(Math.Max(base.Minimum, Math.Min(base.Maximum, value + direction)));
		bool flag = direction > 0.0;
		if (Math.Abs(num - value) < 0.0001 && (!flag || !(Math.Abs(value - base.Maximum) < 0.0001)) && (flag || !(Math.Abs(value - base.Minimum) < 0.0001)))
		{
			AvaloniaList<double> ticks = Ticks;
			if (ticks != null && ticks.Count > 0)
			{
				foreach (double item in ticks)
				{
					if ((flag && MathUtilities.GreaterThan(item, value) && (MathUtilities.LessThan(item, num) || Math.Abs(num - value) < 0.0001)) || (!flag && MathUtilities.LessThan(item, value) && (MathUtilities.GreaterThan(item, num) || Math.Abs(num - value) < 0.0001)))
					{
						num = item;
					}
				}
			}
			else if (MathUtilities.GreaterThan(TickFrequency, 0.0))
			{
				double num2 = Math.Round((value - base.Minimum) / TickFrequency);
				num2 = ((!flag) ? (num2 - 1.0) : (num2 + 1.0));
				num = base.Minimum + num2 * TickFrequency;
			}
		}
		if (Math.Abs(num - value) > 0.0001)
		{
			SetCurrentValue(RangeBase.ValueProperty, num);
		}
	}

	private void TrackMoved(object? sender, PointerEventArgs e)
	{
		if (!base.IsEnabled)
		{
			_isDragging = false;
		}
		else if (_isDragging)
		{
			MoveToPoint(e.GetCurrentPoint(_track));
		}
	}

	private void TrackReleased(object? sender, PointerReleasedEventArgs e)
	{
		_isDragging = false;
	}

	private void TrackPressed(object? sender, PointerPressedEventArgs e)
	{
		if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
		{
			MoveToPoint(e.GetCurrentPoint(_track));
			_isDragging = true;
		}
	}

	private void MoveToPoint(PointerPoint posOnTrack)
	{
		if (_track != null)
		{
			bool num = Orientation == Orientation.Horizontal;
			double num2 = ((!num) ? (_track.Thumb?.Bounds.Height ?? 0.0) : (_track.Thumb?.Bounds.Width ?? 0.0)) + double.Epsilon;
			double num3 = (num ? _track.Bounds.Width : _track.Bounds.Height) - num2;
			double num4 = MathUtilities.Clamp(((num ? posOnTrack.Position.X : posOnTrack.Position.Y) - num2 * 0.5) / num3, 0.0, 1.0);
			double num5 = Math.Abs((double)((!num) ? ((!IsDirectionReversed) ? 1 : 0) : (IsDirectionReversed ? 1 : 0)) - num4);
			double num6 = base.Maximum - base.Minimum;
			double num7 = num5 * num6 + base.Minimum;
			SetCurrentValue(RangeBase.ValueProperty, IsSnapToTickEnabled ? SnapToTick(num7) : num7);
		}
	}

	protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
	{
		if (property == RangeBase.ValueProperty)
		{
			DataValidationErrors.SetError(this, error);
		}
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new SliderAutomationPeer(this);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == OrientationProperty)
		{
			UpdatePseudoClasses(change.GetNewValue<Orientation>());
		}
	}

	protected virtual void OnThumbDragStarted(VectorEventArgs e)
	{
		_isDragging = true;
	}

	protected virtual void OnThumbDragCompleted(VectorEventArgs e)
	{
		_isDragging = false;
	}

	private double SnapToTick(double value)
	{
		if (IsSnapToTickEnabled)
		{
			double num = base.Minimum;
			double num2 = base.Maximum;
			AvaloniaList<double> ticks = Ticks;
			if (ticks != null && ticks.Count > 0)
			{
				foreach (double item in ticks)
				{
					if (MathUtilities.AreClose(item, value))
					{
						return value;
					}
					if (MathUtilities.LessThan(item, value) && MathUtilities.GreaterThan(item, num))
					{
						num = item;
					}
					else if (MathUtilities.GreaterThan(item, value) && MathUtilities.LessThan(item, num2))
					{
						num2 = item;
					}
				}
			}
			else if (MathUtilities.GreaterThan(TickFrequency, 0.0))
			{
				num = base.Minimum + Math.Round((value - base.Minimum) / TickFrequency) * TickFrequency;
				num2 = Math.Min(base.Maximum, num + TickFrequency);
			}
			value = (MathUtilities.GreaterThanOrClose(value, (num + num2) * 0.5) ? num2 : num);
		}
		return value;
	}

	private void UpdatePseudoClasses(Orientation o)
	{
		base.PseudoClasses.Set(":vertical", o == Orientation.Vertical);
		base.PseudoClasses.Set(":horizontal", o == Orientation.Horizontal);
	}
}
