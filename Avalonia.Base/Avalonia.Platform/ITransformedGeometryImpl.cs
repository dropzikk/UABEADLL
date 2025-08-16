using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable]
public interface ITransformedGeometryImpl : IGeometryImpl
{
	IGeometryImpl SourceGeometry { get; }

	Matrix Transform { get; }
}
