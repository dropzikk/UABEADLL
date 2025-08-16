namespace Tmds.DBus.Protocol;

public enum DBusType : byte
{
	Invalid = 0,
	Byte = 121,
	Bool = 98,
	Int16 = 110,
	UInt16 = 113,
	Int32 = 105,
	UInt32 = 117,
	Int64 = 120,
	UInt64 = 116,
	Double = 100,
	String = 115,
	ObjectPath = 111,
	Signature = 103,
	Array = 97,
	Struct = 40,
	Variant = 118,
	DictEntry = 123,
	UnixFd = 104
}
