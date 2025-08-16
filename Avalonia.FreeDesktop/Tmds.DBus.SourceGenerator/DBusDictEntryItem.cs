namespace Tmds.DBus.SourceGenerator;

internal class DBusDictEntryItem : DBusItem
{
	public DBusBasicTypeItem Key { get; }

	public DBusItem Value { get; }

	public DBusDictEntryItem(DBusBasicTypeItem key, DBusItem value)
	{
		Key = key;
		Value = value;
	}
}
