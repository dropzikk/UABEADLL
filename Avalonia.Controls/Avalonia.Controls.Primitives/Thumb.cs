using System;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Automation.Peers;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Controls.Primitives;

[PseudoClasses(new string[] { ":pressed" })]
public class Thumb : TemplatedControl
{
	public static readonly RoutedEvent<VectorEventArgs> DragStartedEvent;

	public static readonly RoutedEvent<VectorEventArgs> DragDeltaEvent;

	public static readonly RoutedEvent<VectorEventArgs> DragCompletedEvent;

	private Point? _lastPoint;

	public event EventHandler<VectorEventArgs>? DragStarted
	{
		add
		{
			AddHandler(DragStartedEvent, value);
		}
		remove
		{
			RemoveHandler(DragStartedEvent, value);
		}
	}

	public event EventHandler<VectorEventArgs>? DragDelta
	{
		add
		{
			AddHandler(DragDeltaEvent, value);
		}
		remove
		{
			RemoveHandler(DragDeltaEvent, value);
		}
	}

	public event EventHandler<VectorEventArgs>? DragCompleted
	{
		add
		{
			AddHandler(DragCompletedEvent, value);
		}
		remove
		{
			RemoveHandler(DragCompletedEvent, value);
		}
	}

	static Thumb()
	{
		DragStartedEvent = RoutedEvent.Register<Thumb, VectorEventArgs>("DragStarted", RoutingStrategies.Bubble);
		DragDeltaEvent = RoutedEvent.Register<Thumb, VectorEventArgs>("DragDelta", RoutingStrategies.Bubble);
		DragCompletedEvent = RoutedEvent.Register<Thumb, VectorEventArgs>("DragCompleted", RoutingStrategies.Bubble);
		DragStartedEvent.AddClassHandler(delegate(Thumb x, VectorEventArgs e)
		{
			x.OnDragStarted(e);
		}, RoutingStrategies.Bubble);
		DragDeltaEvent.AddClassHandler(delegate(Thumb x, VectorEventArgs e)
		{
			x.OnDragDelta(e);
		}, RoutingStrategies.Bubble);
		DragCompletedEvent.AddClassHandler(delegate(Thumb x, VectorEventArgs e)
		{
			x.OnDragCompleted(e);
		}, RoutingStrategies.Bubble);
	}

	internal void AdjustDrag(Vector v)
	{
		if (_lastPoint.HasValue)
		{
			_lastPoint = _lastPoint.Value + v;
		}
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new ThumbAutomationPeer(this);
	}

	protected virtual void OnDragStarted(VectorEventArgs e)
	{
	}

	protected virtual void OnDragDelta(VectorEventArgs e)
	{
	}

	protected virtual void OnDragCompleted(VectorEventArgs e)
	{
	}

	protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
	{
		if (_lastPoint.HasValue)
		{
			VectorEventArgs e2 = new VectorEventArgs
			{
				RoutedEvent = DragCompletedEvent,
				Vector = _lastPoint.Value
			};
			_lastPoint = null;
			RaiseEvent(e2);
		}
		base.PseudoClasses.Remove(":pressed");
		base.OnPointerCaptureLost(e);
	}

	protected override void OnPointerMoved(PointerEventArgs e)
	{
		if (_lastPoint.HasValue)
		{
			VectorEventArgs e2 = new VectorEventArgs
			{
				RoutedEvent = DragDeltaEvent,
				Vector = e.GetPosition(this) - _lastPoint.Value
			};
			RaiseEvent(e2);
		}
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		e.Handled = true;
		_lastPoint = e.GetPosition(this);
		VectorEventArgs e2 = new VectorEventArgs
		{
			RoutedEvent = DragStartedEvent,
			Vector = _lastPoint.Value
		};
		base.PseudoClasses.Add(":pressed");
		e.PreventGestureRecognition();
		RaiseEvent(e2);
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		if (_lastPoint.HasValue)
		{
			e.Handled = true;
			_lastPoint = null;
			VectorEventArgs e2 = new VectorEventArgs
			{
				RoutedEvent = DragCompletedEvent,
				Vector = e.GetPosition(this)
			};
			RaiseEvent(e2);
		}
		base.PseudoClasses.Remove(":pressed");
	}
}
