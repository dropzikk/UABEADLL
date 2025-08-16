namespace Tmds.DBus.Protocol;

public struct ObjectPath
{
	private string _value;

	public ObjectPath(string value)
	{
		_value = value;
	}

	public override string ToString()
	{
		return _value ?? "";
	}

	public static implicit operator string(ObjectPath value)
	{
		return value._value;
	}

	public static implicit operator ObjectPath(string value)
	{
		return new ObjectPath(value);
	}
}
