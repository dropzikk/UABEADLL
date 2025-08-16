namespace Tmds.DBus.SourceGenerator;

internal class DBusStringItem : DBusBasicTypeItem
{
	public string Value { get; }

	public DBusStringItem(string value)
	{
		Value = value;
	}
}
