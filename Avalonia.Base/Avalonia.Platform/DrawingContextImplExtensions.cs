namespace Avalonia.Platform;

public static class DrawingContextImplExtensions
{
	public static T? GetFeature<T>(this IDrawingContextImpl context) where T : class
	{
		return (T)context.GetFeature(typeof(T));
	}
}
