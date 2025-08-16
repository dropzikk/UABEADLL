namespace Tmds.DBus.SourceGenerator;

internal class DBusInt16Item : DBusBasicTypeItem
{
	public short Value { get; }

	public DBusInt16Item(short value)
	{
		Value = value;
	}
}
