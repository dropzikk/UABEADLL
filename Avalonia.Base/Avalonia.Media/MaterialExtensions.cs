using System;

namespace Avalonia.Media;

public static class MaterialExtensions
{
	public static IExperimentalAcrylicMaterial ToImmutable(this IExperimentalAcrylicMaterial material)
	{
		if (material == null)
		{
			throw new ArgumentNullException("material");
		}
		return (material as IMutableExperimentalAcrylicMaterial)?.ToImmutable() ?? material;
	}
}
