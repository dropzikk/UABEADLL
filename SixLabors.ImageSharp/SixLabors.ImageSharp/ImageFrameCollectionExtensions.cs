using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp;

public static class ImageFrameCollectionExtensions
{
	public static IEnumerable<ImageFrame<TPixel>> AsEnumerable<TPixel>(this ImageFrameCollection<TPixel> source) where TPixel : unmanaged, IPixel<TPixel>
	{
		return source;
	}

	public static IEnumerable<TResult> Select<TPixel, TResult>(this ImageFrameCollection<TPixel> source, Func<ImageFrame<TPixel>, TResult> selector) where TPixel : unmanaged, IPixel<TPixel>
	{
		return source.AsEnumerable().Select(selector);
	}
}
