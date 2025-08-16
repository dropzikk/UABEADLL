using System;
using System.ComponentModel;
using Avalonia.Reactive;

namespace Avalonia.Data;

public class InstancedBinding
{
	public BindingMode Mode { get; }

	public BindingPriority Priority { get; }

	public IObservable<object?> Source { get; }

	[Obsolete("Use Source property")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public IObservable<object?> Observable => Source;

	internal InstancedBinding(IObservable<object?> source, BindingMode mode, BindingPriority priority)
	{
		Mode = mode;
		Priority = priority;
		Source = source ?? throw new ArgumentNullException("source");
	}

	public static InstancedBinding OneTime(object value, BindingPriority priority = BindingPriority.LocalValue)
	{
		return new InstancedBinding(Avalonia.Reactive.Observable.SingleValue(value), BindingMode.OneTime, priority);
	}

	public static InstancedBinding OneTime(IObservable<object?> observable, BindingPriority priority = BindingPriority.LocalValue)
	{
		if (observable == null)
		{
			throw new ArgumentNullException("observable");
		}
		return new InstancedBinding(observable, BindingMode.OneTime, priority);
	}

	public static InstancedBinding OneWay(IObservable<object?> observable, BindingPriority priority = BindingPriority.LocalValue)
	{
		if (observable == null)
		{
			throw new ArgumentNullException("observable");
		}
		return new InstancedBinding(observable, BindingMode.OneWay, priority);
	}

	public static InstancedBinding OneWayToSource(IObserver<object?> observer, BindingPriority priority = BindingPriority.LocalValue)
	{
		if (observer == null)
		{
			throw new ArgumentNullException("observer");
		}
		return new InstancedBinding((IObservable<object>)observer, BindingMode.OneWayToSource, priority);
	}

	public static InstancedBinding TwoWay(IObservable<object?> observable, IObserver<object?> observer, BindingPriority priority = BindingPriority.LocalValue)
	{
		if (observable == null)
		{
			throw new ArgumentNullException("observable");
		}
		if (observer == null)
		{
			throw new ArgumentNullException("observer");
		}
		return new InstancedBinding(new CombinedSubject<object>(observer, observable), BindingMode.TwoWay, priority);
	}

	public InstancedBinding WithPriority(BindingPriority priority)
	{
		return new InstancedBinding(Source, Mode, priority);
	}
}
