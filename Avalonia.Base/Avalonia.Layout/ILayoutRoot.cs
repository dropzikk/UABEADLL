using Avalonia.Metadata;

namespace Avalonia.Layout;

[NotClientImplementable]
public interface ILayoutRoot
{
	Size ClientSize { get; }

	double LayoutScaling { get; }

	internal ILayoutManager LayoutManager { get; }
}
