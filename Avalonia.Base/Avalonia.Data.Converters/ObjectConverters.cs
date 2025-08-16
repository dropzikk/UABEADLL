namespace Avalonia.Data.Converters;

public static class ObjectConverters
{
	public static readonly IValueConverter IsNull = new FuncValueConverter<object, bool>((object? x) => x == null);

	public static readonly IValueConverter IsNotNull = new FuncValueConverter<object, bool>((object? x) => x != null);
}
