using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal class DBusSignatureItem : DBusBasicTypeItem
{
	public Signature Value { get; }

	public DBusSignatureItem(Signature value)
	{
		Value = value;
	}
}
