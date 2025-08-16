using Avalonia.Metadata;

namespace Avalonia.Data;

[NotClientImplementable]
public interface IBinding
{
	InstancedBinding? Initiate(AvaloniaObject target, AvaloniaProperty? targetProperty, object? anchor = null, bool enableDataValidation = false);
}
