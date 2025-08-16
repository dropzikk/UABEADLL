namespace Tmds.DBus.Protocol;

public enum MessageType : byte
{
	MethodCall = 1,
	MethodReturn,
	Error,
	Signal
}
