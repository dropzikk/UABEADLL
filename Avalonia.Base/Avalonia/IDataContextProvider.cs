using Avalonia.Metadata;

namespace Avalonia;

[NotClientImplementable]
public interface IDataContextProvider
{
	object? DataContext { get; set; }
}
