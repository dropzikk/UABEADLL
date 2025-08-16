using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Advanced;

public static class AdvancedImageExtensions
{
	public static IImageEncoder DetectEncoder(this Image source, string filePath)
	{
		Guard.NotNull(filePath, "filePath");
		string extension = Path.GetExtension(filePath);
		if (!source.Configuration.ImageFormatsManager.TryFindFormatByFileExtension(extension, out IImageFormat format))
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
			IFormatProvider provider = invariantCulture;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(67, 1, stringBuilder2, invariantCulture);
			handler.AppendLiteral("No encoder was found for extension '");
			handler.AppendFormatted(extension);
			handler.AppendLiteral("'. Registered encoders include:");
			stringBuilder = stringBuilder3.AppendLine(provider, ref handler);
			foreach (IImageFormat imageFormat in source.Configuration.ImageFormats)
			{
				stringBuilder = stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " - {0} : {1}{2}", imageFormat.Name, string.Join(", ", imageFormat.FileExtensions), Environment.NewLine);
			}
			throw new UnknownImageFormatException(stringBuilder.ToString());
		}
		IImageEncoder encoder = source.Configuration.ImageFormatsManager.GetEncoder(format);
		if (encoder == null)
		{
			StringBuilder stringBuilder4 = new StringBuilder();
			StringBuilder stringBuilder2 = stringBuilder4;
			StringBuilder stringBuilder5 = stringBuilder2;
			IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
			IFormatProvider provider2 = invariantCulture;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(89, 2, stringBuilder2, invariantCulture);
			handler.AppendLiteral("No encoder was found for extension '");
			handler.AppendFormatted(extension);
			handler.AppendLiteral("' using image format '");
			handler.AppendFormatted(format.Name);
			handler.AppendLiteral("'. Registered encoders include:");
			stringBuilder4 = stringBuilder5.AppendLine(provider2, ref handler);
			foreach (KeyValuePair<IImageFormat, IImageEncoder> imageEncoder in source.Configuration.ImageFormatsManager.ImageEncoders)
			{
				stringBuilder4 = stringBuilder4.AppendFormat(CultureInfo.InvariantCulture, " - {0} : {1}{2}", imageEncoder.Key, imageEncoder.Value.GetType().Name, Environment.NewLine);
			}
			throw new UnknownImageFormatException(stringBuilder4.ToString());
		}
		return encoder;
	}

	public static void AcceptVisitor(this Image source, IImageVisitor visitor)
	{
		source.Accept(visitor);
	}

	public static Task AcceptVisitorAsync(this Image source, IImageVisitorAsync visitor, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.AcceptAsync(visitor, cancellationToken);
	}

	public static IMemoryGroup<TPixel> GetPixelMemoryGroup<TPixel>(this ImageFrame<TPixel> source) where TPixel : unmanaged, IPixel<TPixel>
	{
		return source?.PixelBuffer.FastMemoryGroup.View ?? throw new ArgumentNullException("source");
	}

	public static IMemoryGroup<TPixel> GetPixelMemoryGroup<TPixel>(this Image<TPixel> source) where TPixel : unmanaged, IPixel<TPixel>
	{
		return source?.Frames.RootFrame.GetPixelMemoryGroup() ?? throw new ArgumentNullException("source");
	}

	public static Memory<TPixel> DangerousGetPixelRowMemory<TPixel>(this ImageFrame<TPixel> source, int rowIndex) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(source, "source");
		Guard.MustBeGreaterThanOrEqualTo(rowIndex, 0, "rowIndex");
		Guard.MustBeLessThan(rowIndex, source.Height, "rowIndex");
		return source.PixelBuffer.GetSafeRowMemory(rowIndex);
	}

	public static Memory<TPixel> DangerousGetPixelRowMemory<TPixel>(this Image<TPixel> source, int rowIndex) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(source, "source");
		Guard.MustBeGreaterThanOrEqualTo(rowIndex, 0, "rowIndex");
		Guard.MustBeLessThan(rowIndex, source.Height, "rowIndex");
		return source.Frames.RootFrame.PixelBuffer.GetSafeRowMemory(rowIndex);
	}
}
