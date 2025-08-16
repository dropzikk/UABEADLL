namespace Tmds.DBus.Protocol;

internal enum MessageHeader : byte
{
	Path = 1,
	Interface,
	Member,
	ErrorName,
	ReplySerial,
	Destination,
	Sender,
	Signature,
	UnixFds
}
