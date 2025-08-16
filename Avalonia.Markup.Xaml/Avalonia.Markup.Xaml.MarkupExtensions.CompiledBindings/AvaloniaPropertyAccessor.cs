using System;
using Avalonia.Data;
using Avalonia.Data.Core.Plugins;
using Avalonia.Reactive;

namespace Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

internal class AvaloniaPropertyAccessor : PropertyAccessorBase
{
	private readonly WeakReference<AvaloniaObject?> _reference;

	private readonly AvaloniaProperty _property;

	private IDisposable? _subscription;

	public AvaloniaObject? Instance
	{
		get
		{
			_reference.TryGetTarget(out AvaloniaObject target);
			return target;
		}
	}

	public override Type PropertyType => _property.PropertyType;

	public override object? Value => Instance?.GetValue(_property);

	public AvaloniaPropertyAccessor(WeakReference<AvaloniaObject?> reference, AvaloniaProperty property)
	{
		_reference = reference ?? throw new ArgumentNullException("reference");
		_property = property ?? throw new ArgumentNullException("property");
	}

	public override bool SetValue(object? value, BindingPriority priority)
	{
		if (!_property.IsReadOnly)
		{
			AvaloniaObject instance = Instance;
			if (instance != null)
			{
				instance.SetValue(_property, value, priority);
				return true;
			}
		}
		return false;
	}

	protected override void SubscribeCore()
	{
		_subscription = Instance?.GetObservable(_property).Subscribe(base.PublishValue);
	}

	protected override void UnsubscribeCore()
	{
		_subscription?.Dispose();
		_subscription = null;
	}
}
