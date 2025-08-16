using Avalonia.Metadata;

namespace Avalonia.Controls;

[NotClientImplementable]
public interface ISetInheritanceParent
{
	void SetParent(AvaloniaObject? parent);
}
