namespace Tmds.DBus.SourceGenerator;

internal class DBusInt32Item : DBusBasicTypeItem
{
	public int Value { get; }

	public DBusInt32Item(int value)
	{
		Value = value;
	}
}
