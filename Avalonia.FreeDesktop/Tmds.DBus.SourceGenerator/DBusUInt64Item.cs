namespace Tmds.DBus.SourceGenerator;

internal class DBusUInt64Item : DBusBasicTypeItem
{
	public ulong Value { get; }

	public DBusUInt64Item(ulong value)
	{
		Value = value;
	}
}
