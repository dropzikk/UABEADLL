namespace Avalonia.Data.Converters;

public static class StringConverters
{
	public static readonly IValueConverter IsNullOrEmpty = new FuncValueConverter<string, bool>(string.IsNullOrEmpty);

	public static readonly IValueConverter IsNotNullOrEmpty = new FuncValueConverter<string, bool>((string? x) => !string.IsNullOrEmpty(x));
}
