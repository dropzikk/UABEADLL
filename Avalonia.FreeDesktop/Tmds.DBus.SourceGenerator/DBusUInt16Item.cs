namespace Tmds.DBus.SourceGenerator;

internal class DBusUInt16Item : DBusBasicTypeItem
{
	public ushort Value { get; }

	public DBusUInt16Item(ushort value)
	{
		Value = value;
	}
}
