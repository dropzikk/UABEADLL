namespace Tmds.DBus.Protocol;

public ref struct ArrayEnd
{
	internal readonly DBusType Type;

	internal readonly int EndOfArray;

	internal ArrayEnd(DBusType type, int endOfArray)
	{
		Type = type;
		EndOfArray = endOfArray;
	}
}
