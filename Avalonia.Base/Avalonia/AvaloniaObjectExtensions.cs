using System;
using Avalonia.Data;
using Avalonia.Reactive;

namespace Avalonia;

public static class AvaloniaObjectExtensions
{
	private class BindingAdaptor : IBinding
	{
		private IObservable<object?> _source;

		public BindingAdaptor(IObservable<object?> source)
		{
			_source = source;
		}

		public InstancedBinding? Initiate(AvaloniaObject target, AvaloniaProperty? targetProperty, object? anchor = null, bool enableDataValidation = false)
		{
			return InstancedBinding.OneWay(_source);
		}
	}

	private class ClassHandlerObserver<TTarget, TValue> : IObserver<AvaloniaPropertyChangedEventArgs<TValue>>
	{
		private readonly Action<TTarget, AvaloniaPropertyChangedEventArgs<TValue>> _action;

		public ClassHandlerObserver(Action<TTarget, AvaloniaPropertyChangedEventArgs<TValue>> action)
		{
			_action = action;
		}

		public void OnCompleted()
		{
		}

		public void OnError(Exception error)
		{
		}

		public void OnNext(AvaloniaPropertyChangedEventArgs<TValue> value)
		{
			if (value.Sender is TTarget arg)
			{
				_action(arg, value);
			}
		}
	}

	private class ClassHandlerObserver<TTarget> : IObserver<AvaloniaPropertyChangedEventArgs>
	{
		private readonly Action<TTarget, AvaloniaPropertyChangedEventArgs> _action;

		public ClassHandlerObserver(Action<TTarget, AvaloniaPropertyChangedEventArgs> action)
		{
			_action = action;
		}

		public void OnCompleted()
		{
		}

		public void OnError(Exception error)
		{
		}

		public void OnNext(AvaloniaPropertyChangedEventArgs value)
		{
			if (value.Sender is TTarget arg)
			{
				_action(arg, value);
			}
		}
	}

	public static IBinding ToBinding<T>(this IObservable<T> source)
	{
		return new BindingAdaptor(source.Select((Func<T, object>)((T x) => x)));
	}

	public static IObservable<object?> GetObservable(this AvaloniaObject o, AvaloniaProperty property)
	{
		return new AvaloniaPropertyObservable<object, object>(o ?? throw new ArgumentNullException("o"), property ?? throw new ArgumentNullException("property"));
	}

	public static IObservable<T> GetObservable<T>(this AvaloniaObject o, AvaloniaProperty<T> property)
	{
		return new AvaloniaPropertyObservable<T, T>(o ?? throw new ArgumentNullException("o"), property ?? throw new ArgumentNullException("property"));
	}

	public static IObservable<TResult> GetObservable<TSource, TResult>(this AvaloniaObject o, AvaloniaProperty<TSource> property, Func<TSource, TResult> converter)
	{
		return new AvaloniaPropertyObservable<TSource, TResult>(o ?? throw new ArgumentNullException("o"), property ?? throw new ArgumentNullException("property"), converter ?? throw new ArgumentNullException("converter"));
	}

	public static IObservable<TResult> GetObservable<TResult>(this AvaloniaObject o, AvaloniaProperty property, Func<object?, TResult> converter)
	{
		return new AvaloniaPropertyObservable<object, TResult>(o ?? throw new ArgumentNullException("o"), property ?? throw new ArgumentNullException("property"), converter ?? throw new ArgumentNullException("converter"));
	}

	public static IObservable<BindingValue<object?>> GetBindingObservable(this AvaloniaObject o, AvaloniaProperty property)
	{
		return new AvaloniaPropertyBindingObservable<object, object>(o ?? throw new ArgumentNullException("o"), property ?? throw new ArgumentNullException("property"));
	}

	public static IObservable<BindingValue<TResult>> GetBindingObservable<TResult>(this AvaloniaObject o, AvaloniaProperty property, Func<object?, TResult> converter)
	{
		return new AvaloniaPropertyBindingObservable<object, TResult>(o ?? throw new ArgumentNullException("o"), property ?? throw new ArgumentNullException("property"), converter ?? throw new ArgumentNullException("converter"));
	}

	public static IObservable<BindingValue<T>> GetBindingObservable<T>(this AvaloniaObject o, AvaloniaProperty<T> property)
	{
		return new AvaloniaPropertyBindingObservable<T, T>(o ?? throw new ArgumentNullException("o"), property ?? throw new ArgumentNullException("property"));
	}

	public static IObservable<BindingValue<TResult>> GetBindingObservable<TSource, TResult>(this AvaloniaObject o, AvaloniaProperty<TSource> property, Func<TSource, TResult> converter)
	{
		return new AvaloniaPropertyBindingObservable<TSource, TResult>(o ?? throw new ArgumentNullException("o"), property ?? throw new ArgumentNullException("property"), converter ?? throw new ArgumentNullException("converter"));
	}

	public static IObservable<AvaloniaPropertyChangedEventArgs> GetPropertyChangedObservable(this AvaloniaObject o, AvaloniaProperty property)
	{
		return new AvaloniaPropertyChangedObservable(o ?? throw new ArgumentNullException("o"), property ?? throw new ArgumentNullException("property"));
	}

	public static IDisposable Bind<T>(this AvaloniaObject target, AvaloniaProperty<T> property, IObservable<BindingValue<T>> source, BindingPriority priority = BindingPriority.LocalValue)
	{
		target = target ?? throw new ArgumentNullException("target");
		property = property ?? throw new ArgumentNullException("property");
		source = source ?? throw new ArgumentNullException("source");
		if (!(property is StyledProperty<T> property2))
		{
			if (property is DirectPropertyBase<T> property3)
			{
				return target.Bind(property3, source);
			}
			throw new NotSupportedException("Unsupported AvaloniaProperty type.");
		}
		return target.Bind(property2, source, priority);
	}

	public static IDisposable Bind<T>(this AvaloniaObject target, AvaloniaProperty<T> property, IObservable<T> source, BindingPriority priority = BindingPriority.LocalValue)
	{
		if (!(property is StyledProperty<T> property2))
		{
			if (property is DirectPropertyBase<T> property3)
			{
				return target.Bind(property3, source);
			}
			throw new NotSupportedException("Unsupported AvaloniaProperty type.");
		}
		return target.Bind(property2, source, priority);
	}

	public static IDisposable Bind(this AvaloniaObject target, AvaloniaProperty property, IBinding binding, object? anchor = null)
	{
		target = target ?? throw new ArgumentNullException("target");
		property = property ?? throw new ArgumentNullException("property");
		binding = binding ?? throw new ArgumentNullException("binding");
		InstancedBinding instancedBinding = binding.Initiate(target, property, anchor, property.GetMetadata(target.GetType()).EnableDataValidation == true);
		if (instancedBinding != null)
		{
			return BindingOperations.Apply(target, property, instancedBinding, anchor);
		}
		return Disposable.Empty;
	}

	public static T GetValue<T>(this AvaloniaObject target, AvaloniaProperty<T> property)
	{
		target = target ?? throw new ArgumentNullException("target");
		property = property ?? throw new ArgumentNullException("property");
		if (!(property is StyledProperty<T> property2))
		{
			if (property is DirectPropertyBase<T> property3)
			{
				return target.GetValue(property3);
			}
			throw new NotSupportedException("Unsupported AvaloniaProperty type.");
		}
		return target.GetValue(property2);
	}

	public static object? GetBaseValue(this AvaloniaObject target, AvaloniaProperty property)
	{
		target = target ?? throw new ArgumentNullException("target");
		property = property ?? throw new ArgumentNullException("property");
		return property.RouteGetBaseValue(target);
	}

	public static Optional<T> GetBaseValue<T>(this AvaloniaObject target, AvaloniaProperty<T> property)
	{
		target = target ?? throw new ArgumentNullException("target");
		property = property ?? throw new ArgumentNullException("property");
		if (!(property is StyledProperty<T> property2))
		{
			if (property is DirectPropertyBase<T> property3)
			{
				return target.GetValue(property3);
			}
			throw new NotSupportedException("Unsupported AvaloniaProperty type.");
		}
		return target.GetBaseValue(property2);
	}

	public static IDisposable AddClassHandler<TTarget>(this IObservable<AvaloniaPropertyChangedEventArgs> observable, Action<TTarget, AvaloniaPropertyChangedEventArgs> action) where TTarget : AvaloniaObject
	{
		return observable.Subscribe(new ClassHandlerObserver<TTarget>(action));
	}

	public static IDisposable AddClassHandler<TTarget, TValue>(this IObservable<AvaloniaPropertyChangedEventArgs<TValue>> observable, Action<TTarget, AvaloniaPropertyChangedEventArgs<TValue>> action) where TTarget : AvaloniaObject
	{
		return observable.Subscribe(new ClassHandlerObserver<TTarget, TValue>(action));
	}
}
