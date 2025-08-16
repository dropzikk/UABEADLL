namespace Tmds.DBus.SourceGenerator;

internal class DBusInt64Item : DBusBasicTypeItem
{
	public long Value { get; }

	public DBusInt64Item(long value)
	{
		Value = value;
	}
}
