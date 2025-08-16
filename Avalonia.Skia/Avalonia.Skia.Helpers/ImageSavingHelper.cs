using System;
using System.IO;
using SkiaSharp;

namespace Avalonia.Skia.Helpers;

public static class ImageSavingHelper
{
	public static void SaveImage(SKImage image, string fileName, int? quality = null)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (fileName == null)
		{
			throw new ArgumentNullException("fileName");
		}
		using FileStream stream = File.Create(fileName);
		SaveImage(image, stream, quality);
	}

	public static void SaveImage(SKImage image, Stream stream, int? quality = null)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		if (!quality.HasValue)
		{
			using (SKData sKData = image.Encode())
			{
				sKData.SaveTo(stream);
				return;
			}
		}
		using SKData sKData2 = image.Encode(SKEncodedImageFormat.Png, quality.Value);
		sKData2.SaveTo(stream);
	}

	internal static void SavePicture(SKPicture picture, float scale, string path)
	{
		SKSizeI dimensions = new SKSizeI((int)Math.Ceiling(picture.CullRect.Width * scale), (int)Math.Ceiling(picture.CullRect.Height * scale));
		using SKImage image = SKImage.FromPicture(picture, dimensions, SKMatrix.CreateScale(scale, scale));
		SaveImage(image, path, null);
	}
}
