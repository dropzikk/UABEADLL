using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Pbm;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Qoi;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp;

public static class ImageExtensions
{
	public static void SaveAsBmp(this Image source, string path)
	{
		source.SaveAsBmp(path, null);
	}

	public static Task SaveAsBmpAsync(this Image source, string path)
	{
		return source.SaveAsBmpAsync(path, default(CancellationToken));
	}

	public static Task SaveAsBmpAsync(this Image source, string path, CancellationToken cancellationToken)
	{
		return source.SaveAsBmpAsync(path, null, cancellationToken);
	}

	public static void SaveAsBmp(this Image source, string path, BmpEncoder encoder)
	{
		source.Save(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(BmpFormat.Instance));
	}

	public static Task SaveAsBmpAsync(this Image source, string path, BmpEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(BmpFormat.Instance), cancellationToken);
	}

	public static void SaveAsBmp(this Image source, Stream stream)
	{
		source.SaveAsBmp(stream, null);
	}

	public static Task SaveAsBmpAsync(this Image source, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsBmpAsync(stream, null, cancellationToken);
	}

	public static void SaveAsBmp(this Image source, Stream stream, BmpEncoder encoder)
	{
		source.Save(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(BmpFormat.Instance));
	}

	public static Task SaveAsBmpAsync(this Image source, Stream stream, BmpEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(BmpFormat.Instance), cancellationToken);
	}

	public static void SaveAsGif(this Image source, string path)
	{
		source.SaveAsGif(path, null);
	}

	public static Task SaveAsGifAsync(this Image source, string path)
	{
		return source.SaveAsGifAsync(path, default(CancellationToken));
	}

	public static Task SaveAsGifAsync(this Image source, string path, CancellationToken cancellationToken)
	{
		return source.SaveAsGifAsync(path, null, cancellationToken);
	}

	public static void SaveAsGif(this Image source, string path, GifEncoder encoder)
	{
		source.Save(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(GifFormat.Instance));
	}

	public static Task SaveAsGifAsync(this Image source, string path, GifEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(GifFormat.Instance), cancellationToken);
	}

	public static void SaveAsGif(this Image source, Stream stream)
	{
		source.SaveAsGif(stream, null);
	}

	public static Task SaveAsGifAsync(this Image source, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsGifAsync(stream, null, cancellationToken);
	}

	public static void SaveAsGif(this Image source, Stream stream, GifEncoder encoder)
	{
		source.Save(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(GifFormat.Instance));
	}

	public static Task SaveAsGifAsync(this Image source, Stream stream, GifEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(GifFormat.Instance), cancellationToken);
	}

	public static void SaveAsJpeg(this Image source, string path)
	{
		source.SaveAsJpeg(path, null);
	}

	public static Task SaveAsJpegAsync(this Image source, string path)
	{
		return source.SaveAsJpegAsync(path, default(CancellationToken));
	}

	public static Task SaveAsJpegAsync(this Image source, string path, CancellationToken cancellationToken)
	{
		return source.SaveAsJpegAsync(path, null, cancellationToken);
	}

	public static void SaveAsJpeg(this Image source, string path, JpegEncoder encoder)
	{
		source.Save(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(JpegFormat.Instance));
	}

	public static Task SaveAsJpegAsync(this Image source, string path, JpegEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(JpegFormat.Instance), cancellationToken);
	}

	public static void SaveAsJpeg(this Image source, Stream stream)
	{
		source.SaveAsJpeg(stream, null);
	}

	public static Task SaveAsJpegAsync(this Image source, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsJpegAsync(stream, null, cancellationToken);
	}

	public static void SaveAsJpeg(this Image source, Stream stream, JpegEncoder encoder)
	{
		source.Save(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(JpegFormat.Instance));
	}

	public static Task SaveAsJpegAsync(this Image source, Stream stream, JpegEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(JpegFormat.Instance), cancellationToken);
	}

	public static void SaveAsPbm(this Image source, string path)
	{
		source.SaveAsPbm(path, null);
	}

	public static Task SaveAsPbmAsync(this Image source, string path)
	{
		return source.SaveAsPbmAsync(path, default(CancellationToken));
	}

	public static Task SaveAsPbmAsync(this Image source, string path, CancellationToken cancellationToken)
	{
		return source.SaveAsPbmAsync(path, null, cancellationToken);
	}

	public static void SaveAsPbm(this Image source, string path, PbmEncoder encoder)
	{
		source.Save(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(PbmFormat.Instance));
	}

	public static Task SaveAsPbmAsync(this Image source, string path, PbmEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(PbmFormat.Instance), cancellationToken);
	}

	public static void SaveAsPbm(this Image source, Stream stream)
	{
		source.SaveAsPbm(stream, null);
	}

	public static Task SaveAsPbmAsync(this Image source, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsPbmAsync(stream, null, cancellationToken);
	}

	public static void SaveAsPbm(this Image source, Stream stream, PbmEncoder encoder)
	{
		source.Save(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(PbmFormat.Instance));
	}

	public static Task SaveAsPbmAsync(this Image source, Stream stream, PbmEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(PbmFormat.Instance), cancellationToken);
	}

	public static void SaveAsPng(this Image source, string path)
	{
		source.SaveAsPng(path, null);
	}

	public static Task SaveAsPngAsync(this Image source, string path)
	{
		return source.SaveAsPngAsync(path, default(CancellationToken));
	}

	public static Task SaveAsPngAsync(this Image source, string path, CancellationToken cancellationToken)
	{
		return source.SaveAsPngAsync(path, null, cancellationToken);
	}

	public static void SaveAsPng(this Image source, string path, PngEncoder encoder)
	{
		source.Save(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(PngFormat.Instance));
	}

	public static Task SaveAsPngAsync(this Image source, string path, PngEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(PngFormat.Instance), cancellationToken);
	}

	public static void SaveAsPng(this Image source, Stream stream)
	{
		source.SaveAsPng(stream, null);
	}

	public static Task SaveAsPngAsync(this Image source, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsPngAsync(stream, null, cancellationToken);
	}

	public static void SaveAsPng(this Image source, Stream stream, PngEncoder encoder)
	{
		source.Save(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(PngFormat.Instance));
	}

	public static Task SaveAsPngAsync(this Image source, Stream stream, PngEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(PngFormat.Instance), cancellationToken);
	}

	public static void SaveAsQoi(this Image source, string path)
	{
		source.SaveAsQoi(path, null);
	}

	public static Task SaveAsQoiAsync(this Image source, string path)
	{
		return source.SaveAsQoiAsync(path, default(CancellationToken));
	}

	public static Task SaveAsQoiAsync(this Image source, string path, CancellationToken cancellationToken)
	{
		return source.SaveAsQoiAsync(path, null, cancellationToken);
	}

	public static void SaveAsQoi(this Image source, string path, QoiEncoder encoder)
	{
		source.Save(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(QoiFormat.Instance));
	}

	public static Task SaveAsQoiAsync(this Image source, string path, QoiEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(QoiFormat.Instance), cancellationToken);
	}

	public static void SaveAsQoi(this Image source, Stream stream)
	{
		source.SaveAsQoi(stream, null);
	}

	public static Task SaveAsQoiAsync(this Image source, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsQoiAsync(stream, null, cancellationToken);
	}

	public static void SaveAsQoi(this Image source, Stream stream, QoiEncoder encoder)
	{
		source.Save(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(QoiFormat.Instance));
	}

	public static Task SaveAsQoiAsync(this Image source, Stream stream, QoiEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(QoiFormat.Instance), cancellationToken);
	}

	public static void SaveAsTga(this Image source, string path)
	{
		source.SaveAsTga(path, null);
	}

	public static Task SaveAsTgaAsync(this Image source, string path)
	{
		return source.SaveAsTgaAsync(path, default(CancellationToken));
	}

	public static Task SaveAsTgaAsync(this Image source, string path, CancellationToken cancellationToken)
	{
		return source.SaveAsTgaAsync(path, null, cancellationToken);
	}

	public static void SaveAsTga(this Image source, string path, TgaEncoder encoder)
	{
		source.Save(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(TgaFormat.Instance));
	}

	public static Task SaveAsTgaAsync(this Image source, string path, TgaEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(TgaFormat.Instance), cancellationToken);
	}

	public static void SaveAsTga(this Image source, Stream stream)
	{
		source.SaveAsTga(stream, null);
	}

	public static Task SaveAsTgaAsync(this Image source, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsTgaAsync(stream, null, cancellationToken);
	}

	public static void SaveAsTga(this Image source, Stream stream, TgaEncoder encoder)
	{
		source.Save(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(TgaFormat.Instance));
	}

	public static Task SaveAsTgaAsync(this Image source, Stream stream, TgaEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(TgaFormat.Instance), cancellationToken);
	}

	public static void SaveAsTiff(this Image source, string path)
	{
		source.SaveAsTiff(path, null);
	}

	public static Task SaveAsTiffAsync(this Image source, string path)
	{
		return source.SaveAsTiffAsync(path, default(CancellationToken));
	}

	public static Task SaveAsTiffAsync(this Image source, string path, CancellationToken cancellationToken)
	{
		return source.SaveAsTiffAsync(path, null, cancellationToken);
	}

	public static void SaveAsTiff(this Image source, string path, TiffEncoder encoder)
	{
		source.Save(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(TiffFormat.Instance));
	}

	public static Task SaveAsTiffAsync(this Image source, string path, TiffEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(TiffFormat.Instance), cancellationToken);
	}

	public static void SaveAsTiff(this Image source, Stream stream)
	{
		source.SaveAsTiff(stream, null);
	}

	public static Task SaveAsTiffAsync(this Image source, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsTiffAsync(stream, null, cancellationToken);
	}

	public static void SaveAsTiff(this Image source, Stream stream, TiffEncoder encoder)
	{
		source.Save(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(TiffFormat.Instance));
	}

	public static Task SaveAsTiffAsync(this Image source, Stream stream, TiffEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(TiffFormat.Instance), cancellationToken);
	}

	public static void SaveAsWebp(this Image source, string path)
	{
		source.SaveAsWebp(path, null);
	}

	public static Task SaveAsWebpAsync(this Image source, string path)
	{
		return source.SaveAsWebpAsync(path, default(CancellationToken));
	}

	public static Task SaveAsWebpAsync(this Image source, string path, CancellationToken cancellationToken)
	{
		return source.SaveAsWebpAsync(path, null, cancellationToken);
	}

	public static void SaveAsWebp(this Image source, string path, WebpEncoder encoder)
	{
		source.Save(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(WebpFormat.Instance));
	}

	public static Task SaveAsWebpAsync(this Image source, string path, WebpEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(path, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(WebpFormat.Instance), cancellationToken);
	}

	public static void SaveAsWebp(this Image source, Stream stream)
	{
		source.SaveAsWebp(stream, null);
	}

	public static Task SaveAsWebpAsync(this Image source, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsWebpAsync(stream, null, cancellationToken);
	}

	public static void SaveAsWebp(this Image source, Stream stream, WebpEncoder encoder)
	{
		source.Save(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(WebpFormat.Instance));
	}

	public static Task SaveAsWebpAsync(this Image source, Stream stream, WebpEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(stream, encoder ?? source.Configuration.ImageFormatsManager.GetEncoder(WebpFormat.Instance), cancellationToken);
	}

	public static void Save(this Image source, string path)
	{
		source.Save(path, source.DetectEncoder(path));
	}

	public static Task SaveAsync(this Image source, string path, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SaveAsync(path, source.DetectEncoder(path), cancellationToken);
	}

	public static void Save(this Image source, string path, IImageEncoder encoder)
	{
		Guard.NotNull(path, "path");
		Guard.NotNull(encoder, "encoder");
		using Stream stream = source.Configuration.FileSystem.Create(path);
		source.Save(stream, encoder);
	}

	public static async Task SaveAsync(this Image source, string path, IImageEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		Guard.NotNull(path, "path");
		Guard.NotNull(encoder, "encoder");
		await using Stream fs = source.Configuration.FileSystem.CreateAsynchronous(path);
		await source.SaveAsync(fs, encoder, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public static void Save(this Image source, Stream stream, IImageFormat format)
	{
		Guard.NotNull(stream, "stream");
		Guard.NotNull(format, "format");
		if (!stream.CanWrite)
		{
			throw new NotSupportedException("Cannot write to the stream.");
		}
		IImageEncoder encoder = source.Configuration.ImageFormatsManager.GetEncoder(format);
		if (encoder == null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("No encoder was found for the provided mime type. Registered encoders include:");
			foreach (KeyValuePair<IImageFormat, IImageEncoder> imageEncoder in source.Configuration.ImageFormatsManager.ImageEncoders)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " - {0} : {1}{2}", imageEncoder.Key.Name, imageEncoder.Value.GetType().Name, Environment.NewLine);
			}
			throw new NotSupportedException(stringBuilder.ToString());
		}
		source.Save(stream, encoder);
	}

	public static Task SaveAsync(this Image source, Stream stream, IImageFormat format, CancellationToken cancellationToken = default(CancellationToken))
	{
		Guard.NotNull(stream, "stream");
		Guard.NotNull(format, "format");
		if (!stream.CanWrite)
		{
			throw new NotSupportedException("Cannot write to the stream.");
		}
		IImageEncoder encoder = source.Configuration.ImageFormatsManager.GetEncoder(format);
		if (encoder == null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("No encoder was found for the provided mime type. Registered encoders include:");
			foreach (KeyValuePair<IImageFormat, IImageEncoder> imageEncoder in source.Configuration.ImageFormatsManager.ImageEncoders)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " - {0} : {1}{2}", imageEncoder.Key.Name, imageEncoder.Value.GetType().Name, Environment.NewLine);
			}
			throw new NotSupportedException(stringBuilder.ToString());
		}
		return source.SaveAsync(stream, encoder, cancellationToken);
	}

	public static string ToBase64String(this Image source, IImageFormat format)
	{
		Guard.NotNull(format, "format");
		using MemoryStream memoryStream = new MemoryStream();
		source.Save(memoryStream, format);
		memoryStream.TryGetBuffer(out var buffer);
		return "data:" + format.DefaultMimeType + ";base64," + Convert.ToBase64String(buffer.Array ?? Array.Empty<byte>(), 0, (int)memoryStream.Length);
	}

	internal static Buffer2D<TPixel> GetRootFramePixelBuffer<TPixel>(this Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		return image.Frames.RootFrame.PixelBuffer;
	}
}
