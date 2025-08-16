using System;
using System.Runtime.Serialization;

namespace Avalonia.Data.Core;

internal abstract class ExpressionNode
{
	protected static readonly WeakReference<object?> UnsetReference = new WeakReference<object>(AvaloniaProperty.UnsetValue);

	protected static readonly WeakReference<object?> NullReference = new WeakReference<object>(null);

	private WeakReference<object?> _target = UnsetReference;

	private Action<object?>? _subscriber;

	private bool _listening;

	protected WeakReference<object?>? LastValue { get; private set; }

	public abstract string? Description { get; }

	public ExpressionNode? Next { get; set; }

	public WeakReference<object?> Target
	{
		get
		{
			return _target;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			_target.TryGetTarget(out object target);
			value.TryGetTarget(out object target2);
			if (target != target2)
			{
				if (_listening)
				{
					StopListening();
				}
				_target = value;
				if (_subscriber != null)
				{
					StartListening();
				}
			}
		}
	}

	public void Subscribe(Action<object?> subscriber)
	{
		if (_subscriber != null)
		{
			throw new AvaloniaInternalException("ExpressionNode can only be subscribed once.");
		}
		_subscriber = subscriber;
		Next?.Subscribe(NextValueChanged);
		StartListening();
	}

	public void Unsubscribe()
	{
		Next?.Unsubscribe();
		if (_listening)
		{
			StopListening();
		}
		LastValue = null;
		_subscriber = null;
	}

	protected virtual void StartListeningCore(WeakReference<object?> reference)
	{
		reference.TryGetTarget(out object target);
		ValueChanged(target);
	}

	protected virtual void StopListeningCore()
	{
	}

	protected virtual void NextValueChanged(object? value)
	{
		if (_subscriber != null)
		{
			(BindingNotification.ExtractError(value) as MarkupBindingChainException)?.AddNode(Description ?? "{empty}");
			_subscriber(value);
		}
	}

	protected void ValueChanged(object? value)
	{
		ValueChanged(value, notify: true);
	}

	private void ValueChanged(object? value, bool notify)
	{
		Action<object> subscriber = _subscriber;
		if (subscriber == null)
		{
			return;
		}
		BindingNotification bindingNotification = value as BindingNotification;
		ExpressionNode next = Next;
		if (bindingNotification == null)
		{
			LastValue = (WeakReference<object?>?)((value != null) ? ((ISerializable)new WeakReference<object>(value)) : ((ISerializable)NullReference));
			if (next != null)
			{
				next.Target = LastValue;
			}
			else if (notify)
			{
				subscriber(value);
			}
			return;
		}
		LastValue = (WeakReference<object?>?)((bindingNotification.Value != null) ? ((ISerializable)new WeakReference<object>(bindingNotification.Value)) : ((ISerializable)NullReference));
		if (next != null)
		{
			next.Target = LastValue;
		}
		if (next == null || bindingNotification.Error != null)
		{
			subscriber(value);
		}
	}

	private void StartListening()
	{
		_target.TryGetTarget(out object target);
		if (target == null)
		{
			ValueChanged(TargetNullNotification());
			_listening = false;
		}
		else if (target != AvaloniaProperty.UnsetValue)
		{
			_listening = true;
			StartListeningCore(_target);
		}
		else
		{
			ValueChanged(AvaloniaProperty.UnsetValue, notify: false);
			_listening = false;
		}
	}

	private void StopListening()
	{
		StopListeningCore();
		_listening = false;
	}

	private static BindingNotification TargetNullNotification()
	{
		return new BindingNotification(new MarkupBindingChainException("Null value"), BindingErrorType.Error, AvaloniaProperty.UnsetValue);
	}
}
