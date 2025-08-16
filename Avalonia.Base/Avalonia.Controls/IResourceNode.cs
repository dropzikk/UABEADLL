using Avalonia.Metadata;
using Avalonia.Styling;

namespace Avalonia.Controls;

[NotClientImplementable]
public interface IResourceNode
{
	bool HasResources { get; }

	bool TryGetResource(object key, ThemeVariant? theme, out object? value);
}
