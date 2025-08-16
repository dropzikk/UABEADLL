using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace AvaloniaEdit.Rendering;

public class PointerHoverLogic : IDisposable
{
	private const double PointerHoverWidth = 2.0;

	private const double PointerHoverHeight = 2.0;

	private static readonly TimeSpan PointerHoverTime = TimeSpan.FromMilliseconds(400.0);

	private readonly Control _target;

	private DispatcherTimer _timer;

	private Point _hoverStartPoint;

	private PointerEventArgs _hoverLastEventArgs;

	private bool _hovering;

	private bool _disposed;

	public event EventHandler<PointerEventArgs> PointerHover;

	public event EventHandler<PointerEventArgs> PointerHoverStopped;

	public PointerHoverLogic(Control target)
	{
		_target = target ?? throw new ArgumentNullException("target");
		_target.PointerExited += OnPointerLeave;
		_target.PointerMoved += OnPointerMoved;
		_target.PointerEntered += OnPointerEnter;
	}

	private void OnPointerMoved(object sender, PointerEventArgs e)
	{
		Point point = _hoverStartPoint - e.GetPosition(_target);
		if (Math.Abs(point.X) > 2.0 || Math.Abs(point.Y) > 2.0)
		{
			StartHovering(e);
		}
	}

	private void OnPointerEnter(object sender, PointerEventArgs e)
	{
		StartHovering(e);
	}

	private void StartHovering(PointerEventArgs e)
	{
		StopHovering();
		_hoverStartPoint = e.GetPosition(_target);
		_hoverLastEventArgs = e;
		_timer = new DispatcherTimer(PointerHoverTime, DispatcherPriority.Background, OnTimerElapsed);
		_timer.Start();
	}

	private void OnPointerLeave(object sender, PointerEventArgs e)
	{
		StopHovering();
	}

	private void StopHovering()
	{
		if (_timer != null)
		{
			_timer.Stop();
			_timer = null;
		}
		if (_hovering)
		{
			_hovering = false;
			OnPointerHoverStopped(_hoverLastEventArgs);
		}
	}

	private void OnTimerElapsed(object sender, EventArgs e)
	{
		_timer.Stop();
		_timer = null;
		_hovering = true;
		OnPointerHover(_hoverLastEventArgs);
	}

	protected virtual void OnPointerHover(PointerEventArgs e)
	{
		this.PointerHover?.Invoke(this, e);
	}

	protected virtual void OnPointerHoverStopped(PointerEventArgs e)
	{
		this.PointerHoverStopped?.Invoke(this, e);
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_target.PointerExited -= OnPointerLeave;
			_target.PointerMoved -= OnPointerMoved;
			_target.PointerEntered -= OnPointerEnter;
		}
		_disposed = true;
	}
}
