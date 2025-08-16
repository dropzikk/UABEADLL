namespace Avalonia.Data;

public static class OptionalExtensions
{
	public static Optional<T> Cast<T>(this Optional<object?> value)
	{
		if (!value.HasValue)
		{
			return Optional<T>.Empty;
		}
		return new Optional<T>((T)value.Value);
	}
}
