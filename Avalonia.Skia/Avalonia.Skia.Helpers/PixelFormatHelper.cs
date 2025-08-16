using Avalonia.Compatibility;
using Avalonia.Platform;
using SkiaSharp;

namespace Avalonia.Skia.Helpers;

public static class PixelFormatHelper
{
	public static SKColorType ResolveColorType(PixelFormat? format)
	{
		SKColorType result = format?.ToSkColorType() ?? SKImageInfo.PlatformColorType;
		if (OperatingSystemEx.IsLinux())
		{
			result = SKColorType.Bgra8888;
		}
		return result;
	}
}
