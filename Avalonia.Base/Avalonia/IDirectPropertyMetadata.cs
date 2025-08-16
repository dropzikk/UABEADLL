using Avalonia.Metadata;

namespace Avalonia;

[NotClientImplementable]
public interface IDirectPropertyMetadata
{
	object? UnsetValue { get; }

	bool? EnableDataValidation { get; }
}
