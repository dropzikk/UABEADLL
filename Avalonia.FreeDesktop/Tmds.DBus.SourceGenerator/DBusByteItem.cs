namespace Tmds.DBus.SourceGenerator;

internal class DBusByteItem : DBusBasicTypeItem
{
	public byte Value { get; }

	public DBusByteItem(byte value)
	{
		Value = value;
	}
}
