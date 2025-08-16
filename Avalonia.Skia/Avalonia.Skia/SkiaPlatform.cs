using Avalonia.Platform;

namespace Avalonia.Skia;

public static class SkiaPlatform
{
	public static Vector DefaultDpi => new Vector(96.0, 96.0);

	public static void Initialize()
	{
		Initialize(new SkiaOptions());
	}

	public static void Initialize(SkiaOptions options)
	{
		PlatformRenderInterface constant = new PlatformRenderInterface(options.MaxGpuResourceSizeBytes);
		AvaloniaLocator.CurrentMutable.Bind<IPlatformRenderInterface>().ToConstant(constant).Bind<IFontManagerImpl>()
			.ToConstant(new FontManagerImpl())
			.Bind<ITextShaperImpl>()
			.ToConstant(new TextShaperImpl());
	}
}
