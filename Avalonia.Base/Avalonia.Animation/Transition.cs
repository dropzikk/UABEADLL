using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Animation.Easings;
using Avalonia.Data;

namespace Avalonia.Animation;

public abstract class Transition<T> : AvaloniaObject, ITransition
{
	private AvaloniaProperty? _prop;

	public TimeSpan Duration { get; set; }

	public TimeSpan Delay { get; set; } = TimeSpan.Zero;

	public Easing Easing { get; set; } = new LinearEasing();

	public AvaloniaProperty? Property
	{
		get
		{
			return _prop;
		}
		[param: DisallowNull]
		set
		{
			if (!value.PropertyType.IsAssignableFrom(typeof(T)))
			{
				throw new InvalidCastException($"Invalid property type \"{typeof(T).Name}\" for this transition: {GetType().Name}.");
			}
			_prop = value;
		}
	}

	AvaloniaProperty ITransition.Property
	{
		get
		{
			return Property ?? throw new InvalidOperationException("Transition has no property specified.");
		}
		set
		{
			Property = value;
		}
	}

	internal abstract IObservable<T> DoTransition(IObservable<double> progress, T oldValue, T newValue);

	IDisposable ITransition.Apply(Animatable control, IClock clock, object? oldValue, object? newValue)
	{
		return Apply(control, clock, oldValue, newValue);
	}

	internal virtual IDisposable Apply(Animatable control, IClock clock, object? oldValue, object? newValue)
	{
		if ((object)Property == null)
		{
			throw new InvalidOperationException("Transition has no property specified.");
		}
		IObservable<T> source = DoTransition(new TransitionInstance(clock, Delay, Duration), (T)oldValue, (T)newValue);
		return control.Bind((AvaloniaProperty<T>)Property, source, BindingPriority.Animation);
	}
}
