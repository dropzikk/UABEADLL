using System;

namespace Avalonia.PropertyStore;

internal readonly struct PropertyNotifying : IDisposable
{
	private readonly AvaloniaObject _owner;

	private readonly AvaloniaProperty _property;

	private PropertyNotifying(AvaloniaObject owner, AvaloniaProperty property)
	{
		_owner = owner;
		_property = property;
		_property.Notifying(owner, arg2: true);
	}

	public void Dispose()
	{
		_property.Notifying(_owner, arg2: false);
	}

	public static PropertyNotifying? Start(AvaloniaObject owner, AvaloniaProperty property)
	{
		if (property.Notifying == null)
		{
			return null;
		}
		return new PropertyNotifying(owner, property);
	}
}
