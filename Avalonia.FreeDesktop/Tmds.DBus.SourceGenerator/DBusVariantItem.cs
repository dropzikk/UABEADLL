namespace Tmds.DBus.SourceGenerator;

internal class DBusVariantItem : DBusItem
{
	public string Signature { get; }

	public DBusItem Value { get; }

	public DBusVariantItem(string signature, DBusItem value)
	{
		Signature = signature;
		Value = value;
	}
}
