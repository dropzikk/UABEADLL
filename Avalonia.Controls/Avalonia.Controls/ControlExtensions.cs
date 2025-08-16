using System;
using Avalonia.Reactive;

namespace Avalonia.Controls;

public static class ControlExtensions
{
	public static void BringIntoView(this Control control)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		control.BringIntoView(new Rect(control.Bounds.Size));
	}

	public static void BringIntoView(this Control control, Rect rect)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		if (control.IsEffectivelyVisible)
		{
			RequestBringIntoViewEventArgs e = new RequestBringIntoViewEventArgs
			{
				RoutedEvent = Control.RequestBringIntoViewEvent,
				TargetObject = control,
				TargetRect = rect
			};
			control.RaiseEvent(e);
		}
	}

	public static T? FindControl<T>(this Control control, string name) where T : Control
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		return (control.FindNameScope() ?? throw new InvalidOperationException("Could not find parent name scope.")).Find<T>(name);
	}

	public static T GetControl<T>(this Control control, string name) where T : Control
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		return (control.FindNameScope() ?? throw new InvalidOperationException("Could not find parent name scope.")).Find<T>(name) ?? throw new ArgumentException("Could not find control named '" + name + "'.");
	}

	public static IDisposable Set(this IPseudoClasses classes, string name, IObservable<bool> trigger)
	{
		if (classes == null)
		{
			throw new ArgumentNullException("classes");
		}
		return trigger.Subscribe(delegate(bool x)
		{
			classes.Set(name, x);
		});
	}
}
