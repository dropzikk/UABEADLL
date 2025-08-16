using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Utilities;

namespace Avalonia.Media.Imaging;

public class RenderTargetBitmap : Bitmap
{
	internal new IRef<IRenderTargetBitmapImpl> PlatformImpl { get; }

	public RenderTargetBitmap(PixelSize pixelSize)
		: this(pixelSize, new Vector(96.0, 96.0))
	{
	}

	public RenderTargetBitmap(PixelSize pixelSize, Vector dpi)
		: this(RefCountable.Create(CreateImpl(pixelSize, dpi)))
	{
	}

	private RenderTargetBitmap(IRef<IRenderTargetBitmapImpl> impl)
		: base(impl)
	{
		PlatformImpl = impl;
	}

	public void Render(Visual visual)
	{
		using DrawingContext context = CreateDrawingContext();
		ImmediateRenderer.Render(visual, context);
	}

	private static IRenderTargetBitmapImpl CreateImpl(PixelSize size, Vector dpi)
	{
		return AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>().CreateRenderTargetBitmap(size, dpi);
	}

	public DrawingContext CreateDrawingContext()
	{
		IDrawingContextImpl drawingContextImpl = PlatformImpl.Item.CreateDrawingContext();
		drawingContextImpl.Clear(Colors.Transparent);
		return new PlatformDrawingContext(drawingContextImpl);
	}

	public override void Dispose()
	{
		PlatformImpl.Dispose();
		base.Dispose();
	}
}
