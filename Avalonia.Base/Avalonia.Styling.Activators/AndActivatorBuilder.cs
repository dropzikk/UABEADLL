namespace Avalonia.Styling.Activators;

internal struct AndActivatorBuilder
{
	private IStyleActivator? _single;

	private AndActivator? _multiple;

	public void Add(IStyleActivator? activator)
	{
		if (activator == null)
		{
			return;
		}
		if (_single == null && _multiple == null)
		{
			_single = activator;
			return;
		}
		if (_multiple == null)
		{
			_multiple = new AndActivator();
			_multiple.Add(_single);
			_single = null;
		}
		_multiple.Add(activator);
	}

	public IStyleActivator Get()
	{
		return _single ?? _multiple;
	}
}
