using System.Diagnostics.CodeAnalysis;

namespace Avalonia.Platform;

public static class OptionalFeatureProviderExtensions
{
	public static T? TryGetFeature<T>(this IOptionalFeatureProvider provider) where T : class
	{
		return (T)provider.TryGetFeature(typeof(T));
	}

	public static bool TryGetFeature<T>(this IOptionalFeatureProvider provider, [MaybeNullWhen(false)] out T rv) where T : class
	{
		rv = provider.TryGetFeature<T>();
		return rv != null;
	}
}
