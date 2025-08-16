namespace Avalonia.Styling.Activators;

internal class NotActivator : StyleActivatorBase, IStyleActivatorSink
{
	private readonly IStyleActivator _source;

	public NotActivator(IStyleActivator source)
	{
		_source = source;
	}

	void IStyleActivatorSink.OnNext(bool value)
	{
		ReevaluateIsActive();
	}

	protected override bool EvaluateIsActive()
	{
		return !_source.GetIsActive();
	}

	protected override void Initialize()
	{
		_source.Subscribe(this);
	}

	protected override void Deinitialize()
	{
		_source.Unsubscribe(this);
	}
}
