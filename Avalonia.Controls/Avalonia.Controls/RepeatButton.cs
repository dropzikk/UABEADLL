using System;
using Avalonia.Input;
using Avalonia.Threading;

namespace Avalonia.Controls;

public class RepeatButton : Button
{
	public static readonly StyledProperty<int> IntervalProperty = AvaloniaProperty.Register<RepeatButton, int>("Interval", 100);

	public static readonly StyledProperty<int> DelayProperty = AvaloniaProperty.Register<RepeatButton, int>("Delay", 300);

	private DispatcherTimer? _repeatTimer;

	public int Interval
	{
		get
		{
			return GetValue(IntervalProperty);
		}
		set
		{
			SetValue(IntervalProperty, value);
		}
	}

	public int Delay
	{
		get
		{
			return GetValue(DelayProperty);
		}
		set
		{
			SetValue(DelayProperty, value);
		}
	}

	private void StartTimer()
	{
		if (_repeatTimer == null)
		{
			_repeatTimer = new DispatcherTimer();
			_repeatTimer.Tick += RepeatTimerOnTick;
		}
		if (!_repeatTimer.IsEnabled)
		{
			_repeatTimer.Interval = TimeSpan.FromMilliseconds(Delay);
			_repeatTimer.Start();
		}
	}

	private void RepeatTimerOnTick(object? sender, EventArgs e)
	{
		TimeSpan timeSpan = TimeSpan.FromMilliseconds(Interval);
		if (_repeatTimer.Interval != timeSpan)
		{
			_repeatTimer.Interval = timeSpan;
		}
		OnClick();
	}

	private void StopTimer()
	{
		_repeatTimer?.Stop();
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == Button.IsPressedProperty && !change.GetNewValue<bool>())
		{
			StopTimer();
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (e.Key == Key.Space)
		{
			StartTimer();
		}
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		base.OnKeyUp(e);
		StopTimer();
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);
		if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
		{
			StartTimer();
		}
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		base.OnPointerReleased(e);
		if (e.InitialPressMouseButton == MouseButton.Left)
		{
			StopTimer();
		}
	}
}
