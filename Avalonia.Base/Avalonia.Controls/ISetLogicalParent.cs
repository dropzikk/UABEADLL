using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace Avalonia.Controls;

[NotClientImplementable]
public interface ISetLogicalParent
{
	void SetParent(ILogical? parent);
}
