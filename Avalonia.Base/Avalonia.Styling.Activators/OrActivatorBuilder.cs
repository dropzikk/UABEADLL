namespace Avalonia.Styling.Activators;

internal struct OrActivatorBuilder
{
	private IStyleActivator? _single;

	private OrActivator? _multiple;

	public int Count
	{
		get
		{
			OrActivator? multiple = _multiple;
			if (multiple == null)
			{
				if (_single == null)
				{
					return 0;
				}
				return 1;
			}
			return multiple.Count;
		}
	}

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
			_multiple = new OrActivator();
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
