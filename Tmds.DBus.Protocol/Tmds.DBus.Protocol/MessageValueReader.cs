namespace Tmds.DBus.Protocol;

public delegate T MessageValueReader<T>(Message message, object? state);
