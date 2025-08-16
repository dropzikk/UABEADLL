using System;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

public sealed class AutoOrientProcessor : IImageProcessor
{
	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new AutoOrientProcessor<TPixel>(configuration, source, sourceRectangle);
	}
}
internal class AutoOrientProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	public AutoOrientProcessor(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
	}

	protected override void BeforeImageApply()
	{
		ushort exifOrientation = GetExifOrientation(base.Source);
		Size size = base.SourceRectangle.Size;
		switch (exifOrientation)
		{
		case 2:
			new FlipProcessor(FlipMode.Horizontal).Execute(base.Configuration, base.Source, base.SourceRectangle);
			break;
		case 3:
			new RotateProcessor(180f, size).Execute(base.Configuration, base.Source, base.SourceRectangle);
			break;
		case 4:
			new FlipProcessor(FlipMode.Vertical).Execute(base.Configuration, base.Source, base.SourceRectangle);
			break;
		case 5:
			new RotateProcessor(90f, size).Execute(base.Configuration, base.Source, base.SourceRectangle);
			new FlipProcessor(FlipMode.Horizontal).Execute(base.Configuration, base.Source, base.SourceRectangle);
			break;
		case 6:
			new RotateProcessor(90f, size).Execute(base.Configuration, base.Source, base.SourceRectangle);
			break;
		case 7:
			new FlipProcessor(FlipMode.Vertical).Execute(base.Configuration, base.Source, base.SourceRectangle);
			new RotateProcessor(270f, size).Execute(base.Configuration, base.Source, base.SourceRectangle);
			break;
		case 8:
			new RotateProcessor(270f, size).Execute(base.Configuration, base.Source, base.SourceRectangle);
			break;
		}
		base.BeforeImageApply();
	}

	protected override void OnFrameApply(ImageFrame<TPixel> sourceBase)
	{
	}

	private static ushort GetExifOrientation(Image<TPixel> source)
	{
		if (source.Metadata.ExifProfile == null)
		{
			return 0;
		}
		if (!source.Metadata.ExifProfile.TryGetValue(ExifTag.Orientation, out IExifValue<ushort> exifValue))
		{
			return 0;
		}
		ushort result;
		if (exifValue.DataType == ExifDataType.Short)
		{
			result = exifValue.Value;
		}
		else
		{
			result = Convert.ToUInt16(exifValue.Value);
			source.Metadata.ExifProfile.RemoveValue(ExifTag.Orientation);
		}
		source.Metadata.ExifProfile.SetValue<ushort>(ExifTag.Orientation, 1);
		return result;
	}
}
