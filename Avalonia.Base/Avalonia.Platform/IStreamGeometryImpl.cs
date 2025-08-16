using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable]
public interface IStreamGeometryImpl : IGeometryImpl
{
	IStreamGeometryImpl Clone();

	IStreamGeometryContextImpl Open();
}
