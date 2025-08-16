using SkiaSharp;

namespace Avalonia.Skia;

internal class SKPaintCache : SKCacheBase<SKPaint, SKPaintCache>
{
	public void ReturnReset(SKPaint paint)
	{
		paint.Reset();
		Cache.Add(paint);
	}
}
