namespace Tmds.DBus.SourceGenerator;

internal class DBusBoolItem : DBusBasicTypeItem
{
	public bool Value { get; }

	public DBusBoolItem(bool value)
	{
		Value = value;
	}
}
