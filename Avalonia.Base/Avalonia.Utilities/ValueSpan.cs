namespace Avalonia.Utilities;

public readonly record struct ValueSpan<T>(int start, int length, T value);
