using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal class DBusObjectPathItem : DBusBasicTypeItem
{
	public ObjectPath Value { get; }

	public DBusObjectPathItem(ObjectPath value)
	{
		Value = value;
	}
}
