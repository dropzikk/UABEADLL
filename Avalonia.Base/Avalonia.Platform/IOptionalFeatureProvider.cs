using System;

namespace Avalonia.Platform;

public interface IOptionalFeatureProvider
{
	object? TryGetFeature(Type featureType);
}
