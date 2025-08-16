using Avalonia.Skia;

namespace Avalonia;

public static class SkiaApplicationExtensions
{
	public static AppBuilder UseSkia(this AppBuilder builder)
	{
		return builder.UseRenderingSubsystem(delegate
		{
			SkiaPlatform.Initialize(AvaloniaLocator.Current.GetService<SkiaOptions>() ?? new SkiaOptions());
		}, "Skia");
	}
}
