using System.Collections.Concurrent;
using SkiaSharp;

namespace Avalonia.Skia;

internal class SKRoundRectCache : SKCacheBase<SKRoundRect, SKRoundRectCache>
{
	private readonly ConcurrentBag<SKPoint[]> _radiiCache = new ConcurrentBag<SKPoint[]>();

	public SKRoundRect GetAndSetRadii(in SKRect rectangle, in RoundedRect roundedRect)
	{
		if (!Cache.TryTake(out SKRoundRect result))
		{
			result = new SKRoundRect();
		}
		if (!_radiiCache.TryTake(out SKPoint[] result2))
		{
			result2 = new SKPoint[4];
		}
		result2[0].X = (float)roundedRect.RadiiTopLeft.X;
		result2[0].Y = (float)roundedRect.RadiiTopLeft.Y;
		result2[1].X = (float)roundedRect.RadiiTopRight.X;
		result2[1].Y = (float)roundedRect.RadiiTopRight.Y;
		result2[2].X = (float)roundedRect.RadiiBottomRight.X;
		result2[2].Y = (float)roundedRect.RadiiBottomRight.Y;
		result2[3].X = (float)roundedRect.RadiiBottomLeft.X;
		result2[3].Y = (float)roundedRect.RadiiBottomLeft.Y;
		result.SetRectRadii(rectangle, result2);
		_radiiCache.Add(result2);
		return result;
	}

	public SKRoundRect GetAndSetRadii(in SKRect rectangle, in SKPoint[] radii)
	{
		if (!Cache.TryTake(out SKRoundRect result))
		{
			result = new SKRoundRect();
		}
		result.SetRectRadii(rectangle, radii);
		return result;
	}

	public void ReturnReset(SKRoundRect rect)
	{
		rect.SetEmpty();
		Cache.Add(rect);
	}

	public new void Clear()
	{
		base.Clear();
		SKPoint[] result;
		while (_radiiCache.TryTake(out result))
		{
		}
	}
}
