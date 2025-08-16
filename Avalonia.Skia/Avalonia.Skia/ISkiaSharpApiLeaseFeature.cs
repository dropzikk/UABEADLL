using Avalonia.Metadata;

namespace Avalonia.Skia;

[Unstable]
public interface ISkiaSharpApiLeaseFeature
{
	ISkiaSharpApiLease Lease();
}
