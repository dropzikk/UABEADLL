using Avalonia.Metadata;

namespace Avalonia;

[NotClientImplementable]
public interface IStyledPropertyMetadata
{
	object? DefaultValue { get; }
}
