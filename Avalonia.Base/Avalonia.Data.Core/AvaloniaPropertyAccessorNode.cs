using System;
using Avalonia.Reactive;

namespace Avalonia.Data.Core;

internal class AvaloniaPropertyAccessorNode : SettableNode
{
	private IDisposable? _subscription;

	private readonly bool _enableValidation;

	private readonly AvaloniaProperty _property;

	public override string? Description => PropertyName;

	public string? PropertyName { get; }

	public override Type PropertyType => _property.PropertyType;

	public AvaloniaPropertyAccessorNode(AvaloniaProperty property, bool enableValidation)
	{
		_property = property;
		_enableValidation = enableValidation;
	}

	protected override bool SetTargetValueCore(object? value, BindingPriority priority)
	{
		try
		{
			if (base.Target.TryGetTarget(out object target) && target is AvaloniaObject avaloniaObject)
			{
				avaloniaObject.SetValue(_property, value, priority);
				return true;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	protected override void StartListeningCore(WeakReference<object?> reference)
	{
		if (reference.TryGetTarget(out object target) && target is AvaloniaObject target2)
		{
			_subscription = new AvaloniaPropertyObservable<object, object>(target2, _property).Subscribe(base.ValueChanged);
		}
		else
		{
			_subscription = null;
		}
	}

	protected override void StopListeningCore()
	{
		_subscription?.Dispose();
		_subscription = null;
	}
}
