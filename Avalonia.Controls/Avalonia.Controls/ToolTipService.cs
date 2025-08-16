using System;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;

namespace Avalonia.Controls;

internal sealed class ToolTipService
{
	private DispatcherTimer? _timer;

	public static ToolTipService Instance { get; } = new ToolTipService();

	private ToolTipService()
	{
	}

	internal void TipChanged(AvaloniaPropertyChangedEventArgs e)
	{
		Control control = (Control)e.Sender;
		if (e.OldValue != null)
		{
			control.PointerEntered -= ControlPointerEntered;
			control.PointerExited -= ControlPointerExited;
		}
		if (e.NewValue != null)
		{
			control.PointerEntered += ControlPointerEntered;
			control.PointerExited += ControlPointerExited;
		}
		if (!ToolTip.GetIsOpen(control) || e.NewValue == e.OldValue || e.NewValue is ToolTip)
		{
			return;
		}
		if (e.NewValue == null)
		{
			Close(control);
			return;
		}
		ToolTip value = control.GetValue(ToolTip.ToolTipProperty);
		if (value != null)
		{
			value.Content = e.NewValue;
		}
	}

	internal void TipOpenChanged(AvaloniaPropertyChangedEventArgs e)
	{
		Control control = (Control)e.Sender;
		object oldValue = e.OldValue;
		if (oldValue is bool && !(bool)oldValue)
		{
			oldValue = e.NewValue;
			if (oldValue is bool && (bool)oldValue)
			{
				control.DetachedFromVisualTree += ControlDetaching;
				control.EffectiveViewportChanged += ControlEffectiveViewportChanged;
				return;
			}
		}
		oldValue = e.OldValue;
		if (oldValue is bool && (bool)oldValue)
		{
			oldValue = e.NewValue;
			if (oldValue is bool && !(bool)oldValue)
			{
				control.DetachedFromVisualTree -= ControlDetaching;
				control.EffectiveViewportChanged -= ControlEffectiveViewportChanged;
			}
		}
	}

	private void ControlDetaching(object? sender, VisualTreeAttachmentEventArgs e)
	{
		Control control = (Control)sender;
		control.DetachedFromVisualTree -= ControlDetaching;
		control.EffectiveViewportChanged -= ControlEffectiveViewportChanged;
		Close(control);
	}

	private void ControlPointerEntered(object? sender, PointerEventArgs e)
	{
		StopTimer();
		Control control = (Control)sender;
		int showDelay = ToolTip.GetShowDelay(control);
		if (showDelay == 0)
		{
			Open(control);
		}
		else
		{
			StartShowTimer(showDelay, control);
		}
	}

	private void ControlPointerExited(object? sender, PointerEventArgs e)
	{
		Control control = (Control)sender;
		Close(control);
	}

	private void ControlEffectiveViewportChanged(object? sender, EffectiveViewportChangedEventArgs e)
	{
		Control control = (Control)sender;
		control.GetValue(ToolTip.ToolTipProperty)?.RecalculatePosition(control);
	}

	private void StartShowTimer(int showDelay, Control control)
	{
		_timer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(showDelay)
		};
		_timer.Tick += delegate
		{
			Open(control);
		};
		_timer.Start();
	}

	private void Open(Control control)
	{
		StopTimer();
		if (control.IsAttachedToVisualTree)
		{
			ToolTip.SetIsOpen(control, value: true);
		}
	}

	private void Close(Control control)
	{
		StopTimer();
		ToolTip.SetIsOpen(control, value: false);
	}

	private void StopTimer()
	{
		_timer?.Stop();
		_timer = null;
	}
}
