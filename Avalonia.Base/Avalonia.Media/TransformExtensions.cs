using System;
using Avalonia.Media.Immutable;

namespace Avalonia.Media;

public static class TransformExtensions
{
	public static ImmutableTransform ToImmutable(this ITransform transform)
	{
		if (transform == null)
		{
			throw new ArgumentNullException("transform");
		}
		return (transform as Transform)?.ToImmutable() ?? new ImmutableTransform(transform.Value);
	}
}
