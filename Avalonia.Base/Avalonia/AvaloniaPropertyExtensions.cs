using System;
using Avalonia.Media;

namespace Avalonia;

internal static class AvaloniaPropertyExtensions
{
	public static bool CanValueAffectRender(this AvaloniaProperty property)
	{
		Type propertyType = property.PropertyType;
		return !propertyType.IsSealed || typeof(IAffectsRender).IsAssignableFrom(propertyType);
	}
}
