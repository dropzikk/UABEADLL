using System;
using Avalonia.Reactive;

namespace Avalonia.Data;

public static class BindingOperations
{
	private sealed class TwoWayBindingDisposable : IDisposable
	{
		private readonly IDisposable _toTargetSubscription;

		private readonly IDisposable _fromTargetSubsription;

		private bool _isDisposed;

		public TwoWayBindingDisposable(IDisposable toTargetSubscription, IDisposable fromTargetSubsription)
		{
			_toTargetSubscription = toTargetSubscription;
			_fromTargetSubsription = fromTargetSubsription;
		}

		public void Dispose()
		{
			if (!_isDisposed)
			{
				_fromTargetSubsription.Dispose();
				_toTargetSubscription.Dispose();
				_isDisposed = true;
			}
		}
	}

	public static readonly object DoNothing = new DoNothingType();

	public static IDisposable Apply(AvaloniaObject target, AvaloniaProperty property, InstancedBinding binding, object? anchor)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if ((object)property == null)
		{
			throw new ArgumentNullException("property");
		}
		if (binding == null)
		{
			throw new ArgumentNullException("binding");
		}
		BindingMode bindingMode = binding.Mode;
		if (bindingMode == BindingMode.Default)
		{
			bindingMode = property.GetMetadata(target.GetType()).DefaultBindingMode;
		}
		switch (bindingMode)
		{
		case BindingMode.Default:
		case BindingMode.OneWay:
			return target.Bind(property, binding.Source, binding.Priority);
		case BindingMode.TwoWay:
			if (!(binding.Source is IObserver<object> observer2))
			{
				throw new InvalidOperationException("InstancedBinding does not contain a subject.");
			}
			return new TwoWayBindingDisposable(target.Bind(property, binding.Source, binding.Priority), target.GetObservable(property).Subscribe(observer2));
		case BindingMode.OneTime:
		{
			AvaloniaObject targetCopy = target;
			AvaloniaProperty propertyCopy = property;
			InstancedBinding bindingCopy = binding;
			return binding.Source.Where((object x) => BindingNotification.ExtractValue(x) != AvaloniaProperty.UnsetValue).Take(1).Subscribe(delegate(object x)
			{
				targetCopy.SetValue(propertyCopy, BindingNotification.ExtractValue(x), bindingCopy.Priority);
			});
		}
		case BindingMode.OneWayToSource:
		{
			IObservable<object> source = binding.Source;
			IObserver<object?> observer = source as IObserver<object>;
			if (observer == null)
			{
				throw new InvalidOperationException("InstancedBinding does not contain a subject.");
			}
			return binding.Source.CombineLatest(target.GetObservable(property), (object _, object v) => v).Subscribe(delegate(object x)
			{
				observer.OnNext(x);
			});
		}
		default:
			throw new ArgumentException("Invalid binding mode.");
		}
	}
}
