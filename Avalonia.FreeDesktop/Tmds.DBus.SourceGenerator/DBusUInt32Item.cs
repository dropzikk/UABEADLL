namespace Tmds.DBus.SourceGenerator;

internal class DBusUInt32Item : DBusBasicTypeItem
{
	public uint Value { get; }

	public DBusUInt32Item(uint value)
	{
		Value = value;
	}
}
