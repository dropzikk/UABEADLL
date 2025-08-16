using System;

namespace Avalonia.Styling.Activators;

internal abstract class StyleActivatorBase : IStyleActivator, IDisposable
{
	private IStyleActivatorSink? _sink;

	private bool? _value;

	public bool IsSubscribed => _sink != null;

	public bool GetIsActive()
	{
		bool flag = EvaluateIsActive();
		bool valueOrDefault = _value == true;
		if (!_value.HasValue)
		{
			valueOrDefault = flag;
			_value = valueOrDefault;
		}
		return flag;
	}

	public void Subscribe(IStyleActivatorSink sink)
	{
		if (_sink == null)
		{
			Initialize();
			_sink = sink;
			return;
		}
		throw new AvaloniaInternalException("StyleActivator is already subscribed.");
	}

	public void Unsubscribe(IStyleActivatorSink sink)
	{
		if (_sink != null)
		{
			if (_sink != sink)
			{
				throw new AvaloniaInternalException("StyleActivatorSink is not subscribed.");
			}
			_sink = null;
			Deinitialize();
		}
	}

	public void Dispose()
	{
		_sink = null;
		Deinitialize();
	}

	protected abstract bool EvaluateIsActive();

	protected bool ReevaluateIsActive()
	{
		bool isActive = GetIsActive();
		if (isActive != _value)
		{
			_value = isActive;
			_sink?.OnNext(isActive);
		}
		return isActive;
	}

	protected abstract void Initialize();

	protected abstract void Deinitialize();
}
