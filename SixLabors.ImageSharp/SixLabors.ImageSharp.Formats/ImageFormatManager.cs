using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SixLabors.ImageSharp.Formats;

public class ImageFormatManager
{
	private static readonly object HashLock = new object();

	private readonly ConcurrentDictionary<IImageFormat, IImageEncoder> mimeTypeEncoders = new ConcurrentDictionary<IImageFormat, IImageEncoder>();

	private readonly ConcurrentDictionary<IImageFormat, IImageDecoder> mimeTypeDecoders = new ConcurrentDictionary<IImageFormat, IImageDecoder>();

	private readonly HashSet<IImageFormat> imageFormats = new HashSet<IImageFormat>();

	private ConcurrentBag<IImageFormatDetector> imageFormatDetectors = new ConcurrentBag<IImageFormatDetector>();

	internal int MaxHeaderSize { get; private set; }

	public IEnumerable<IImageFormat> ImageFormats => imageFormats;

	internal IEnumerable<IImageFormatDetector> FormatDetectors => imageFormatDetectors;

	internal IEnumerable<KeyValuePair<IImageFormat, IImageDecoder>> ImageDecoders => mimeTypeDecoders;

	internal IEnumerable<KeyValuePair<IImageFormat, IImageEncoder>> ImageEncoders => mimeTypeEncoders;

	public void AddImageFormat(IImageFormat format)
	{
		Guard.NotNull(format, "format");
		Guard.NotNull(format.MimeTypes, "MimeTypes");
		Guard.NotNull(format.FileExtensions, "FileExtensions");
		lock (HashLock)
		{
			if (!imageFormats.Contains(format))
			{
				imageFormats.Add(format);
			}
		}
	}

	public bool TryFindFormatByFileExtension(string extension, [NotNullWhen(true)] out IImageFormat? format)
	{
		if (!string.IsNullOrWhiteSpace(extension) && extension[0] == '.')
		{
			string text = extension;
			extension = text.Substring(1, text.Length - 1);
		}
		format = imageFormats.FirstOrDefault((IImageFormat x) => x.FileExtensions.Contains<string>(extension, StringComparer.OrdinalIgnoreCase));
		return format != null;
	}

	public bool TryFindFormatByMimeType(string mimeType, [NotNullWhen(true)] out IImageFormat? format)
	{
		format = imageFormats.FirstOrDefault((IImageFormat x) => x.MimeTypes.Contains<string>(mimeType, StringComparer.OrdinalIgnoreCase));
		return format != null;
	}

	internal bool TryFindFormatByDecoder(IImageDecoder decoder, [NotNullWhen(true)] out IImageFormat? format)
	{
		format = mimeTypeDecoders.FirstOrDefault<KeyValuePair<IImageFormat, IImageDecoder>>((KeyValuePair<IImageFormat, IImageDecoder> x) => x.Value.GetType() == decoder.GetType()).Key;
		return format != null;
	}

	public void SetEncoder(IImageFormat imageFormat, IImageEncoder encoder)
	{
		Guard.NotNull(imageFormat, "imageFormat");
		Guard.NotNull(encoder, "encoder");
		AddImageFormat(imageFormat);
		mimeTypeEncoders.AddOrUpdate(imageFormat, encoder, (IImageFormat _, IImageEncoder _) => encoder);
	}

	public void SetDecoder(IImageFormat imageFormat, IImageDecoder decoder)
	{
		Guard.NotNull(imageFormat, "imageFormat");
		Guard.NotNull(decoder, "decoder");
		AddImageFormat(imageFormat);
		mimeTypeDecoders.AddOrUpdate(imageFormat, decoder, (IImageFormat _, IImageDecoder _) => decoder);
	}

	public void ClearImageFormatDetectors()
	{
		imageFormatDetectors = new ConcurrentBag<IImageFormatDetector>();
	}

	public void AddImageFormatDetector(IImageFormatDetector detector)
	{
		Guard.NotNull(detector, "detector");
		imageFormatDetectors.Add(detector);
		SetMaxHeaderSize();
	}

	public IImageDecoder GetDecoder(IImageFormat format)
	{
		Guard.NotNull(format, "format");
		if (!mimeTypeDecoders.TryGetValue(format, out IImageDecoder value))
		{
			ThrowInvalidDecoder(this);
		}
		return value;
	}

	public IImageEncoder GetEncoder(IImageFormat format)
	{
		Guard.NotNull(format, "format");
		if (!mimeTypeEncoders.TryGetValue(format, out IImageEncoder value))
		{
			ThrowInvalidDecoder(this);
		}
		return value;
	}

	private void SetMaxHeaderSize()
	{
		MaxHeaderSize = imageFormatDetectors.Max((IImageFormatDetector x) => x.HeaderSize);
	}

	[DoesNotReturn]
	internal static void ThrowInvalidDecoder(ImageFormatManager manager)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder = stringBuilder.AppendLine("Image cannot be loaded. Available decoders:");
		foreach (KeyValuePair<IImageFormat, IImageDecoder> imageDecoder in manager.ImageDecoders)
		{
			stringBuilder = stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " - {0} : {1}{2}", imageDecoder.Key.Name, imageDecoder.Value.GetType().Name, Environment.NewLine);
		}
		throw new UnknownImageFormatException(stringBuilder.ToString());
	}
}
