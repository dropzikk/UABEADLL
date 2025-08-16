namespace Avalonia.Layout;

public interface IEmbeddedLayoutRoot : ILayoutRoot
{
	Size AllocatedSize { get; }
}
