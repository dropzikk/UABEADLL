namespace Tmds.DBus.SourceGenerator;

internal class DBusDoubleItem : DBusBasicTypeItem
{
	public double Value { get; }

	public DBusDoubleItem(double value)
	{
		Value = value;
	}
}
